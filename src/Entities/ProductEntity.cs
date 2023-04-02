namespace UpDEV.BI.ReiDasVendas.Domains.Entities
{
    public class ProductEntity : EntityBase
    {
        public virtual string? Sku { get; set; }
        public virtual string? Name { get; set; }
        public virtual CategoryEntity? Category { get; set; }
    }
}
