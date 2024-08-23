using System.Text.Json;
using Backend.Application.Dtos;
using Backend.Application.Exceptions;
using Backend.Application.Handlers;
using Backend.DAL.Managers;
using Backend.Models;

namespace Backend.Application.Repository;

public class MessageRepository
{
    private readonly MessageManager _messageManager;
    private readonly WebSocketHandler _webSocketHandler;

    public MessageRepository(WebSocketHandler webSocketHandler, MessageManager messageManager)
    {
        _webSocketHandler = webSocketHandler;
        _messageManager = messageManager;
    }

    /// <summary>
    /// Add a new message
    /// </summary>
    public async Task AddMessageAsync(MessageRequestDto messageDto)
    {
        if (string.IsNullOrEmpty(messageDto.Content) || messageDto.OrderId <= 0)
        {
            throw new BadRequestException("Invalid message data");
        }

        var message = await _messageManager.AddAsync(messageDto.Content, messageDto.OrderId);
        await _webSocketHandler.BroadcastMessageAsync(JsonSerializer.Serialize(message));
    }

    /// <summary>
    /// Get a list of messages for a specific date range
    /// </summary>
    /// <param name="from">Start date of the selection</param>
    /// <param name="to">The end date of the selection</param>
    /// <returns>The list of sorted messages</returns>
    public async Task<IEnumerable<Message>> GetMessagesAsync(DateTime from, DateTime to)
    {
        if (from == DateTime.MinValue || to == DateTime.MinValue || from >= to)
        {
            throw new BadRequestException("Invalid date range");
        }

        return await _messageManager.GetByDateAsync(from, to);
    }
}