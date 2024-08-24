using System.Text;
using System.Text.Json;
using Client.Writer.Dtos;
using Microsoft.Extensions.Logging;

namespace Client.Writer;

public class MessageClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiUrl;
    private int _orderId;
    private readonly ILogger<MessageClient> _logger;

    public MessageClient(string apiUrl, ILogger<MessageClient> logger)
    {
        _httpClient = new HttpClient();
        _apiUrl = apiUrl;
        _orderId = 1;
        _logger = logger;
    }

    /// <summary>
    /// Sends a stream of messages asynchronously to the server
    /// </summary>
    /// <param name="messageCount">The number of messages to send</param>
    /// <param name="delay">The delay between sending each message</param>
    public async Task SendMessagesAsync(int messageCount, TimeSpan delay)
    {
        for (var i = 0; i < messageCount; i++)
        {
            var content = GenerateRandomMessage();
            var message = new MessageDto
            {
                Content = content,
                OrderId = _orderId++
            };

            var json = JsonSerializer.Serialize(message);
            var contentString = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation($"Sending message {i + 1}/{messageCount} with content: {content}");

            try
            {
                var response = await _httpClient.PostAsync(_apiUrl, contentString);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Message {i + 1} successfully sent.");
                }
                else
                {
                    _logger.LogError($"Failed to send message {i + 1}. Response code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while sending message {i + 1}");
            }

            await Task.Delay(delay);
        }
    }

    /// <summary>
    /// Generates a random message string with a length between 10 and 128 characters
    /// </summary>
    private string GenerateRandomMessage()
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var message = new string(Enumerable.Repeat(chars, random.Next(10, 128))
            .Select(s => s[random.Next(s.Length)]).ToArray());

        _logger.LogDebug($"Generated random message: {message}");
        return message;
    }
}