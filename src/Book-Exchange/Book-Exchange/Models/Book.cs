using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Book_Exchange.Models;


[Table("books")]
public class Book
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    [Required]
    [MaxLength(255)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;
    [StringLength(13)]
    [Column("isbn_13", TypeName = "char(13)")]
    public string? Isbn13 { get; set; }
    [StringLength(10)]
    [Column("isbn_10", TypeName = "char(10)")]
    public string? Isbn10 { get; set; }
    [Column("published_date")]
    public DateOnly? PublishedDate { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    public ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();
    public ICollection<Listing> Listings { get; set; } = new List<Listing>();
    public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
}   
