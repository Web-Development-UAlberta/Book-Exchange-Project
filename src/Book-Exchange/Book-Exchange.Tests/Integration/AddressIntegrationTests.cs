using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Address;
using Book_Exchange.Services;
using Book_Exchange.Services.Interfaces;
using Book_Exchange.Data;

// Address Integration Tests
// Covers: IT-ADDR-01 through IT-ADDR-06

namespace Book_Exchange.Tests.Integration;

// TODO: Uncomment when AddressService and IGooglePlacesService are implemented
// public class AddressServiceIntegrationTests : IDisposable
// {
//     private readonly ApplicationDbContext _db;
//     private readonly IAddressService _service;
//     private readonly Mock<IGooglePlacesService> _googlePlacesMock;

//     public AddressServiceIntegrationTests()
//     {
//         var options = new DbContextOptionsBuilder<ApplicationDbContext>()
//             .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
//             .Options;

//         _db = new ApplicationDbContext(options);
//         _googlePlacesMock = new Mock<IGooglePlacesService>();

//         // Default: any Place ID is considered valid unless overridden per test
//         _googlePlacesMock
//             .Setup(g => g.ValidatePlaceIdAsync(It.IsAny<string>()))
//             .ReturnsAsync(true);

//         _service = new AddressService(_db, _googlePlacesMock.Object);
//     }

//     public void Dispose()
//     {
//         _db.Dispose();
//     }

//     /// <summary>
//     /// IT-ADDR-01: User creates a valid address
//     /// Expected: Address is saved
//     /// </summary>
//     /// <returns>
//     /// The newly created Address, verifiable in the database.
//     /// </returns>
//     [Fact]
//     public async Task IT_ADDR_01_CreateValidAddress_IsSavedToDatabase()
//     {
//         var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "testuser" };
//         _db.Users.Add(user);
//         await _db.SaveChangesAsync();

//         var dto = new CreateAddressDto
//         {
//             FullName = "John Doe",
//             GooglePlaceId = "ChIJP3Sa8ziYEmsRUKgyFmh9AQM"
//         };

//         var result = await _service.CreateAddressAsync(dto, user.Id);

//         Assert.NotNull(result);
//         Assert.Equal(dto.FullName, result.FullName);
//         Assert.Equal(dto.GooglePlaceId, result.GooglePlaceId);
//         Assert.Equal(user.Id, result.UserId);

//         var saved = await _db.Addresses.FirstOrDefaultAsync(a => a.Id == result.Id);
//         Assert.NotNull(saved);
//     }

//     /// <summary>
//     /// IT-ADDR-02: Create address with invalid GooglePlaceId (rejected by Google Places API)
//     /// Expected: Validation fails and address is not saved
//     /// </summary>
//     /// <returns>
//     /// ArgumentException. No record should exist in the database.
//     /// </returns>
//     [Fact]
//     public async Task IT_ADDR_02_CreateAddress_InvalidPlaceId_IsNotSaved()
//     {
//         var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "testuser2" };
//         _db.Users.Add(user);
//         await _db.SaveChangesAsync();

//         _googlePlacesMock
//             .Setup(g => g.ValidatePlaceIdAsync("INVALID_PLACE_ID"))
//             .ReturnsAsync(false);

//         var dto = new CreateAddressDto
//         {
//             FullName = "John Doe",
//             GooglePlaceId = "INVALID_PLACE_ID"
//         };

//         await Assert.ThrowsAsync<ArgumentException>(
//             () => _service.CreateAddressAsync(dto, user.Id));

//         var saved = await _db.Addresses.AnyAsync(a => a.UserId == user.Id);
//         Assert.False(saved);
//     }

//     /// <summary>
//     /// IT-ADDR-03: User retrieves all their saved addresses
//     /// Expected: Only addresses belonging to them are returned
//     /// </summary>
//     /// <returns>
//     /// A list containing only the requesting user's addresses.
//     /// </returns>
//     [Fact]
//     public async Task IT_ADDR_03_GetAddressesByUserId_ReturnsOnlyOwnAddresses()
//     {
//         var userA = new ApplicationUser { Id = Guid.NewGuid(), UserName = "userA" };
//         var userB = new ApplicationUser { Id = Guid.NewGuid(), UserName = "userB" };

