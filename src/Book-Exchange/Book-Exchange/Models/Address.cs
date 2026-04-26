using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Exchange.Models;

[Table("addresses")]
public class Address
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    [Column("user_id")]
    public Guid? UserId { get; set; }
    [Required]
    [Column("location_id")]
    public Guid LocationId { get; set; }
    [Required]
    [MaxLength(200)]
    [Column("full_name")]
    public string FullName { get; set; } = string.Empty;
    [Required]
    [MaxLength(255)]
    [Column("address_line1")]
    public string AddressLine1 { get; set; } = string.Empty;
    [MaxLength(255)]
    [Column("address_line2")]
    public string? AddressLine2 { get; set; }
    [Required]
    [MaxLength(20)]
    [Column("postal_code")]
    public string PostalCode { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    public ApplicationUser? User { get; set; }
    public Location Location { get; set; } = null!;

    public ICollection<Shipment> SenderShipments { get; set; } = new List<Shipment>();
    public ICollection<Shipment> ReceiverShipments { get; set; } = new List<Shipment>();
}
