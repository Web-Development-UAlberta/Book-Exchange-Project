namespace Book_Exchange.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


[Table("wishlist")]
public class WishlistItem
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

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    public ApplicationUser User { get; set; } = null!;
    public Book Book { get; set; } = null!;

    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}