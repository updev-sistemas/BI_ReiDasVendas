using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.Extensions.Options;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System.Reflection;

namespace UpDEV.BI.ReiDasVendas.Infrastructures.Database
{
    public class SqlServerConnectionBuild : IDatabaseContract
    {
        private readonly string _dsn;

        private ISessionFactory? _sessionFactory;

        private static readonly object _lockSessionFactory = new();

        public SqlServerConnectionBuild(IOptions<ConnectionString> options)
        {
            ArgumentNullException.ThrowIfNull(options, "Configuração Inválida.");
            ArgumentNullException.ThrowIfNull(options?.Value, "Configuração Inválida.");

            if (!string.IsNullOrEmpty(options?.Value?.Dsn) || !string.IsNullOrWhiteSpace(options?.Value?.Dsn))
            {
                throw new ArgumentNullException("Configuração Inválida.");
            }

            this._dsn = options?.Value?.Dsn!;
        }

        public SqlServerConnectionBuild(string dsn)
        {
            if (string.IsNullOrEmpty(dsn) || string.IsNullOrWhiteSpace(dsn))
            {
                throw new ArgumentNullException("Configuração Inválida.");
            }

            this._dsn = dsn!;
        }

        public ISessionFactory Create()
        {
            lock (_lockSessionFactory)
            {
                if (_sessionFactory == null)
                {
                    this._sessionFactory = Fluently
                            .Configure()
                            .Database(MsSqlConfiguration.MsSql2012.ConnectionString(this._dsn!))
                            .Mappings(m =>
                            {
                                var assembly = Assembly.Load("UpDEV.BI.ReiDasVendas.Infrastructures.Database.Mappings");
                                m.HbmMappings.AddFromAssembly(Assembly.GetExecutingAssembly());
                                m.HbmMappings.AddFromAssembly(assembly);
                            })
                            .ExposeConfiguration(cfg =>
                            {
                                /**
                                 * Generate new database
                                 */
                                new SchemaUpdate(cfg).Execute(true, true);
                                new SchemaExport(cfg).Execute(true, true, false);
                                cfg.SetProperty("adonet.batch_size", "5");
                            })
                            .BuildSessionFactory();
                }
            }

            return this._sessionFactory!;
        }
    }
}
