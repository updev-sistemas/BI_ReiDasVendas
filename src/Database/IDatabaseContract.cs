using NHibernate;

namespace UpDEV.BI.ReiDasVendas.Infrastructures.Database
{
    public interface IDatabaseContract
    {
        ISessionFactory Create();
    }
}
