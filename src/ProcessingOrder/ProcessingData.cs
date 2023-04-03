using FluentNHibernate.Data;
using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.Proxy;
using UpDEV.BI.ReiDasVendas.Domains.Entities;
using UpDEV.BI.ReiDasVendas.Infrastructures.Database;

namespace UpDEV.BI.ReiDasVendas.BusinessRules.ProcessingOrder
{
    public class ProcessingData : IProcessingData
    {
        private readonly ISession? session;
        private readonly ILogger<ProcessingData> logger;

        public ProcessingData(ILogger<ProcessingData> logger, IDatabaseContract database)
        {
            this.session = database!.Create().OpenSession();
            this.session.FlushMode = FlushMode.Auto;

            this.logger = logger;
        }

        public async Task Handler(CancellationToken cancellationToken)
        {
            var transactionsFilePending = this.session!.Query<TransactionFileEntity>().Where(p => !p.ProcessedAt.HasValue).ToArray();

            if (transactionsFilePending.Length > 0)
            {
                var group = transactionsFilePending!.GroupBy(x => x.OrderId).ToDictionary(p => p.Key!, q => q.ToArray()!);

                foreach (var rol in group)
                {
                    var transaction = this.session.BeginTransaction();
                    try
                    {
                        var categoriesDb = new List<CategoryEntity>();
                        var categories = rol.Value.Select(x => x.CategoryName).Distinct().ToArray();
                        foreach (var category in categories)
                        {
                            var categoryDb = this.session.Query<CategoryEntity>().Where(x => x.Name == category).FirstOrDefault();

                            if (categoryDb is not null)
                            {
                                categoriesDb.Add(categoryDb);
                                continue;
                            }

                            categoryDb = new CategoryEntity();
                            categoryDb.CreatedAt = DateTime.Now;
                            categoryDb.UpdatedAt = DateTime.Now;
                            categoryDb.Name = category;

                            await session.SaveAsync(categoryDb, cancellationToken).ConfigureAwait(false);

                            categoriesDb.Add(categoryDb);
                        }

                        var productsDb = new List<ProductEntity>();
                        var products = rol.Value.Select(x => x.ProductSku).Distinct().ToArray();
                        foreach (var product in products)
                        {
                            var productDb = this.session.Query<ProductEntity>().Where(x => x.Sku == product).FirstOrDefault();

                            if (productDb is not null)
                            {
                                productsDb.Add(productDb);
                                continue;
                            }

                            var catName = transactionsFilePending.Where(x => x.ProductSku == product).Select(x => x.CategoryName).Distinct().FirstOrDefault("NONE");
                            productDb = new ProductEntity
                            {
                                Category = categoriesDb.First(x => x.Name!.Equals(catName, StringComparison.InvariantCultureIgnoreCase)),
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now,
                                Name = transactionsFilePending.Where(x => x.ProductSku == product).Select(x => x.ProductName).Distinct().FirstOrDefault("NONE"),
                                Sku = product
                            };

                            await session.SaveAsync(productDb, cancellationToken).ConfigureAwait(false);

                            productsDb.Add(productDb);
                        }

                        var orderDb = this.session!.Query<OrderEntity>().Where(x => x.Code == rol.Key).FirstOrDefault();
                        orderDb ??= new OrderEntity
                        {
                            CreatedAt = DateTime.Now,
                            Code = rol.Key,
                            Date = rol.Value.Select(x => x.OrderDate).First()
                        };

                        await session.SaveOrUpdateAsync(orderDb, cancellationToken).ConfigureAwait(false);

                        foreach (var orderItem in rol.Value)
                        {
                            var itemDb = this.session!.Query<OrderItemEntity>().Where(x => x.Product!.Sku == orderItem.ProductSku && x.Order!.Code == orderDb.Code).FirstOrDefault();
                            itemDb ??= new OrderItemEntity
                            {
                                CreatedAt = DateTime.Now,
                                Product = productsDb.First(x => x.Sku == orderItem.ProductSku),
                                Order = orderDb,
                                Quantity = 1,
                            };

                            itemDb.UpdatedAt = DateTime.Now;

                            await session.SaveOrUpdateAsync(itemDb, cancellationToken).ConfigureAwait(false);
                        }

                        foreach (var item in rol.Value)
                        {
                            item!.ProcessedAt = DateTime.Now;
                            await session.UpdateAsync(item, cancellationToken).ConfigureAwait(false);
                        }

                        await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                        logger.LogError(ex.Message, ex.StackTrace);
                    }
                }
            }

            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
