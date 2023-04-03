namespace UpDEV.BI.ReiDasVendas.Applications.SpreadsheetProcess.EntityManagers
{
    public interface IEntityManager
    {
        Task Run(CancellationToken cancellationToken);
    }
}