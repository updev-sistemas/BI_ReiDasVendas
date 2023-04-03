using UpDEV.BI.ReiDasVendas.Infrastructures.Database;
using UpDEV.BI.ReiDasVendas.BusinessRules.Filemanager;
using UpDEV.BI.ReiDasVendas.BusinessRules.ProcessingOrder;
using UpDEV.BI.ReiDasVendas.Applications.SpreadsheetProcess.EntityManagers;
using UpDEV.BI.ReiDasVendas.Applications.SpreadsheetProcess;
using UpDEV.BI.ReiDasVendas.Domains.Common.Settings;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .ConfigureServices((hostContext, services) =>
    {
        ArgumentNullException.ThrowIfNull(services, "Service is Null");
        ArgumentNullException.ThrowIfNull(hostContext, "HostedBuild is Null");

        services.AddOptions();

        services.Configure<FolderConfig>(hostContext.Configuration.GetSection("Folder"));

        services.RegisterDatabase(hostContext.Configuration);

        services.AddFileManager();

        services.AddProcessingData();

        services.AddSingleton<IEntityManager, FileManagerEntityManager>();
        services.AddSingleton<IEntityManager, ProcessTransactionFileEntityManager>();
       
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
