namespace UpDEV.BI.ReiDasVendas.Domains.Entities
{
    public class OrderEntity : EntityBase
    {
        public virtual string? Code { get; set; }
        public virtual DateTime? Date { get; set; }
    }
}
