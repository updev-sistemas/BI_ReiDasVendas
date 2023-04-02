using UpDEV.BI.ReiDasVendas.Domains.Common.DBUtil;

namespace UpDEV.BI.ReiDasVendas.Domains.Entities
{
    public abstract class EntityBase : IEntity, ITimestamp
    {
        public virtual long? Id { get; set; }
        public virtual DateTime? CreatedAt { get; set; }
        public virtual DateTime? UpdatedAt { get; set; }
    }
}
