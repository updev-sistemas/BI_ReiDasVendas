using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UpDEV.BI.ReiDasVendas.Infrastructures.Database
{
    public static class DatabaseExtension
    {
        public static void RegisterDatabase(this IServiceCollection service, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(service, nameof(service));
            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

            var connectionString = configuration!.GetConnectionString("Database");
            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrWhiteSpace(connectionString))
                throw new Exception("Configuração de conexão com o banco de dados inexistente");

            _ = service.AddSingleton<IDatabaseContract, SqlServerConnectionBuild>(cfg => new SqlServerConnectionBuild(connectionString!));
        }
    }
}
