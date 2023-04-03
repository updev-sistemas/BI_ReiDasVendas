using Microsoft.Extensions.Logging;
using UpDEV.BI.ReiDasVendas.Domains.Entities;
using UpDEV.BI.ReiDasVendas.Infrastructures.Database;
using UpDEV.BI.ReiDasVendas.Infrastructures.Filemanager.CSV;

namespace UpDEV.BI.ReiDasVendas.BusinessRules.Filemanager
{
    public class ProcessingFile : IProcessingFile
    {
        private readonly Reader? reader;
        private readonly ILogger<ProcessingFile>? logger;
        private readonly IDatabaseContract? database;

        public ProcessingFile(
            ILogger<ProcessingFile> logger,
            IDatabaseContract databaseContract)
        {
            ArgumentNullException.ThrowIfNull(databaseContract, nameof(databaseContract));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));

            this.logger = logger;
            this.database = databaseContract;

            this.reader = new Reader();
        }

        public async Task Handler(string filepath, CancellationToken cancellationToken)
        {
            try
            {
                var result = this.reader!.Build(filepath);
                if (result.Any())
                {
                    await this.RegisterAsync(result, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (Exception exception)
            {
                this.logger!.LogError(exception.Message, exception.StackTrace);
            }
        }

        private async Task RegisterAsync(IEnumerable<MagaluModel> result, CancellationToken cancellationToken)
        {
            var now = DateTime.Now;
            using var session = this.database!.Create().OpenSession();
            foreach (var model in result)
            {
                try
                {
                    var item = session.Query<TransactionFileEntity>()
                        .Where(x => x.CategoryName == model.CategoryName && x.OrderId == model.OrderId && x.ProductSku == model.ProductSku)
                        .FirstOrDefault();

                    if (item is not null)
                        continue;

                    item = new TransactionFileEntity
                    {
                        CreatedAt = now,
                        UpdatedAt = now,
                        OrderId = model!.OrderId,
                        OrderDate = model!.OrderDate,
                        ProductSku = model!.ProductSku,
                        ProductName = model!.ProductName,
                        CategoryName = model!.CategoryName,
                        Status = model!.Status,
                        ProcessedAt = default
                    };

                    await session!.SaveAsync(item, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    this.logger!.LogError(exception.Message, exception.StackTrace);
                }
            }

            await session!.FlushAsync().ConfigureAwait(false);
            session.Close();
        }
    }
}
