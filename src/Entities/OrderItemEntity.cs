namespace UpDEV.BI.ReiDasVendas.Domains.Entities
{
    public class OrderItemEntity : EntityBase
    {
        public virtual OrderEntity? Order { get; set; }
        public virtual ProductEntity? Product { get; set; }
        public virtual int? Quantity { get; set; }
    }
}
