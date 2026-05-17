namespace Book_Exchange.Models.DTOs.Notification;

public class NotificationDto
{
    public Guid Id { get; set; }

    public NotificationCategory Category { get; set; }

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ReadAt { get; set; }

    public Guid? RelatedListingId { get; set; }

    public Guid? RelatedExchangeRequestId { get; set; }

    public Guid? RelatedTransactionId { get; set; }
}

public class CreateNotificationDto
{
    public Guid UserId { get; set; }

    public NotificationCategory Category { get; set; }

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public Guid? RelatedListingId { get; set; }

    public Guid? RelatedExchangeRequestId { get; set; }

    public Guid? RelatedTransactionId { get; set; }
}