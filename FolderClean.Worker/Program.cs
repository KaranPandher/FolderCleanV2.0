using System;
using FolderClean.Application.Infrastructure.Interfaces;
using FolderClean.Application.Infrastructure.Options;
using FolderClean.Application.Infrastructure.Services;
using FolderClean.Application.Persistence;
using FolderClean.Worker.Workers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog.Extensions.Logging;

namespace FolderClean.Worker
{
    public class Program
    {

        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;
                    services.Configure<EmailOption>(configuration.GetSection("Smtp"));
                    services.AddScoped(p =>
                    {
                        var connectionString = configuration["ConnectionStrings:DefaultConnection"];
                        return new ApplicationDbContext(connectionString);
                    });
                    services.AddHostedService<FolderCleanWorker>();
                    services.AddTransient<IEmailService, EmailService>();
                    services.AddLogging(logging =>
                    {
                        logging.AddNLog();
                    });
                });
    }
}