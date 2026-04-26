namespace Book_Exchange.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("notifications")]
public class Notification
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Required]
    [Column("type")]
    public NotificationType Type { get; set; }

    [Required]
    [Column("status")]
    public NotificationStatus Status { get; set; } = NotificationStatus.Unread;

    [Required]
    [MaxLength(255)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [Column("message")]
    public string Message { get; set; } = string.Empty;

    [Column("related_listing_id")]
    public Guid? RelatedListingId { get; set; }

    [Column("related_book_id")]
    public Guid? RelatedBookId { get; set; }

    [Column("related_wishlist_id")]
    public Guid? RelatedWishlistId { get; set; }

    [Column("related_transaction_id")]
    public Guid? RelatedTransactionId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("read_at")]
    public DateTime? ReadAt { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public Listing? RelatedListing { get; set; }
    public Book? RelatedBook { get; set; }
    public WishlistItem? RelatedWishlist { get; set; }
    public Transaction? RelatedTransaction { get; set; }
}
