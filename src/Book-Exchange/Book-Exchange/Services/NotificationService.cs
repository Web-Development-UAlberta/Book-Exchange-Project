using Book_Exchange.Data;
using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Notification;
using Book_Exchange.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Book_Exchange.Services;

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _context;

    // A notification is visible if it is unread, or was ready within the last 30 days.
    private static readonly int VisibleReadWindowDays = 30;

    public NotificationService(ApplicationDbContext context)
    {
        _context = context;
    }

    // Returns true if the notification is visible to the user (unread or read within the last 30 days)
    private static bool IsVisible(Notification n)
    {
        if (!n.IsRead) return true;
        return n.ReadAt.HasValue && n.ReadAt.Value >= DateTime.UtcNow.AddDays(-VisibleReadWindowDays);
    }

    /// <summary>
    /// GetNotificationByIdAsync
    /// - Retrieves a single notification by its ID, ensuring it belongs to the specified user
    /// - Throws KeyNotFoundException if the notification does not exist or does not belong to the user
    /// - Throws UnauthorizedAccessException if the userId does not match the notification's UserId
    /// </summary>
    /// <param name="notificationId"></param>
    /// <param name="userId"></param>
    /// <returns>
    /// A NotificationDto representing the notification with the specified ID, if it exists and belongs to the user; otherwise, an exception is thrown.
    /// </returns>
    /// <exception cref="KeyNotFoundException"></exception>
    /// <exception cref="UnauthorizedAccessException"></exception>
    public async Task<NotificationDto> GetNotificationByIdAsync(Guid notificationId, Guid userId)
    {
        var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId);

        if (notification == null)
        {
            throw new KeyNotFoundException($"Notification {notificationId} not found.");
        }
        if (notification.UserId != userId)
        {
            throw new UnauthorizedAccessException("You do not have access to this notification.");
        }
        return MapToDto(notification);
    }

    /// <summary>
    /// GetNotificationsForUserAsync
    /// - Retrieves all notifications for a user, ordered by IsRead (unread first) and then by CreatedAt descending
    /// - Returns an empty list if the user has no notifications
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>
    /// A list of NotificationDto representing the notifications for the specified user, ordered by IsRead (unread first) and then by CreatedAt descending.
    /// </returns>
    public async Task<IEnumerable<NotificationDto>> GetNotificationsForUserAsync(Guid userId)
    {
        var cutoff = DateTime.UtcNow.AddDays(-VisibleReadWindowDays);

        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && (!n.IsRead || (n.ReadAt.HasValue && n.ReadAt.Value >= cutoff)))
            .OrderBy(n => n.IsRead)
            .ThenByDescending(n => n.CreatedAt)
            .ToListAsync();

        return notifications.Select(MapToDto);
    }

    /// <summary>
    /// GetUnreadNotificationsAsync
    /// - Retrieves only the unread notifications for a user, ordered by CreatedAt descending
    /// - IsRead == false indicates an unread notification
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>
    /// A list of NotificationDto representing the unread notifications for the specified user, ordered by CreatedAt descending.
    /// </returns>
    public async Task<IEnumerable<NotificationDto>> GetUnreadNotificationsAsync(Guid userId)
    {
        var notification = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return notification.Select(MapToDto);
    }

    /// <summary>
    /// GetUnreadCountAsync
    /// - Returns the count of unread notifications for a user (where IsRead == false)
    /// - IsRead == false indicates an unread notification
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>
    /// The count of unread notifications for the specified user.
    /// </returns>
    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        return await _context.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead);
    }

    /// <summary>
    /// MarkAsReadAsync
    /// - Marks a single notification as read by setting IsRead = true and ReadAt = DateTime.UtcNow
    /// - Only the owner of the notification can mark it as read
    /// - Throws KeyNotFoundException if the notification does not exist or does not belong to the user
    /// - Throws UnauthorizedAccessException if the userId does not match the notification's UserId
    /// </summary>
    /// <param name="notificationId"></param>
    /// <param name="userId"></param>
    /// <exception cref="KeyNotFoundException"></exception>
    /// <exception cref="UnauthorizedAccessException"></exception>
    public async Task MarkAsReadAsync(Guid notificationId, Guid userId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId);

        if (notification == null)
            throw new KeyNotFoundException($"Notification {notificationId} not found.");

        if (notification.UserId != userId)
            throw new UnauthorizedAccessException("You do not have access to this notification.");

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

    }

    /// <summary>
    /// MarkAllAsReadAsync
    /// - Marks all notifications for a user as read by setting IsRead = true and ReadAt = DateTime.UtcNow for all unread notifications
    /// - Only affects notifications that belong to the specified user
    /// </summary>
    /// <param name="userId"></param>
    public async Task MarkAllAsReadAsync(Guid userId)
    {
        var unread = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        if (!unread.Any()) return;

        var now = DateTime.UtcNow;
        foreach (var notification in unread)
        {
            notification.IsRead = true;
            notification.ReadAt = now;
        }

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// ArchiveNotificationAsync
    /// - Permanently deletes a notification record
    /// - Only the owner of the notification can delete it
    /// - Throws KeyNotFoundException if the notification does not exist or does not belong to the user
    /// - Throws UnauthorizedAccessException if the userId does not match the notification's UserId
    /// </summary>
    /// <param name="notificationId"></param>
    /// <param name="userId"></param>
    /// <exception cref="KeyNotFoundException"></exception>
    /// <exception cref="UnauthorizedAccessException"></exception>
    public async Task ArchiveNotificationAsync(Guid notificationId, Guid userId)
    {
        var ntoficitation = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId);

        if (ntoficitation == null)
            throw new KeyNotFoundException($"Notification {notificationId} not found.");

        if (ntoficitation.UserId != userId)
            throw new UnauthorizedAccessException("You do not have access to this notification.");

        _context.Notifications.Remove(ntoficitation);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// CreateNotificationAsync
    /// - Creates a new notification record based on the provided DTO
    /// - Sets IsRead = false and CreatedAt = DateTime.UtcNow for the new notification
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task CreateNotificationAsync(CreateNotificationDto dto)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            Category = dto.Category,
            Title = dto.Title,
            Message = dto.Message,
            IsRead = false,
            RelatedListingId = dto.RelatedListingId,
            RelatedExchangeRequestId = dto.RelatedExchangeRequestId,
            RelatedTransactionId = dto.RelatedTransactionId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

    }

    /// <summary>
    /// Maps a Notification model to a NotificationDto
    /// </summary>
    /// <param name="notification"></param>
    private static NotificationDto MapToDto(Notification notification)
    {
        return new NotificationDto
        {
            Id = notification.Id,
            Category = notification.Category,
            Title = notification.Title,
            Message = notification.Message,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt,
            ReadAt = notification.ReadAt,
            RelatedListingId = notification.RelatedListingId,
            RelatedExchangeRequestId = notification.RelatedExchangeRequestId,
            RelatedTransactionId = notification.RelatedTransactionId
        };
    }
}
