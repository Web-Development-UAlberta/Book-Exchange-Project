namespace Book_Exchange.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("carrier_rates")]
public class CarrierRate
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("carrier_id")]
    public Guid CarrierId { get; set; }

    [Column("base_cost", TypeName = "numeric(8,2)")]
    public decimal BaseCost { get; set; }

    [Column("cost_per_kg", TypeName = "numeric(8,2)")]
    public decimal CostPerKg { get; set; }

    [Column("cost_per_km", TypeName = "numeric(8,4)")]
    public decimal CostPerKm { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    public Carrier Carrier { get; set; } = null!;
}
