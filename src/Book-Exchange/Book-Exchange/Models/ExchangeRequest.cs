namespace Book_Exchange.Models;

public class ExchangeRequest
{
    public Guid Id { get; set; }

    public Guid TargetListingId { get; set; }
    public Listing TargetListing { get; set; } = null!;

    public Guid RequesterId { get; set; }
    public ApplicationUser Requester { get; set; } = null!;

    public ExchangeType Type { get; set; }
    public ExchangeStatus Status { get; set; } = ExchangeStatus.Requested;

    public decimal? Price { get; set; }
    public decimal? CounterOffer { get; set; }

    public string? Message { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? AcceptedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }

    public ICollection<ExchangeRequestItem> ExchangeRequestItems { get; set; } = new List<ExchangeRequestItem>();
    public Transaction? Transaction { get; set; }
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}