namespace Book_Exchange.Models;

public class Address
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public string FullName { get; set; } = null!;
    public string GooglePlaceId { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Shipment> SenderShipments { get; set; } = new List<Shipment>();
    public ICollection<Shipment> ReceiverShipments { get; set; } = new List<Shipment>();
}