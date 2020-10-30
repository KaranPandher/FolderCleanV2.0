using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FolderClean.Application.Common;
using FolderClean.Application.Core;
using FolderClean.Application.Core.Constants;
using FolderClean.Application.Infrastructure.Interfaces;
using FolderClean.Application.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FolderClean.Worker.Workers
{
    public class FolderCleanWorker : BackgroundService
    {
        private readonly ILogger<FolderCleanWorker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public FolderCleanWorker(ILogger<FolderCleanWorker> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            // emailService.SendEmail("h.halai0334@gmail.com", "Status of Folder Clean Up", 
            //     "Testing"
            // );
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
                    int totalNotFoundInDb = 0;
                    int totalNotCompletedInDb = 0;
                    List<string> notFoundBatchIds = new List<string>();
                    List<string> notCompletedBatchIds = new List<string>();
                    _logger.LogInformation("Found Files: " + totalFiles);
                    // Loop Over all files
                    foreach (var file in files)
                    {
                        try
                        {
                            _logger.LogInformation("Checking File: " + file);
                            totalFilesChecked++;
                            // Check for Business Checks
                            (bool isProcessed, string projectName, string batchNo) = FileService.IsBatchProcessedAndDateOlder(file, ConfigUtil.Config.Days);
                            if (isProcessed)
                            {
                                var batchName = projectName + batchNo;
                                ; var documentStats =
                                     dbContext.Batches.FirstOrDefault(p =>
                                         p.BatchName == batchName);
                                if (documentStats == null)
                                {
                                    totalNotFoundInDb++;
                                    notFoundBatchIds.Add(batchName);
                                    continue;
                                }

                                if (documentStats.AlarisToSBStatsStatus != BatchStatus.Completed)
                                {
                                    totalNotCompletedInDb++;
                                    notCompletedBatchIds.Add(batchName);
                                    continue;
                                }
                                _logger.LogInformation("Moving File: " + file);
                                // Moving File's Folder to Destination Folder
                                (string source, string destination) = FileService.MoveFolder(file, ConfigUtil.Config.DestinationFolder, projectName);
                                _logger.LogInformation("File Moved From " + source);
                                _logger.LogInformation("File Move to " + destination);
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
                    _logger.LogInformation("Total Not Found in Database: " + totalNotFoundInDb);
                    _logger.LogInformation("Total Not Completed in Database: " + totalNotCompletedInDb);
                    _logger.LogInformation("Total Errors: " + totalErrors);
                    if (totalNotCompletedInDb > 0 || totalNotFoundInDb > 0 || totalErrors > 0)
                    {
                        emailService.SendEmail("h.halai0334@gmail.com", "Status of Folder Clean Up",
                            $"Total Errors: {totalErrors},\n" +
                            $"Total Not Found in Database: {totalNotFoundInDb}" +
                            $"Not Found in Database: {string.Join(",", notFoundBatchIds)}" +
                            $"Total Not Completed in Database: {totalNotCompletedInDb}" +
                            $"Not Completed in Database: {string.Join(",", notCompletedBatchIds)}"
                            );
                    }
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