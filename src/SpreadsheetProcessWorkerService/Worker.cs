using UpDEV.BI.ReiDasVendas.Infrastructures.Database;
using UpDEV.BI.ReiDasVendas.Infrastructures.Filemanager.CSV;

namespace SpreadsheetProcessWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker>? _logger;
        private readonly IDatabaseContract? _databaseFactory;

        public Worker(ILogger<Worker> logger, IDatabaseContract databaseContract)
        {
            this._logger = logger;
            this._databaseFactory = databaseContract;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var session = _databaseFactory!.Create().OpenSession();
                try
                {
                    var reader = new Reader();

                    var result = reader.Build(@"C:\Workspace\relatorio.csv");

                    foreach (var item in result)
                    {
                        Console.WriteLine("[{0} | {1} | {2} | {3} | {4} | {5}]", item.OrderId, item.OrderDate, item.CategoryName, item.ProductName, item.ProductSku, item.Status);
                    }

                }
                catch (Exception ex)
                {
                    this._logger!.LogError(ex.Message, ex);
                }


                _logger!.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}