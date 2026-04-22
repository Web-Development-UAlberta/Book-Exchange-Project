namespace Book_Exchange.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("messages")]
public class Message
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("sender_id")]
    public Guid SenderId { get; set; }

    [Required]
    [Column("receiver_id")]
    public Guid ReceiverId { get; set; }

    [Column("message_text")]
    public string? MessageText { get; set; }

    [Required]
    [Column("message_type")]
    public MessageType MessageType { get; set; } = MessageType.Text;

    [Column("listing_id")]
    public Guid? ListingId { get; set; }

    [Column("transaction_id")]
    public Guid? TransactionId { get; set; }

    [Column("offer_amount", TypeName = "numeric(8,2)")]
    public decimal? OfferAmount { get; set; }

    [Required]
    [Column("status")]
    public MessageStatus Status { get; set; } = MessageStatus.Sent;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    public ApplicationUser Sender { get; set; } = null!;
    public ApplicationUser Receiver { get; set; } = null!;
    public Listing? Listing { get; set; }
    public Transaction? Transaction { get; set; }
}
