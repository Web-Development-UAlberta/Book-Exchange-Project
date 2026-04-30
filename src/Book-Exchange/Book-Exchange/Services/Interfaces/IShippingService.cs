using Book_Exchange.Models;
using Book_Exchange.Areas.Shipping;

namespace Book_Exchange.Services.Interfaces;

public interface IShippingService
{
    Task<IEnumerable<Carrier>> GetEligibleCarriersAsync(int packageWeightGrams);

    decimal CalculateShippingCost(Carrier carrier, int packageWeightGrams, decimal distanceKm);

    Task<decimal> GetDistanceKmAsync(string senderPlaceId, string receiverPlaceId);

    Task<IEnumerable<ShippingQuoteDto>> GetQuotesAsync(Guid transactionId, Guid senderAddressId, Guid receiverAddressId, int packageWeightGrams);

    Task<Shipment> CreateShipmentAsync(Guid transactionId, Guid senderAddressId, Guid receiverAddressId, Guid carrierId, int packageWeightGrams);

    Task<Shipment?> GetShipmentByTransactionAsync(Guid transactionId);

    Task<IEnumerable<Shipment>> GetShipmentsForUserAsync(string userId);

    Task<Shipment> UpdateShipmentStatusAsync(Guid shipmentId, ShipmentStatus newStatus);

    Task<Shipment> CancelShipmentAsync(Guid shipmentId);
}