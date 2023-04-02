using CsvHelper.Configuration;

namespace UpDEV.BI.ReiDasVendas.Infrastructures.Filemanager.CSV
{
    public class MagaluMap : ClassMap<MagaluModel>
    {
        public MagaluMap()
        {
            Map(p => p.OrderId).Name("id_pedido");
            Map(p => p.OrderDate).Name("data_criacao");
            Map(p => p.ProductSku).Name("sku_produto");
            Map(p => p.ProductName).Name("nome_produto");
            Map(p => p.CategoryName).Name("categoria");
            Map(p => p.Status).Name("faturado");
        }
    }
}
