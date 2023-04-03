namespace UpDEV.BI.ReiDasVendas.Domains.Entities
{
    public class TransactionFileEntity : EntityBase
    {
        public virtual string? OrderId { get; set; }
        public virtual DateTime? OrderDate { get; set; }
        public virtual string? ProductSku { get; set; }
        public virtual string? ProductName { get; set; }
        public virtual string? CategoryName { get; set; }
        public virtual string? Status { get; set; }
        public virtual DateTime? ProcessedAt { get; set; }
    }
}
