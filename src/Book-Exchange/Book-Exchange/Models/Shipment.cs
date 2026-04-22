namespace Book_Exchange.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


[Table("shipments")]
public class Shipment
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("transaction_id")]
    public Guid TransactionId { get; set; }

    [Required]
    [Column("sender_address_id")]
    public Guid SenderAddressId { get; set; }

    [Required]
    [Column("receiver_address_id")]
    public Guid ReceiverAddressId { get; set; }

    [Column("carrier_id")]
    public Guid? CarrierId { get; set; }

    [Column("locality")]
    public LocalityType? Locality { get; set; }

    [Column("package_weight_kg", TypeName = "numeric(8,2)")]
    public decimal PackageWeightKg { get; set; }

    [Column("shipping_cost", TypeName = "numeric(8,2)")]
    public decimal? ShippingCost { get; set; }

    [MaxLength(100)]
    [Column("tracking_number")]
    public string? TrackingNumber { get; set; }

    [Column("label_url")]
    public string? LabelUrl { get; set; }

    [Required]
    [Column("status")]
    public ShippingStatus Status { get; set; } = ShippingStatus.Pending;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    public Transaction Transaction { get; set; } = null!;
    public Address SenderAddress { get; set; } = null!;
    public Address ReceiverAddress { get; set; } = null!;
    public Carrier? Carrier { get; set; }
}