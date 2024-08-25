using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Client.Writer.Services;

public class MessageClientBackgroundService : BackgroundService
{
    private readonly MessageClient _messageClient;
    private readonly ILogger<MessageClientBackgroundService> _logger;

    public MessageClientBackgroundService(MessageClient messageClient, ILogger<MessageClientBackgroundService> logger)
    {
        _messageClient = messageClient;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("MessageClientBackgroundService is starting");

        stoppingToken.Register(() =>
            _logger.LogInformation("MessageClientBackgroundService is stopping"));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _messageClient.SendMessagesAsync(5000, TimeSpan.FromMilliseconds(500));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending messages");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }

        _logger.LogInformation("MessageClientBackgroundService has stopped");
    }
}