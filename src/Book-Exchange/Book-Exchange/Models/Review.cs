namespace Book_Exchange.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("reviews")]
public class Review
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("transaction_id")]
    public Guid TransactionId { get; set; }

    [Required]
    [Column("reviewer_id")]
    public Guid ReviewerId { get; set; }

    [Required]
    [Column("reviewee_id")]
    public Guid RevieweeId { get; set; }

    [Range(1, 5)]
    [Column("rating")]
    public int Rating { get; set; }

    [Column("comment")]
    public string? Comment { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    public Transaction Transaction { get; set; } = null!;
    public ApplicationUser Reviewer { get; set; } = null!;
    public ApplicationUser Reviewee { get; set; } = null!;
}
