using Book_Exchange.Models;

namespace Book_Exchange.Models.DTOs.Shipping;

public class ShippingQuoteDto
{
    public Carrier Carrier { get; set; } = null!;
    public int PackageWeightGrams { get; set; }
    public decimal DistanceKm { get; set; }
    public decimal EstimatedCost { get; set; }
    public decimal WeightKg => PackageWeightGrams / 1000m;
}