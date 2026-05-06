namespace Book_Exchange.Models.DTOs.Transaction;

public class TransactionViewModel
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string WithUserName { get; set; } = string.Empty;
    public decimal? TotalValue { get; set; }
    public DateTime CreatedAt { get; set; }
    public TransactionStatus CurrentStatus { get; set; }
    public bool IsActive { get; set; }
    public Guid? ShipmentId { get; set; }
    public bool HasReview { get; set; }
}