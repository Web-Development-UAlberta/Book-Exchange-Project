using Book_Exchange.Models;
using Book_Exchange.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// TODO: Once ORM is implemented make sure nothing changes. 
namespace Book_Exchange.Controllers;

[Authorize]
public class NotificationController : Controller
{
    private readonly INotificationService _notificationService;
    private readonly UserManager<ApplicationUser> _userManager;

    public NotificationController(INotificationService notificationService, UserManager<ApplicationUser> userManager)
    {
        _notificationService = notificationService;
        _userManager = userManager;
    }

    // GET /Notification
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = Guid.Parse(_userManager.GetUserId(User)!);
        var notifications = await _notificationService.GetNotificationsForUserAsync(userId);
        return View(notifications);
    }

    // GET /Notification/Details/{id}
    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var userId = Guid.Parse(_userManager.GetUserId(User)!);

        try
        {
            var notification = await _notificationService.GetNotificationByIdAsync(id, userId);
            await _notificationService.MarkAsReadAsync(id, userId);
            return View(notification);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    // POST /Notification/MarkAsRead/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var userId = Guid.Parse(_userManager.GetUserId(User)!);

        try
        {
            await _notificationService.MarkAsReadAsync(id, userId);
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    // POST /Notification/MarkAllAsRead
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = Guid.Parse(_userManager.GetUserId(User)!);
        await _notificationService.MarkAllAsReadAsync(userId);
        TempData["Success"] = "All notifications marked as read.";
        return RedirectToAction(nameof(Index));
    }

    // POST /Notification/Archive/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Archive(Guid id)
    {
        var userId = Guid.Parse(_userManager.GetUserId(User)!);

        try
        {
            await _notificationService.ArchiveNotificationAsync(id, userId);
            TempData["Success"] = "Notification archived.";
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    // Returns the unread notification count as JSON (for the UI badge/indicator)
    // GET /Notification/UnreadCount
    [HttpGet]
    public async Task<IActionResult> UnreadCount()
    {
        var userId = Guid.Parse(_userManager.GetUserId(User)!);
        var count = await _notificationService.GetUnreadCountAsync(userId);
        return Json(new { unreadCount = count });
    }
}