using Book_Exchange.Models;
using Book_Exchange.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// TODO: Once ORM is implemented make sure nothing changes. 
namespace Book_Exchange.Areas.Message;

[Area("Message")]
[Authorize]
public class MessageController : Controller
{
    private readonly IMessageService _messageService;
    private readonly UserManager<ApplicationUser> _userManager;

    public MessageController(IMessageService messageService, UserManager<ApplicationUser> userManager)
    {
        _messageService = messageService;
        _userManager = userManager;
    }

    // Shows the inbox: list of all conversations for the current user
    // GET /Message
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = Guid.Parse(_userManager.GetUserId(User)!);
        var inbox = await _messageService.GetInboxAsync(userId);
        return View(inbox);
    }

    // Shows the full message thread between the current user and another user, Mark all as read
    // GET /Message/Conversation/{otherUserId}
    [HttpGet]
    public async Task<IActionResult> Conversation(Guid otherUserId)
    {
        var userId = Guid.Parse(_userManager.GetUserId(User)!);

        try
        {
            var messages = await _messageService.GetConversationAsync(userId, otherUserId);
            await _messageService.MarkConversationAsReadAsync(userId, otherUserId);

            ViewBag.OtherUserId = otherUserId;
            return View(messages);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // POST /Message/Send
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Send(SendMessageDto dto)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please enter a valid message.";
            return RedirectToAction(nameof(Conversation), new { otherUserId = dto.ReceiverId });
        }

        var userId = Guid.Parse(_userManager.GetUserId(User)!);

        try
        {
            await _messageService.SendMessageAsync(dto, userId);
            return RedirectToAction(nameof(Conversation), new { otherUserId = dto.ReceiverId });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Conversation), new { otherUserId = dto.ReceiverId });
        }
    }

    // Returns the current user's unread message count as JSON
    // GET /Message/UnreadCount
    [HttpGet]
    public async Task<IActionResult> UnreadCount()
    {
        var userId = Guid.Parse(_userManager.GetUserId(User)!);
        var count = await _messageService.GetUnreadCountAsync(userId);
        return Json(new { unreadCount = count });
    }
}
