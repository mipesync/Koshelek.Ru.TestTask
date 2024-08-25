using Client.Historian.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Historian.Controllers;

public class MessagesController : Controller
{
    private static List<MessageModel> _messages = [];
    private readonly string _backendUrl;

    public MessagesController(IConfiguration configuration)
    {
        _backendUrl = configuration["BackendUrl"]!;
    }
    
    public IActionResult Index()
    {
        ViewBag.BackendUrl = _backendUrl;
        return View();
    }

    [HttpGet]
    public JsonResult GetRecentMessages()
    {
        DateTime tenMinutesAgo = DateTime.Now.AddMinutes(-10);
        var recentMessages = _messages.Where(m => m.Timestamp >= tenMinutesAgo).ToList();
        
        return Json(recentMessages);
    }
}