using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Exchange.Models;

[Table("authors")]
public class Author
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(255)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
}
