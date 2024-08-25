using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Client.Observer.Models;

namespace Client.Observer.Services;

public class WebSocketService
{
    private readonly ILogger<WebSocketService> _logger;
    private readonly List<MessageModel> _messages = [];

    public WebSocketService(ILogger<WebSocketService> logger)
    {
        _logger = logger;
    }

    public IEnumerable<MessageModel> GetMessages()
    {
        _logger.LogInformation("Fetching messages from the service");
        return _messages.OrderBy(m => m.OrderId);
    }

    public async Task ConnectAndReceiveMessagesAsync(string webSocketUri)
    {
        _logger.LogInformation($"Attempting to connect to WebSocket at {webSocketUri}");
        using var clientWebSocket = new ClientWebSocket();
        
        try
        {
            await clientWebSocket.ConnectAsync(new Uri(webSocketUri), CancellationToken.None);
            _logger.LogInformation($"Successfully connected to WebSocket at {webSocketUri}");

            var buffer = new byte[1024 * 4];
            while (clientWebSocket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var messageJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        var message = JsonSerializer.Deserialize<MessageModel>(messageJson);

                        if (message != null)
                        {
                            _messages.Add(message);
                            _logger.LogInformation($"Received message: {message.Content} with OrderId: {message.OrderId}");
                        }
                        else
                        {
                            _logger.LogWarning("Received an empty message or failed to deserialize");
                        }
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        _logger.LogInformation("WebSocket connection closed by the server");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while receiving messages");
                    break;
                }
            }
        }
        catch (WebSocketException ex)
        {
            _logger.LogError(ex, $"Failed to connect to WebSocket at {webSocketUri}");
        }
    }
}