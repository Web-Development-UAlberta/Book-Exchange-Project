using System.Text.Json;
using Book_Exchange.Data;
using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Shipping;
using Book_Exchange.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Book_Exchange.Services;

public class ShippingService : IShippingService
{
    private readonly ApplicationDbContext _context;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ShippingService(
        ApplicationDbContext context,
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _context = context;
        _httpClient = httpClient;
        _configuration = configuration;
    }

    /// <summary>
    /// GetEligibleCarriersAsync
    /// - Returns all active carriers that can handle the given package weight
    /// - A carrier is eligible if IsActive = true AND (MaxWeightGrams is null OR MaxWeightGrams >= packageWeightGrams)
    /// - Returns empty list if no carriers are eligible
    /// </summary>
    /// <param name="packageWeightGrams"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<IEnumerable<Carrier>> GetEligibleCarriersAsync(int packageWeightGrams)
    {
        return await _context.Carriers
            .Where(c => c.IsActive &&
                        (c.MaxWeightGrams == null || c.MaxWeightGrams >= packageWeightGrams))
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    /// <summary>
    /// CalculateShippingCost
    /// - Calculates a mock shipping cost using the carrier's pricing model
    /// - Formula: BaseCost + (WeightKg × CostPerKg) + (DistanceKm × CostPerKm)
    /// - WeightKg = packageWeightGrams / 1000
    /// </summary>
    /// <param name="carrier"></param>
    /// <param name="packageWeightGrams"></param>
    /// <param name="distanceKm"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public decimal CalculateShippingCost(Carrier carrier, int packageWeightGrams, decimal distanceKm)
    {
        if (packageWeightGrams <= 0)
            throw new ArgumentException("Package weight must be greater than zero.");

        if (distanceKm <= 0)
            throw new ArgumentException("Distance must be greater than zero.");

        decimal weightKg = packageWeightGrams / 1000m;
        return carrier.BaseCost + (weightKg * carrier.CostPerKg) + (distanceKm * carrier.CostPerKm);
    }

    /// <summary>
    /// GetDistanceKmAsync
    /// - Calls Google Distance Matrix API using two Place IDs
    /// - Returns distance in kilometres as a decimal
    /// - Throws InvalidOperationException if the API call fails or returns no results
    /// </summary>
    /// <param name="senderPlaceId"></param>
    /// <param name="receiverPlaceId"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<decimal> GetDistanceKmAsync(string senderPlaceId, string receiverPlaceId)
    {
        var apiKey = _configuration["GoogleMaps:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("Google Maps API key is missing.");

        var url =
            "https://maps.googleapis.com/maps/api/distancematrix/json" +
            $"?origins=place_id:{Uri.EscapeDataString(senderPlaceId)}" +
            $"&destinations=place_id:{Uri.EscapeDataString(receiverPlaceId)}" +
            "&units=metric" +
            $"&key={apiKey}";

        string json;
        try
        {
            json = await _httpClient.GetStringAsync(url);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Distance lookup failed. Unable to reach the distance API.", ex);
        }

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var status = root.GetProperty("status").GetString();
        if (status != "OK")
            throw new InvalidOperationException($"Distance lookup failed. Google API returned: {status}");

        var element = root
            .GetProperty("rows")[0]
            .GetProperty("elements")[0];

        var elementStatus = element.GetProperty("status").GetString();
        if (elementStatus != "OK")
            throw new InvalidOperationException($"Distance lookup failed. No route found between addresses.");

        // Value is in metres — convert to km
        var metres = element.GetProperty("distance").GetProperty("value").GetInt64();
        return Math.Round(metres / 1000m, 2);
    }

    /// <summary>
    ///  GetQuotesAsync
    /// - Retrieves eligible carriers for the given weight
    /// - Calculates distance between sender and receiver using Google Distance Matrix
    /// - Returns one ShippingQuoteDto per eligible carrier with EstimatedCost and DistanceKm populated
    /// - Throws ArgumentException if sender or receiver address is not found
    /// </summary>
    /// <param name="transactionId"></param>
    /// <param name="senderAddressId"></param>
    /// <param name="receiverAddressId"></param>
    /// <param name="packageWeightGrams"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<IEnumerable<ShippingQuoteDto>> GetQuotesAsync(
        Guid transactionId,
        Guid senderAddressId,
        Guid receiverAddressId,
        int packageWeightGrams)
    {
        var senderAddress = await _context.Addresses.FindAsync(senderAddressId)
            ?? throw new ArgumentException("Sender address not found.");

        var receiverAddress = await _context.Addresses.FindAsync(receiverAddressId)
            ?? throw new ArgumentException("Receiver address not found.");

        var carriers = await GetEligibleCarriersAsync(packageWeightGrams);
        var distanceKm = await GetDistanceKmAsync(senderAddress.GooglePlaceId, receiverAddress.GooglePlaceId);

        var quotes = carriers.Select(carrier => new ShippingQuoteDto
        {
            Carrier = carrier,
            PackageWeightGrams = packageWeightGrams,
            DistanceKm = distanceKm,
            EstimatedCost = CalculateShippingCost(carrier, packageWeightGrams, distanceKm)
        });

        return quotes;
    }

    /// <summary>
    /// CreateShipmentAsync
    /// - Validates that transaction, sender address, receiver address, and carrier all exist
    /// - Validates that the carrier is active and the package weight does not exceed MaxWeightGrams
    /// - Calculates distance and shipping cost
    /// - Creates and persists a new Shipment with Status = Quoted
    /// - Throws ArgumentException if any referenced entity is not found
    /// - Throws InvalidOperationException if carrier is inactive or weight exceeds carrier max
    /// </summary>
    /// <param name="transactionId"></param>
    /// <param name="senderAddressId"></param>
    /// <param name="receiverAddressId"></param>
    /// <param name="carrierId"></param>
    /// <param name="packageWeightGrams"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<Shipment> CreateShipmentAsync(
        Guid transactionId,
        Guid senderAddressId,
        Guid receiverAddressId,
        Guid carrierId,
        int packageWeightGrams)
    {
        var transaction = await _context.Transactions.FindAsync(transactionId)
            ?? throw new ArgumentException("Transaction not found.");

        var senderAddress = await _context.Addresses.FindAsync(senderAddressId)
            ?? throw new ArgumentException("Sender address not found.");

        var receiverAddress = await _context.Addresses.FindAsync(receiverAddressId)
            ?? throw new ArgumentException("Receiver address not found.");

        var carrier = await _context.Carriers.FindAsync(carrierId)
            ?? throw new ArgumentException("Carrier not found.");

        if (!carrier.IsActive)
            throw new InvalidOperationException("The selected carrier is not currently active.");

        if (carrier.MaxWeightGrams.HasValue && packageWeightGrams > carrier.MaxWeightGrams.Value)
            throw new InvalidOperationException("Package weight exceeds the carrier's maximum allowed weight.");

        var distanceKm = await GetDistanceKmAsync(senderAddress.GooglePlaceId, receiverAddress.GooglePlaceId);
        var shippingCost = CalculateShippingCost(carrier, packageWeightGrams, distanceKm);

        var shipment = new Shipment
        {
            Id = Guid.NewGuid(),
            TransactionId = transactionId,
            SenderAddressId = senderAddressId,
            ReceiverAddressId = receiverAddressId,
            CarrierId = carrierId,
            PackageWeightGrams = packageWeightGrams,
            DistanceKm = distanceKm,
            ShippingCost = shippingCost,
            Status = ShipmentStatus.Quoted,
            CreatedAt = DateTime.UtcNow
        };

        _context.Shipments.Add(shipment);
        await _context.SaveChangesAsync();

        return shipment;
    }

