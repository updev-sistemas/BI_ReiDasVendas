namespace UpDEV.BI.ReiDasVendas.BusinessRules.ProcessingOrder
{
    public interface IProcessingData
    {
        Task Handler(CancellationToken cancellationToken);
    }
}