//         _db.Users.AddRange(userA, userB);
//         _db.Addresses.AddRange(
//             new Address { Id = Guid.NewGuid(), UserId = userA.Id, FullName = "User A", GooglePlaceId = "PlaceA" },
//             new Address { Id = Guid.NewGuid(), UserId = userB.Id, FullName = "User B", GooglePlaceId = "PlaceB" }
//         );
//         await _db.SaveChangesAsync();

//         var results = await _service.GetAddressesByUserIdAsync(userA.Id);

//         Assert.NotNull(results);
//         Assert.All(results, a => Assert.Equal(userA.Id, a.UserId));
//     }

//     /// <summary>
//     /// IT-ADDR-04: User updates their address
//     /// Expected: New address is saved with new GooglePlaceID
//     /// </summary>
//     /// <returns>
//     /// The updated Address with the new GooglePlaceId reflected in the database.
//     /// </returns>
//     [Fact]
//     public async Task IT_ADDR_04_UpdateAddress_NewPlaceId_IsValidatedAndSaved()
//     {
//         var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "testuser3" };
//         var address = new Address
//         {
//             Id = Guid.NewGuid(),
//             UserId = user.Id,
//             FullName = "Old Name",
//             GooglePlaceId = "OldPlaceId",
//             CreatedAt = DateTime.UtcNow
//         };

//         _db.Users.Add(user);
//         _db.Addresses.Add(address);
//         await _db.SaveChangesAsync();

//         var dto = new UpdateAddressDto { GooglePlaceId = "ChIJP3Sa8ziYEmsRUKgyFmh9AQM" };

//         var result = await _service.UpdateAddressAsync(address.Id, dto, user.Id);

//         Assert.NotNull(result);
//         Assert.Equal(dto.GooglePlaceId, result.GooglePlaceId);

//         var saved = await _db.Addresses.FindAsync(address.Id);
//         Assert.Equal(dto.GooglePlaceId, saved!.GooglePlaceId);

//         _googlePlacesMock.Verify(g => g.ValidatePlaceIdAsync(dto.GooglePlaceId), Times.Once);
//     }

//     /// <summary>
//     /// IT-ADDR-05: User deletes their address that has no active shipments
//     /// Expected: Address is removed
//     /// </summary>
//     /// <returns>
//     /// No record found in the database after deletion.
//     /// </returns>
//     [Fact]
//     public async Task IT_ADDR_05_DeleteAddress_NoActiveShipments_IsRemovedFromDatabase()
//     {
//         var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "testuser4" };
//         var address = new Address
//         {
//             Id = Guid.NewGuid(),
//             UserId = user.Id,
//             FullName = "To Delete",
//             GooglePlaceId = "ChIJN1t_tDeuEmsRUsoyG83frY4",
//             CreatedAt = DateTime.UtcNow
//         };

//         _db.Users.Add(user);
//         _db.Addresses.Add(address);
//         await _db.SaveChangesAsync();

//         await _service.DeleteAddressAsync(address.Id, user.Id);

//         var saved = await _db.Addresses.FindAsync(address.Id);
//         Assert.Null(saved);
//     }

//     /// <summary>
//     /// IT-ADDR-06: User deletes their address that has active shipments
//     /// Expected: Validation fails
//     /// </summary>
//     /// <returns>
//     /// InvalidOperationException. The address record still exists in the database.
//     /// </returns>
//     [Fact]
//     public async Task IT_ADDR_06_DeleteAddress_ReferencedByActiveShipment_IsBlocked()
//     {
//         var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "testuser5" };
//         var address = new Address
//         {
//             Id = Guid.NewGuid(),
//             UserId = user.Id,
//             FullName = "In Use",
//             GooglePlaceId = "ChIJN1t_tDeuEmsRUsoyG83frY4",
//             CreatedAt = DateTime.UtcNow
//         };

//         // Attach a shipment referencing this address as the sender
//         var shipment = new Shipment
//         {
//             Id = Guid.NewGuid(),
//             SenderAddressId = address.Id,
//             Status = ShipmentStatus.InTransit
//             // populate other required fields as needed
//         };

//         _db.Users.Add(user);
//         _db.Addresses.Add(address);
//         _db.Shipments.Add(shipment);
//         await _db.SaveChangesAsync();

//         await Assert.ThrowsAsync<InvalidOperationException>(
//             () => _service.DeleteAddressAsync(address.Id, user.Id));

//         var saved = await _db.Addresses.FindAsync(address.Id);
//         Assert.NotNull(saved);
//     }
// }