    /// <summary>
    /// GetShipmentByTransactionAsync
    /// - Returns the shipment linked to the given transaction ID, including Carrier, SenderAddress, and ReceiverAddress
    /// - Returns null if no shipment exists for the transaction
    /// </summary>
    /// <param name="transactionId"></param>
    public async Task<Shipment?> GetShipmentByTransactionAsync(Guid transactionId)
    {
        return await _context.Shipments
            .Include(s => s.Carrier)
            .Include(s => s.SenderAddress)
            .Include(s => s.ReceiverAddress)
            .FirstOrDefaultAsync(s => s.TransactionId == transactionId);
    }

    /// <summary>
    /// GetShipmentsForUserAsync
    /// - Returns all shipments where the sender or receiver address belongs to the given user
    /// - Ordered by CreatedAt descending (most recent first)
    /// - Includes Carrier, SenderAddress, ReceiverAddress, and Transaction
    /// </summary>
    /// <param name="userId"></param>
    public async Task<IEnumerable<Shipment>> GetShipmentsForUserAsync(string userId)
    {
        var userGuid = Guid.Parse(userId);

        return await _context.Shipments
            .Include(s => s.Carrier)
            .Include(s => s.SenderAddress)
            .Include(s => s.ReceiverAddress)
            .Include(s => s.Transaction)
            .Where(s =>
                s.SenderAddress.UserId == userGuid ||
                s.ReceiverAddress.UserId == userGuid)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    ///  UpdateShipmentStatusAsync
    /// - Finds the shipment by ID
    /// - Validates that the status transition is allowed (Quoted → LabelCreated → Shipped → Delivered)
    /// - Cancelled can be reached from any non-terminal status
    /// - Throws ArgumentException if shipment not found
    /// - Throws InvalidOperationException if the transition is invalid
    /// </summary>
    /// <param name="shipmentId"></param>
    /// <param name="newStatus"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<Shipment> UpdateShipmentStatusAsync(Guid shipmentId, ShipmentStatus newStatus)
    {
        var shipment = await _context.Shipments.FindAsync(shipmentId)
            ?? throw new ArgumentException("Shipment not found.");

        var allowed = GetAllowedTransitions(shipment.Status);

        if (!allowed.Contains(newStatus))
            throw new InvalidOperationException(
                $"Cannot transition shipment from {shipment.Status} to {newStatus}.");

        shipment.Status = newStatus;
        await _context.SaveChangesAsync();

        return shipment;
    }

    /// <summary>
    /// CancelShipmentAsync
    /// - Finds the shipment by ID
    /// - Validates the shipment is not already in a terminal state (Delivered or Cancelled)
    /// - Sets status to Cancelled
    /// - Throws ArgumentException if shipment not found
    /// - Throws InvalidOperationException if shipment is already delivered or cancelled
    /// </summary>
    /// <param name="shipmentId"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<Shipment> CancelShipmentAsync(Guid shipmentId)
    {
        var shipment = await _context.Shipments.FindAsync(shipmentId)
            ?? throw new ArgumentException("Shipment not found.");

        if (shipment.Status == ShipmentStatus.Delivered)
            throw new InvalidOperationException("Cannot cancel a shipment that has already been delivered.");

        if (shipment.Status == ShipmentStatus.Cancelled)
            throw new InvalidOperationException("Shipment is already cancelled.");

        shipment.Status = ShipmentStatus.Cancelled;
        await _context.SaveChangesAsync();

        return shipment;
    }

    // Private Helpers

    private static IReadOnlySet<ShipmentStatus> GetAllowedTransitions(ShipmentStatus current)
    {
        return current switch
        {
            ShipmentStatus.Pending => new HashSet<ShipmentStatus> { ShipmentStatus.Quoted, ShipmentStatus.Cancelled },
            ShipmentStatus.Quoted => new HashSet<ShipmentStatus> { ShipmentStatus.LabelCreated, ShipmentStatus.Cancelled },
            ShipmentStatus.LabelCreated => new HashSet<ShipmentStatus> { ShipmentStatus.Shipped, ShipmentStatus.Cancelled },
            ShipmentStatus.Shipped => new HashSet<ShipmentStatus> { ShipmentStatus.Delivered, ShipmentStatus.Cancelled },
            ShipmentStatus.Delivered => new HashSet<ShipmentStatus>(),
            ShipmentStatus.Cancelled => new HashSet<ShipmentStatus>(),
            _ => new HashSet<ShipmentStatus>()
        };
    }
}