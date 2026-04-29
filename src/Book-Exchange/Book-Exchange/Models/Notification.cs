namespace Book_Exchange.Models;

public class Notification
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public NotificationCategory Category { get; set; }

    public NotificationStatus Status { get; set; } = NotificationStatus.Unread;

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public Guid? RelatedListingId { get; set; }
    public Listing? RelatedListing { get; set; }

    public Guid? RelatedExchangeRequestId { get; set; }
    public ExchangeRequest? RelatedExchangeRequest { get; set; }

    public Guid? RelatedTransactionId { get; set; }
    public Transaction? RelatedTransaction { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ReadAt { get; set; }
}