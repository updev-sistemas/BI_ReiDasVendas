namespace UpDEV.BI.ReiDasVendas.BusinessRules.Filemanager
{
    public interface IProcessingFile
    {
        Task Handler(string filepath, CancellationToken cancellationToken);
    }
}