namespace Book_Exchange.Models;

public class Shipment
{
    public Guid Id { get; set; }

    public Guid TransactionId { get; set; }
    public Transaction Transaction { get; set; } = null!;

    public Guid SenderAddressId { get; set; }
    public Address SenderAddress { get; set; } = null!;

    public Guid ReceiverAddressId { get; set; }
    public Address ReceiverAddress { get; set; } = null!;

    public Guid? CarrierId { get; set; }
    public Carrier? Carrier { get; set; }

    public int PackageWeightGrams { get; set; }

    public decimal? DistanceKm { get; set; }
    public decimal? ShippingCost { get; set; }

    public string? TrackingNumber { get; set; }
    public string? LabelUrl { get; set; }

    public ShipmentStatus Status { get; set; } = ShipmentStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}