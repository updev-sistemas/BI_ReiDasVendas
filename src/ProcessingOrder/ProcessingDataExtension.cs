using Microsoft.Extensions.DependencyInjection;

namespace UpDEV.BI.ReiDasVendas.BusinessRules.ProcessingOrder
{
    public static class ProcessingDataExtension
    {
        public static void AddProcessingData(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services, nameof(services));

            _ = services.AddSingleton<IProcessingData, ProcessingData>();
        }
    }
}
