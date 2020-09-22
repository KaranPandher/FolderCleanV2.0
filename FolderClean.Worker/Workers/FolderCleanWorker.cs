using System;
using System.Threading;
using System.Threading.Tasks;
using FolderClean.Application.Common;
using FolderClean.Application.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FolderClean.Worker.Workers
{
    public class FolderCleanWorker : BackgroundService
    {
        private readonly ILogger<FolderCleanWorker> _logger;

        public FolderCleanWorker(ILogger<FolderCleanWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            try
            {
                // Check if Settings are initialized
                if (ConfigUtil.Config == null)
                {
                    _logger.LogInformation("Reading Config File");
                    // Read Config File
                    (bool success, string exception) = ConfigUtil.ReadConfigFile();
                    if (success)
                    {
                        _logger.LogInformation("Config File Read Successfully");
                    }

                    if (!string.IsNullOrWhiteSpace(exception))
                    {
                        _logger.LogCritical(exception);
                    }
                }

                // Check if Settings are initialized
                if (ConfigUtil.Config != null)
                {
                    _logger.LogInformation("Getting All Files from Source Path");
                    // Get All files from Source folder
                    var files = FileService.GetAllFiles(ConfigUtil.Config.SourceFolder);
                    int totalFiles = files.Count;
                    int totalFilesChecked = 0;
                    int totalFilesMoved = 0;
                    int totalErrors = 0;
                    _logger.LogInformation("Found Files: " + totalFiles);
                    // Loop Over all files
                    foreach (var file in files)
                    {
                        try
                        {
                            _logger.LogInformation("Checking File: " + file);
                            totalFilesChecked++;
                            // Check for Business Checks
                            (bool isProcessed, string projectName) = FileService.IsBatchProcessedAndDateOlder(file, ConfigUtil.Config.Days);
                            if (isProcessed)
                            {
                                _logger.LogInformation("Moving File: " + file);
                                // Moving File's Folder to Destination Folder
                                FileService.MoveFolder(file, ConfigUtil.Config.DestinationFolder, projectName);
                                _logger.LogInformation("File Move to " + ConfigUtil.Config.DestinationFolder);
                                totalFilesMoved++;
                            }
                        }
                        catch (Exception e)
                        {
                            _logger.LogError("Error on File: " + file);
                            _logger.LogError(e.Message);
                            totalErrors++;
                        }
                    }
                    _logger.LogInformation("Summary");
                    _logger.LogInformation("Total Files Found: " + totalFiles);
                    _logger.LogInformation("Total Files Checked: " + totalFilesChecked);
                    _logger.LogInformation("Total Files Moved: " + totalFilesMoved);
                    _logger.LogInformation("Total Errors: " + totalErrors);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + "\n" + e.InnerException?.Message);
            }

           // Console.WriteLine("Press enter to exit");
            // Console.ReadLine();


        }
    }
}