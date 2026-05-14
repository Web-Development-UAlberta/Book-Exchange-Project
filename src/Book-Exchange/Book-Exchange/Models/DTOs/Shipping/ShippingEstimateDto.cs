namespace Book_Exchange.Models.DTOs.Shipping;

public class ShippingEstimateDto
{
    public Guid CarrierId { get; set; }
    public string CarrierName { get; set; } = string.Empty;

    public decimal BaseCost { get; set; }
    public decimal CostPerKg { get; set; }
    public decimal CostPerKm { get; set; }

    public int WeightGrams { get; set; }
    public decimal WeightKg { get; set; }

    public double DistanceKm { get; set; }

    public decimal EstimatedCost { get; set; }

    public string? Duration { get; set; }
}