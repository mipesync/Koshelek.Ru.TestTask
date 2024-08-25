using Client.Observer.Services;
using Microsoft.AspNetCore.Mvc;

namespace Client.Observer.Controllers;

public class MessagesController : Controller
{
    private readonly WebSocketService _webSocketService;
    
    private readonly string _webSocketUri;

    public MessagesController(WebSocketService webSocketService, IConfiguration configuration)
    {
        _webSocketService = webSocketService;
        _webSocketUri = configuration["WebSocketUrl"]!;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult GetMessages()
    {
        var messages = _webSocketService.GetMessages();
        return Json(messages);
    }

    [HttpGet]
    public async Task<IActionResult> ConnectToWebSocket()
    {
        await _webSocketService.ConnectAndReceiveMessagesAsync(_webSocketUri);

        return Ok();
    }
}