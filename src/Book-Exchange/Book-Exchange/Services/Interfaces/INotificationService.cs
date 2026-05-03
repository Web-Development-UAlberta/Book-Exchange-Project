using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Notification;

namespace Book_Exchange.Services.Interfaces;
// TODO: Once ORM is implemented make sure nothing changes. 
public interface INotificationService
{
    Task<NotificationDto> GetNotificationByIdAsync(Guid notificationId, Guid userId);

    Task<IEnumerable<NotificationDto>> GetNotificationsForUserAsync(Guid userId);

    Task<IEnumerable<NotificationDto>> GetUnreadNotificationsAsync(Guid userId);

    Task<int> GetUnreadCountAsync(Guid userId);

    Task MarkAsReadAsync(Guid notificationId, Guid userId);

    Task MarkAllAsReadAsync(Guid userId);

    Task ArchiveNotificationAsync(Guid notificationId, Guid userId);

    Task CreateNotificationAsync(CreateNotificationDto dto);
}