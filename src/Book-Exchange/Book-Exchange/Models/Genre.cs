using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Exchange.Models;

[Table("genres")]
public class Genre
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    [Required]
    [MaxLength(100)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;
}