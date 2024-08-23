using System.Globalization;
using Backend.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Backend.DAL.Managers;

public class MessageManager
{
    private readonly string _connectionString;
    private readonly ILogger<MessageManager> _logger;

    public MessageManager(string connectionString, IServiceProvider serviceProvider)
    {
        _connectionString = connectionString;
        _logger = serviceProvider.GetService<ILogger<MessageManager>>()!;
    }

    /// <summary>
    /// Add a new message
    /// </summary>
    /// <param name="content">Message content</param>
    /// <param name="orderId">Order number</param>
    public async Task<Message?> AddAsync(string content, int orderId)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var dateTime = DateTime.UtcNow;
            
            _logger.LogInformation("Building an SQL query to add a new message...");
            var command =
                new NpgsqlCommand(
                    "INSERT INTO messages (content, orderid, timestamp) VALUES (@content, @orderid, @timestamp) RETURNING id",
                    connection);
            command.Parameters.AddWithValue("content", content);
            command.Parameters.AddWithValue("orderid", orderId);
            command.Parameters.AddWithValue("timestamp", dateTime);

            _logger.LogInformation("Executing an SQL query...");
            await command.ExecuteNonQueryAsync();
            
            _logger.LogInformation("The message was successfully added!");
            
            var newMessageId = (int)command.ExecuteScalar()!;
            
            return new Message
            {
                Id = newMessageId,
                Content = content,
                OrderId = orderId,
                Timestamp = dateTime
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while adding a new message");
            return null;
        }
    }

    /// <summary>
    /// Get a list of messages for a specific date range
    /// </summary>
    /// <param name="from">Start date of the selection</param>
    /// <param name="to">The end date of the selection</param>
    /// <returns>The list of sorted messages</returns>
    public async Task<IEnumerable<Message>> GetByDateAsync(DateTime from, DateTime to)
    {
        try
        {
            var messages = new List<Message>();

            await using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            _logger.LogInformation($"Building an SQL query to receive messages " +
                $"from {from.ToString(CultureInfo.CurrentCulture)} to {to.ToString(CultureInfo.InvariantCulture)}...");
            var command = new NpgsqlCommand(
                "SELECT id, content, orderid, timestamp FROM messages WHERE timestamp BETWEEN @from AND @to ORDER BY timestamp",
                connection);
            command.Parameters.AddWithValue("from", from);
            command.Parameters.AddWithValue("to", to);

            _logger.LogInformation("Executing an SQL query...");
            await using var reader = await command.ExecuteReaderAsync();
            while (reader.Read())
            {
                messages.Add(new Message
                {
                    Id = reader.GetInt32(0),
                    Content = reader.GetString(1),
                    OrderId = reader.GetInt32(2),
                    Timestamp = reader.GetDateTime(3)
                });
            }

            _logger.LogInformation($"Messages were received: {messages.Count}");
            return messages;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while receiving messages");
            return new List<Message>();
        }
    }
}