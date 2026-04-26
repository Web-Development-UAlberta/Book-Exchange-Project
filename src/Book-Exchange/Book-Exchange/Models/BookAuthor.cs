namespace Book_Exchange.Models;

using System.ComponentModel.DataAnnotations.Schema;

[Table("book_authors")]
public class BookAuthor
{
    [Column("book_id")]
    public Guid BookId { get; set; }

    [Column("author_id")]
    public Guid AuthorId { get; set; }

    public Book Book { get; set; } = null!;
    public Author Author { get; set; } = null!;
}
