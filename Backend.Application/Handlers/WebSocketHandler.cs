using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Handlers;

public class WebSocketHandler
{
    private readonly ILogger<WebSocketHandler> _logger;
    
    private static readonly ConcurrentDictionary<string, WebSocket> Sockets = new();

    public WebSocketHandler(ILogger<WebSocketHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Process a WebSocket connection by associating it with a unique connection identifier (connectionId)
    /// </summary>
    public async Task HandleWebSocketAsync(WebSocket webSocket, string connectionId)
    {
        if (!Sockets.TryAdd(connectionId, webSocket)) _logger.LogError($"Failed to register connection with id: {connectionId}");
            
        _logger.LogInformation($"A new connection with id '{connectionId}' has been registered");
        
        var buffer = new byte[1024 * 4];

        try
        {
            WebSocketReceiveResult result;
            do
            {
                _logger.LogInformation("Listening to the socket...");
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType != WebSocketMessageType.Text)
                {
                    _logger.LogWarning("The buffer data type is not equal to the text type. Skipping...");
                    continue;
                }
                
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                _logger.LogInformation("A message has been received from the data buffer");
                
                await BroadcastMessageAsync(message);

            } while (!result.CloseStatus.HasValue);

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
        finally
        {
            _logger.LogInformation($"Removing a connection from id '{connectionId}'");
            Sockets.TryRemove(connectionId, out _);
        }
    }

    /// <summary>
    /// Send a message to all connected WebSocket clients
    /// </summary>
    public async Task BroadcastMessageAsync(string message)
    {
        if (!Sockets.IsEmpty)
        {
            _logger.LogInformation($"Sending a message to {Sockets.Count} connections...");
            foreach (var socket in Sockets)
            {
                if (socket.Value.State != WebSocketState.Open)
                {
                    _logger.LogWarning($"Connection with id '{socket.Key}' is closed. Skipping...");
                    continue;
                }
                
                var messageBytes = Encoding.UTF8.GetBytes(message);
                await socket.Value.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            _logger.LogInformation("The message was successfully sent to the connections");
        }
        else
        {
            _logger.LogInformation("No connection has been registered");
        }
    }
}