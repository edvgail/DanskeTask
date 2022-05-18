using Company.Logger;
using Company.Logger.Options;
using Company.Logger.Provider;
using Company.Logger.Service;
using Company.Logger.Stream;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostingContext, configuration) =>
        {
            configuration.Sources.Clear();

            configuration
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        })
        .ConfigureServices((context, services) =>
            services.Configure<LoggerOptions>(context.Configuration.GetSection("LoggerOptions"))
            .AddTransient<ILogFile, LogFile>()
            .AddTransient<ILoggerService, LoggerService>()
            .AddTransient<ILogger, Logger>()
            .AddTransient<IDateTimeProvider, DateTimeProvider>())
        .Build();

Console.WriteLine("Logging started.");

ILogger logger = host.Services.GetRequiredService<ILogger>();
for (int i = 0; i < 15; i++)
{
    logger.Write($"Number with Flush: {i}");
    Thread.Sleep(50);
}

logger.StopWithFlush();

logger = host.Services.GetRequiredService<ILogger>();
for (int i = 50; i > 0; i--)
{
    logger.Write($"Number with No flush: {i}");
    Thread.Sleep(20);
}

logger.StopWithoutFlush();

Console.WriteLine("Logging ended.");

await host.RunAsync();
