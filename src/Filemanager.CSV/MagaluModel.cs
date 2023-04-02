using CsvHelper.Configuration.Attributes;

namespace UpDEV.BI.ReiDasVendas.Infrastructures.Filemanager.CSV
{
    public class MagaluModel
    {
        [Name("id_pedido")]
        public virtual string? OrderId { get; set; }

        [Name("data_criacao")]
        public virtual string? OrderDate { get; set; }

        [Name("sku_produto")]
        public virtual string? ProductSku { get; set; }

        [Name("nome_produto")]
        public virtual string? ProductName { get; set; }

        [Name("categoria")]
        public virtual string? CategoryName { get; set; }

        [Name("faturado")]
        public virtual string? Status { get; set; }

    }
}
