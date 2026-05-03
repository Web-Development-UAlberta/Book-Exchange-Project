using Book_Exchange.Models;
using Book_Exchange.Services.Interfaces;
using Book_Exchange.Models.DTOs.Notification;

namespace Book_Exchange.Services;

/// TODO: Once ORM is implemented make sure nothing changes.

public class NotificationService : INotificationService
{
    // private readonly AppDbContext _context;

    // public NotificationService(AppDbContext context)
    // {
    //     _context = context;
    // }

    // GetNotificationByIdAsync
    // - Retrieves a single notification by its ID, ensuring it belongs to the specified user
    // - Returns the notification if it exists and belongs to the user
    // - Throws KeyNotFoundException if the notification does not exist or does not belong to the user
    public async Task<NotificationDto> GetNotificationByIdAsync(Guid notificationId, Guid userId)
    {
        throw new NotImplementedException();
    }

    // GetNotificationsForUserAsync
    // - Retrieves all notifications for a user, ordered by IsRead (unread first) and then by CreatedAt descending
    // - Returns an empty list if the user has no notifications
    public async Task<IEnumerable<NotificationDto>> GetNotificationsForUserAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    // GetUnreadNotificationsAsync
    // - Retrieves only the unread notifications for a user, ordered by CreatedAt descending
    // - IsRead == false indicates an unread notification
    public async Task<IEnumerable<NotificationDto>> GetUnreadNotificationsAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    // GetUnreadCountAsync
    // - Returns the count of unread notifications for a user (where IsRead == false)
    // - IsRead == false indicates an unread notification
    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    // MarkAsReadAsync
    // - Marks a single notification as read by setting IsRead = true and ReadAt = DateTime.UtcNow
    // - Only the owner of the notification can mark it as read
    // - Throws KeyNotFoundException if the notification does not exist or does not belong to the user
    // - Throws UnauthorizedAccessException if the userId does not match the notification's UserId
    public async Task MarkAsReadAsync(Guid notificationId, Guid userId)
    {
        throw new NotImplementedException();
    }

    // MarkAllAsReadAsync
    // - Marks all notifications for a user as read by setting IsRead = true and ReadAt = DateTime.UtcNow for all unread notifications
    // - Only affects notifications that belong to the specified user
    public async Task MarkAllAsReadAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    // ArchiveNotificationAsync
    // - Archives a notification (soft-hides it from the main list without deleting)
    // - Only the owner of the notification can archive it
    // - Throws KeyNotFoundException if the notification does not exist or does not belong to the user
    // TODO: Do we add an IsArchived boolean to the model, or do we delete the record outright?
    public async Task ArchiveNotificationAsync(Guid notificationId, Guid userId)
    {
        throw new NotImplementedException();
    }

    // HELPER - called by other services to create a notification record
    // CreateNotificationAsync
    // - Creates a new notification based on the provided DTO
    // - Sets IsRead = false and CreatedAt = DateTime.UtcNow for the new notification
    public async Task CreateNotificationAsync(CreateNotificationDto dto)
    {
        throw new NotImplementedException();
    }

    // HELPER - Maps a Notification model to a NotificationDto
    // TODO: Commented out due to IsRead not being implemented yet
    // private static NotificationDto MapToDto(Notification notification)
    // {
    //     return new NotificationDto
    //     {
    //         Id = notification.Id,
    //         Category = notification.Category,
    //         Title = notification.Title,
    //         Message = notification.Message,
    //         IsRead = notification.IsRead,
    //         CreatedAt = notification.CreatedAt,
    //         ReadAt = notification.ReadAt,
    //         RelatedListingId = notification.RelatedListingId,
    //         RelatedExchangeRequestId = notification.RelatedExchangeRequestId,
    //         RelatedTransactionId = notification.RelatedTransactionId
    //     };
    // }
}
