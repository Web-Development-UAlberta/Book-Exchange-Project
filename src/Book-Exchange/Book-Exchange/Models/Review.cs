namespace Book_Exchange.Models;

public class Review
{
    public Guid Id { get; set; }

    public Guid TransactionId { get; set; }
    public Transaction Transaction { get; set; } = null!;

    public Guid ReviewerId { get; set; }
    public ApplicationUser Reviewer { get; set; } = null!;

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}