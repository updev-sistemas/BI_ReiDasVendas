namespace UpDEV.BI.ReiDasVendas.Domains.Common.DBUtil
{
    public interface ITimestamp
    {
        DateTime? CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
    }
}
