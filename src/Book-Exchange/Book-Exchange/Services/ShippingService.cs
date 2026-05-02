using Book_Exchange.Models;
using Book_Exchange.Services.Interfaces;
using Book_Exchange.Models.DTOs.Shipping;

namespace Book_Exchange.Services;

public class ShippingService : IShippingService
{
    // TODO: Implement once ORM is set up and database context is available.
    // private readonly ApplicationDbContext _context;

    // public ShippingService(ApplicationDbContext context)
    // {
    //     _context = context;
    // }

    // GetEligibleCarriersAsync
    // - Returns all active carriers that can handle the given package weight
    // - A carrier is eligible if IsActive = true AND (MaxWeightGrams is null OR MaxWeightGrams >= packageWeightGrams)
    // - Returns empty list if no carriers are eligible
    public Task<IEnumerable<Carrier>> GetEligibleCarriersAsync(int packageWeightGrams)
        => throw new NotImplementedException();

    // CalculateShippingCost
    // - Calculates a mock shipping cost using the carrier's pricing model
    // - Formula: BaseCost + (WeightKg × CostPerKg) + (DistanceKm × CostPerKm)
    // - WeightKg = packageWeightGrams / 1000
    public decimal CalculateShippingCost(Carrier carrier, int packageWeightGrams, decimal distanceKm)
        => throw new NotImplementedException();

    // GetDistanceKmAsync
    // - Returns the distance in kilometres between two addresses using their Google Place IDs
    // TODO: Implement using Google Distance Matrix API once integrated
    // - Returns a fixed mock distance until the API is wired up
    public Task<decimal> GetDistanceKmAsync(string senderPlaceId, string receiverPlaceId)
        => throw new NotImplementedException();

    // GetQuotesAsync
    // - Throws ArgumentException if senderAddressId or receiverAddressId does not exist
    // - Returns a ShippingQuote for each eligible carrier
    // - Each quote includes the carrier, weight, mock distance, and calculated cost
    // - Returns empty list if no carriers are eligible for the given weight
    public Task<IEnumerable<ShippingQuoteDto>> GetQuotesAsync(Guid transactionId, Guid senderAddressId, Guid receiverAddressId, int packageWeightGrams)
        => throw new NotImplementedException();

    // CreateShipmentAsync
    // - Throws ArgumentException if senderAddressId, receiverAddressId, or carrierId does not exist
    // - Throws InvalidOperationException if the carrier is not active
    // - Throws InvalidOperationException if packageWeightGrams exceeds the carrier's MaxWeightGrams
    // - Calculates and stores DistanceKm and ShippingCost at creation time
    // - Creates the shipment with Status = Quoted
    public Task<Shipment> CreateShipmentAsync(Guid transactionId, Guid senderAddressId, Guid receiverAddressId, Guid carrierId, int packageWeightGrams)
        => throw new NotImplementedException();

    // GetShipmentByTransactionAsync
    // - Returns the shipment linked to the given transaction
    // - Returns null if no shipment exists for the transaction
    public Task<Shipment?> GetShipmentByTransactionAsync(Guid transactionId)
        => throw new NotImplementedException();

    // GetShipmentsForUserAsync
    // - Returns all shipments where the user owns the sender or receiver address
    // - Returns empty list if the user has no associated shipments
    public Task<IEnumerable<Shipment>> GetShipmentsForUserAsync(string userId)
        => throw new NotImplementedException();

    // UpdateShipmentStatusAsync
    // - Throws ArgumentException if the shipment does not exist
    // - Enforces allowed transitions only:
    //     Pending → Quoted
    //     Quoted → LabelCreated
    //     LabelCreated → Shipped
    //     Shipped → Delivered
    // - Throws InvalidOperationException for any other transition
    public Task<Shipment> UpdateShipmentStatusAsync(Guid shipmentId, ShipmentStatus newStatus)
        => throw new NotImplementedException();

    // CancelShipmentAsync
    // - Throws ArgumentException if the shipment does not exist
    // - Throws InvalidOperationException if the shipment is already Shipped or Delivered
    // - Throws InvalidOperationException if the shipment is already Cancelled
    // - Sets Status = Cancelled
    public Task<Shipment> CancelShipmentAsync(Guid shipmentId)
        => throw new NotImplementedException();
}