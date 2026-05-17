namespace Book_Exchange.Models;

public class Carrier
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal BaseCost { get; set; }
    public decimal CostPerKg { get; set; }
    public decimal CostPerKm { get; set; }

    public int? MaxWeightGrams { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
}