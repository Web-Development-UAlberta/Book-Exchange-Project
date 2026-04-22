namespace Book_Exchange.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("listings")]
public class Listing
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Required]
    [Column("book_id")]
    public Guid BookId { get; set; }

    [Required]
    [Column("condition")]
    public BookCondition Condition { get; set; }

    [Column("price", TypeName = "numeric(8,2)")]
    public decimal Price { get; set; }

    [Column("weight_kg", TypeName = "numeric(8,2)")]
    public decimal WeightKg { get; set; }

    [Required]
    [Column("type")]
    public ListingType Type { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public Book Book { get; set; } = null!;

    public ICollection<Transaction> PrimaryTransactions { get; set; } = new List<Transaction>();
    public ICollection<TransactionListing> TransactionListings { get; set; } = new List<TransactionListing>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
