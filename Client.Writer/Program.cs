using Client.Writer;
using Client.Writer.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

const string backendUrl = "http://localhost:5241/api/message";

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddLogging(config =>
        {
            config.AddConsole();
            config.SetMinimumLevel(LogLevel.Information);
        });

        services.AddSingleton<MessageClient>(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<MessageClient>>();
            return new MessageClient(backendUrl, logger);
        });

        services.AddHostedService<MessageClientBackgroundService>();
    })
    .Build();

await host.RunAsync();