using Microsoft.Extensions.Options;
using System.IO;
using UpDEV.BI.ReiDasVendas.BusinessRules.Filemanager;
using UpDEV.BI.ReiDasVendas.Domains.Common.Settings;

namespace UpDEV.BI.ReiDasVendas.Applications.SpreadsheetProcess.EntityManagers
{
    public class FileManagerEntityManager : IEntityManager
    {
        private readonly ILogger<FileManagerEntityManager> logger;
        private readonly IProcessingFile? processingFile;
        private readonly FolderConfig? folder;

        public FileManagerEntityManager(
            ILogger<FileManagerEntityManager> logger,
            IOptions<FolderConfig> options,
            IProcessingFile processingFile)
        {
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));
            ArgumentNullException.ThrowIfNull(options, nameof(options));
            ArgumentNullException.ThrowIfNull(options?.Value, nameof(options));

            this.logger = logger;
            this.processingFile = processingFile;
            this.folder = options?.Value;
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            var files = this.GetFiles();

            if (files.Any())
            {
                foreach (var file in files)
                {
                    try
                    {
                        await this.processingFile!.Handler(file, cancellationToken).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Ocorreu um erro ao processar o arquivo {file}.");
                        logger.LogError(ex.Message, ex.StackTrace);
                    }
                }
            }

            if (this.folder!.ActionAfterProcessing!.Equals("move", StringComparison.CurrentCultureIgnoreCase))
            {
                this.MoveFiles(files);
            }
            else if (this.folder!.ActionAfterProcessing!.Equals("delete", StringComparison.CurrentCultureIgnoreCase))
            {
                this.RemoveFiles(files);
            }
            else
            {
                this.logger!.LogWarning("Por falta de configuração adequada, o arquivo será removido.");
            }

            logger!.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            await Task.CompletedTask.ConfigureAwait(false);
        }

        private IEnumerable<string> GetFiles()
            => Directory.EnumerateFiles(folder!.Path!)
                        .ToArray(); 

        private void MoveFiles(IEnumerable<string> files)
        {
            var montedPath = Path.Combine(this.folder!.Path!, "Processeds");
            foreach (var file in files)
            {
                try
                {
                    var newFile = Path.Combine(montedPath, file);
                    FileInfo fileInfo = new(newFile);

                    if (!fileInfo.Exists)
                    {
                        File.Move(file, montedPath);
                    }
                }
                catch (Exception ex)
                {
                    this.logger!.LogError($"Ocorreu um erro ao processar o arquivo {file}");
                    this.logger!.LogError(ex.Message, ex.StackTrace);
                }
            }
        }

        private void RemoveFiles(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                try
                {
                    FileInfo fileInfo = new(file);

                    if (fileInfo.Exists)
                    {
                        fileInfo.IsReadOnly = false;
                        fileInfo.Delete();
                    }
                }
                catch (Exception ex)
                {
                    this.logger!.LogError($"Ocorreu um erro ao remover o arquivo {file}");
                    this.logger!.LogError(ex.Message, ex.StackTrace);
                }
            }
        }
    }
}
