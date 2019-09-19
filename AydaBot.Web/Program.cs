// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EmptyBot v4.5.0

using AydaBot.Common.Abstraction;
using AydaBot.Notifier;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AydaBot.Web
{
    public class Program
    {
        /*public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }*/

        public static void Main(string[] args)
        {
            IWebHost webHost = CreateWebHostBuilder(args).Build();

            StartJobs(webHost);

            // Run the WebHost, and start accepting requests
            // There's an async overload, so we may as well use it
            webHost.Run();
        }


        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, logging) =>
                {
                    //logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                    //logging.AddEventSourceLogger();
                })
                /*//add Microsoft.Extensions.Logging.AzureAppServices
                .ConfigureLogging(logging => logging.AddAzureWebAppDiagnostics())
                .ConfigureServices(serviceCollection => serviceCollection
                .Configure<AzureFileLoggerOptions>(options =>
                {
                    options.FileName = "azure-diagnostics-";
                    options.FileSizeLimit = 50 * 1024;
                    options.RetainedFileCountLimit = 5;
                })*/
                .UseStartup<Startup>();

        private static void StartJobs(IWebHost webHost)
        {
            var adapter = (IBotFrameworkHttpAdapter)webHost
                .Services
                .GetService(typeof(IBotFrameworkHttpAdapter));
            var storage = (IStorage)webHost
                .Services
                .GetService(typeof(IStorage));
            var configuration = (IConfiguration)webHost
                .Services
                .GetService(typeof(IConfiguration));
            var notifierLogger = (ILogger<NotifyService>)webHost
                .Services
                .GetService(typeof(ILogger<NotifyService>));
            var crawlerLogger = (ILogger<CrawlerService>)webHost
                .Services
                .GetService(typeof(ILogger<CrawlerService>));

            //notifier
            var notifier = new NotifyService(adapter, configuration["MicrosoftAppId"],
                storage, notifierLogger);            
            Task.Factory.StartNew(notifier.Run, TaskCreationOptions.LongRunning);

            //crawler
            var crowler = new CrawlerService(storage, crawlerLogger);
            Task.Factory.StartNew(crowler.Run, TaskCreationOptions.LongRunning);
        }
    }
}
