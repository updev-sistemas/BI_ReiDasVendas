using Microsoft.Extensions.DependencyInjection;

namespace UpDEV.BI.ReiDasVendas.BusinessRules.Filemanager
{
    public static class FileManagerExtension
    {
        public static void AddFileManager(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services, nameof(services));

            _ = services.AddSingleton<IProcessingFile, ProcessingFile>();
        }
    }
}
