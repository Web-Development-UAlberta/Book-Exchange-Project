namespace Book_Exchange.Models;

public class TransactionStatusHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TransactionId { get; set; }
    public Guid UpdatedByUserId { get; set; }
    public TransactionStatus Status { get; set; } = TransactionStatus.Confirmed;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public Transaction Transaction { get; set; } = null!;
    public ApplicationUser UpdatedByUser { get; set; } = null!;
}