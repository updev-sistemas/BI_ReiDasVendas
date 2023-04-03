using UpDEV.BI.ReiDasVendas.BusinessRules.ProcessingOrder;

namespace UpDEV.BI.ReiDasVendas.Applications.SpreadsheetProcess.EntityManagers
{
    public class ProcessTransactionFileEntityManager : IEntityManager
    {
        private readonly ILogger<ProcessTransactionFileEntityManager> logger;
        private readonly IProcessingData? processingData;

        public ProcessTransactionFileEntityManager(
            ILogger<ProcessTransactionFileEntityManager> logger,
            IProcessingData processingData)
        {
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));
            ArgumentNullException.ThrowIfNull(processingData, nameof(processingData));

            this.logger = logger;
            this.processingData = processingData;
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            await this.processingData!.Handler(cancellationToken).ConfigureAwait(false);
            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
