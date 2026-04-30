using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Book_Exchange.Models;
using Book_Exchange.Services;
using Book_Exchange.Services.Interfaces;
using Book_Exchange.Data;

// Shipping Integration Tests
// Covers: IT-SHIP-01 through IT-SHIP-03 (Integration Tests)

namespace Book_Exchange.Tests.Integration;

// TODO: Uncomment when ShippingService is implemented
// public class ShippingServiceIntegrationTests : IDisposable
// {
//     private readonly ApplicationDbContext _db;
//     private readonly IShippingService _service;

//     public ShippingServiceIntegrationTests()
//     {
//         var options = new DbContextOptionsBuilder<ApplicationDbContext>()
//             .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
//             .Options;

//         _db = new ApplicationDbContext(options);

//         _service = new ShippingService(_db);
//         _service = null!;
//     }

//     public void Dispose() => _db.Dispose();

//     /// <summary>
//     /// IT-SHIP-01: Create shipment for transaction
//     /// Expected: Shipment is saved and linked to transaction
//     /// </summary>
//     /// <returns>
//     /// The newly created Shipment with Status = Quoted and correct TransactionId
//     /// </returns>
//     [Fact]
//     public async Task IT_SHIP_01_CreateShipment_IsSavedAndLinkedToTransaction()
//     {
//         var sender = new ApplicationUser { Id = Guid.NewGuid(), UserName = "sender" };
//         var receiver = new ApplicationUser { Id = Guid.NewGuid(), UserName = "receiver" };

//         var senderAddress = new Address
//         {
//             Id = Guid.NewGuid(),
//             UserId = sender.Id,
//             FullName = "Sender Name",
//             GooglePlaceId = "SenderPlaceId123"
//         };

//         var receiverAddress = new Address
//         {
//             Id = Guid.NewGuid(),
//             UserId = receiver.Id,
//             FullName = "Receiver Name",
//             GooglePlaceId = "ReceiverPlaceId456"
//         };

//         var carrier = new Carrier
//         {
//             Id = Guid.NewGuid(),
//             Name = "Canada Post",
//             BaseCost = 5.00m,
//             CostPerKg = 2.00m,
//             CostPerKm = 0.01m,
//             IsActive = true
//         };

//         var transactionId = Guid.NewGuid();

//         _db.Users.AddRange(sender, receiver);
//         _db.Addresses.AddRange(senderAddress, receiverAddress);
//         _db.Carriers.Add(carrier);
//         await _db.SaveChangesAsync();

//         var result = await _service.CreateShipmentAsync(
//             transactionId,
//             senderAddress.Id,
//             receiverAddress.Id,
//             carrier.Id,
//             400);

//         Assert.NotNull(result);
//         Assert.Equal(transactionId, result.TransactionId);
//         Assert.Equal(ShipmentStatus.Quoted, result.Status);

//         var saved = await _db.Shipments.FirstOrDefaultAsync(s => s.Id == result.Id);
//         Assert.NotNull(saved);
//     }

//     /// <summary>
//     /// IT-SHIP-02: Quote shipment using carrier pricing and distance
//     /// Expected: DistanceKm and ShippingCost are calculated
//     /// </summary>
//     /// <returns>
//     /// A list of ShippingQuoteDtos with EstimatedCost > 0 and DistanceKm > 0
//     /// </returns>
//     [Fact]
//     public async Task IT_SHIP_02_GetQuotes_ReturnsCalculatedCostsPerCarrier()
//     {
//         var senderAddress = new Address
//         {
//             Id = Guid.NewGuid(),
//             FullName = "Sender",
//             GooglePlaceId = "SenderPlaceId123"
//         };

//         var receiverAddress = new Address
//         {
//             Id = Guid.NewGuid(),
//             FullName = "Receiver",
//             GooglePlaceId = "ReceiverPlaceId456"
//         };

//         var carrier = new Carrier
//         {
//             Id = Guid.NewGuid(),
//             Name = "Canada Post",
//             BaseCost = 5.00m,
//             CostPerKg = 2.00m,
//             CostPerKm = 0.01m,
//             IsActive = true
//         };

//         _db.Addresses.AddRange(senderAddress, receiverAddress);
//         _db.Carriers.Add(carrier);
//         await _db.SaveChangesAsync();

//         var quotes = await _service.GetQuotesAsync(
//             Guid.NewGuid(),
//             senderAddress.Id,
//             receiverAddress.Id,
//             400);

//         Assert.NotNull(quotes);
//         Assert.NotEmpty(quotes);

//         foreach (var quote in quotes)
//         {
//             Assert.True(quote.EstimatedCost > 0);
//             Assert.True(quote.DistanceKm > 0);
//         }
//     }

//     /// <summary>
//     /// IT-SHIP-03: Update shipment status
//     /// Expected: Shipment status updates correctly
//     /// </summary>
//     /// <returns>
//     /// The updated Shipment with the new Status value
//     /// </returns>
//     [Fact]
//     public async Task IT_SHIP_03_UpdateShipmentStatus_StatusIsUpdated()
//     {
//         var shipment = new Shipment
//         {
//             Id = Guid.NewGuid(),
//             TransactionId = Guid.NewGuid(),
//             SenderAddressId = Guid.NewGuid(),
//             ReceiverAddressId = Guid.NewGuid(),
//             PackageWeightGrams = 400,
//             Status = ShipmentStatus.Quoted
//         };

//         _db.Shipments.Add(shipment);
//         await _db.SaveChangesAsync();

//         var result = await _service.UpdateShipmentStatusAsync(shipment.Id, ShipmentStatus.LabelCreated);

//         Assert.NotNull(result);
//         Assert.Equal(ShipmentStatus.LabelCreated, result.Status);

//         var saved = await _db.Shipments.FindAsync(shipment.Id);
//         Assert.Equal(ShipmentStatus.LabelCreated, saved!.Status);
//     }
// }