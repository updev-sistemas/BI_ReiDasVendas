using SpreadsheetProcessWorkerService;
using UpDEV.BI.ReiDasVendas.Infrastructures.Database;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.RegisterDatabase(hostContext.Configuration);

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
