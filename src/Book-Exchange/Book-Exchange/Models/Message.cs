namespace Book_Exchange.Models;

public class Message
{
    public Guid Id { get; set; }

    public Guid SenderId { get; set; }
    public ApplicationUser Sender { get; set; } = null!;

    public Guid ReceiverId { get; set; }
    public ApplicationUser Receiver { get; set; } = null!;

    public string? MessageText { get; set; }

    public bool IsRead { get; set; } = false;

    public Guid? ListingId { get; set; }
    public Listing? Listing { get; set; }

    public Guid? ExchangeRequestId { get; set; }
    public ExchangeRequest? ExchangeRequest { get; set; }

    public Guid? TransactionId { get; set; }
    public Transaction? Transaction { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}