using Backend.Application.Dtos;
using Backend.Application.Repository;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessageController : Controller
{
    private readonly MessageRepository _repository;

    public MessageController(MessageRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Add a new message
    /// </summary>
    /// <param name="messageDto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> AddMessageAsync([FromBody] MessageRequestDto messageDto)
    {
        await _repository.AddMessageAsync(messageDto);
        return Ok();
    }

    /// <summary>
    /// Get a list of messages for a specific date range
    /// </summary>
    /// <param name="from">Start date of the selection</param>
    /// <param name="to">The end date of the selection</param>
    /// <returns>The list of sorted messages</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Message>>> GetMessagesAsync([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var messages = await _repository.GetMessagesAsync(from, to);
        return Ok(messages);
    }
}