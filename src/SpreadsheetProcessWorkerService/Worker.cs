using UpDEV.BI.ReiDasVendas.Applications.SpreadsheetProcess.EntityManagers;

namespace UpDEV.BI.ReiDasVendas.Applications.SpreadsheetProcess
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker>? logger;
        private readonly IEnumerable<IEntityManager> entityManagers;

        public Worker(
            ILogger<Worker> logger,
            IEnumerable<IEntityManager> entityManagers)
        {
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));
            ArgumentNullException.ThrowIfNull(entityManagers, nameof(entityManagers));

            this.logger = logger;
            this.entityManagers = entityManagers;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var em in entityManagers)
                {
                    try
                    {
                        await em.Run(stoppingToken).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        this.logger!.LogError($"Ocorreu um erro ao processar o EntityManager {nameof(em)}");
                        this.logger!.LogError(ex.Message, ex.StackTrace);
                    }
                }
            }
        }
    }
}