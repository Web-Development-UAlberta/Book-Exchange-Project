using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Exchange.Models;

[Table("book_genres")]
public class BookGenre
{
    [Column("book_id")]
    public Guid BookId { get; set; }
    [Column("genre_id")]
    public Guid GenreId { get; set; }
    public Book Book { get; set; } = null!;
    public Genre Genre { get; set; } = null!;
}
