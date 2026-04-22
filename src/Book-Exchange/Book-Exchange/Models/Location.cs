using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace Book_Exchange.Models;

[Table("locations")]
public class Location

{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    [Required]
    [MaxLength(100)]
    [Column("city")]
    public string City { get; set; } = string.Empty;
    [Required]
    [MaxLength(100)]
    [Column("province_state")]
    public string ProvinceState { get; set; } = string.Empty;
    [Required]
    [MaxLength(100)]
    [Column("country")]
    public string Country { get; set; } = "Canada";

    public ICollection<Address> Addresses { get; set; } = new List<Address>();

    public ICollection<LocationDistance> DistancesFrom { get; set; } = new List<LocationDistance>();
    public ICollection<LocationDistance> DistancesTo { get; set; } = new List<LocationDistance>();
}