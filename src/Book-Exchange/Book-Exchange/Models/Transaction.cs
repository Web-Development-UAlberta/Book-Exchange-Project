namespace Book_Exchange.Models;

public class Transaction
{
    public Guid Id { get; set; }

    public Guid ExchangeRequestId { get; set; }
    public ExchangeRequest ExchangeRequest { get; set; } = null!;

    public TransactionStatus Status { get; set; } = TransactionStatus.Confirmed;

    public decimal? TotalValue { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }

    public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
}