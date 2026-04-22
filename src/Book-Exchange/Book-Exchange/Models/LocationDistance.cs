using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Exchange.Models;

[Table("location_distances")]
public class LocationDistance
{
    [Column("from_location_id")]
    public Guid FromLocationId { get; set; }
    [Column("to_location_id")]
    public Guid ToLocationId { get; set; }
    [Column("distance_km", TypeName = "numeric(10,2)")]
    public decimal DistanceKm { get; set; }

    public Location FromLocation { get; set; } = null!;
    public Location ToLocation { get; set; } = null!;
}