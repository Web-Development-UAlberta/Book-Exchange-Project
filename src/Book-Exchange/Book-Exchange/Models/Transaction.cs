namespace Book_Exchange.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


[Table("transactions")]
public class Transaction
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("type")]
    public TransactionType Type { get; set; }

    [Column("buyer_id")]
    public Guid? BuyerId { get; set; }

    [Column("seller_id")]
    public Guid? SellerId { get; set; }

    [Column("listing_id")]
    public Guid? ListingId { get; set; }

    [Required]
    [Column("status")]
    public TransactionStatus Status { get; set; } = TransactionStatus.Proposed;

    [Column("total_value", TypeName = "numeric(8,2)")]
    public decimal? TotalValue { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("confirmed_at")]
    public DateTime? ConfirmedAt { get; set; }

    [Column("completed_at")]
    public DateTime? CompletedAt { get; set; }

    [Column("cancelled_at")]
    public DateTime? CancelledAt { get; set; }

    public ApplicationUser? Buyer { get; set; }
    public ApplicationUser? Seller { get; set; }
    public Listing? Listing { get; set; }

    public ICollection<TransactionListing> TransactionListings { get; set; } = new List<TransactionListing>();
    public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
