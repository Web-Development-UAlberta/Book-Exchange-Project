using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Address;
using Book_Exchange.Services;
using Book_Exchange.Services.Interfaces;
using Book_Exchange.Data;

namespace Book_Exchange.Tests.Integration;

public class AddressServiceIntegrationTests : IDisposable
{
    private readonly ApplicationDbContext _db;
    private readonly IAddressService _service;
    private readonly Mock<IPlaceApiService> _placeApiMock;

    public AddressServiceIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _db = new ApplicationDbContext(options);

        _placeApiMock = new Mock<IPlaceApiService>();

        _placeApiMock
            .Setup(p => p.GetAddressByPlaceIdAsync(It.IsAny<string>()))
            .ReturnsAsync((string placeId) => new PlaceAddressDto
            {
                PlaceId = placeId,
                FormattedAddress = "Mock Google Address"
            });

        _service = new AddressService(_db, _placeApiMock.Object);
    }

    public void Dispose()
    {
        _db.Dispose();
    }

    [Fact]
    public async Task IT_ADDR_01_CreateValidAddress_IsSavedToDatabase()
    {
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "testuser"
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var dto = new CreateAddressDto
        {
            FullName = "User Typed Address",
            GooglePlaceId = "VALID_PLACE_ID"
        };

        var result = await _service.CreateAddressAsync(dto, user.Id);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result.UserId);
        Assert.Equal("VALID_PLACE_ID", result.GooglePlaceId);
        Assert.Equal("Mock Google Address", result.FullName);

        var saved = await _db.Addresses.FirstOrDefaultAsync(a => a.Id == result.Id);

        Assert.NotNull(saved);
        Assert.Equal(user.Id, saved!.UserId);
    }

    [Fact]
    public async Task IT_ADDR_02_CreateAddress_InvalidPlaceId_IsNotSaved()
    {
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "testuser2"
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        _placeApiMock
            .Setup(p => p.GetAddressByPlaceIdAsync("INVALID_PLACE_ID"))
            .ReturnsAsync((PlaceAddressDto?)null);

        var dto = new CreateAddressDto
        {
            FullName = "Invalid Address",
            GooglePlaceId = "INVALID_PLACE_ID"
        };

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CreateAddressAsync(dto, user.Id));

        var exists = await _db.Addresses.AnyAsync(a => a.UserId == user.Id);

        Assert.False(exists);
    }

    [Fact]
    public async Task IT_ADDR_03_GetAddressesByUserId_ReturnsOnlyOwnAddresses()
    {
        var userA = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "userA"
        };

        var userB = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "userB"
        };

        _db.Users.AddRange(userA, userB);

        _db.Addresses.AddRange(
            new Address
            {
                Id = Guid.NewGuid(),
                UserId = userA.Id,
                FullName = "User A Address",
                GooglePlaceId = "PlaceA"
            },
            new Address
            {
                Id = Guid.NewGuid(),
                UserId = userB.Id,
                FullName = "User B Address",
                GooglePlaceId = "PlaceB"
            }
        );

        await _db.SaveChangesAsync();

        var results = await _service.GetAddressesByUserIdAsync(userA.Id);

        Assert.Single(results);
        Assert.All(results, a => Assert.Equal(userA.Id, a.UserId));
    }

    [Fact]
    public async Task IT_ADDR_04_UpdateAddress_NewPlaceId_IsValidatedAndSaved()
    {
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "testuser3"
        };

        var address = new Address
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            FullName = "Old Address",
            GooglePlaceId = "OLD_PLACE_ID",
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        _db.Addresses.Add(address);
        await _db.SaveChangesAsync();

        _placeApiMock
            .Setup(p => p.GetAddressByPlaceIdAsync("NEW_PLACE_ID"))
            .ReturnsAsync(new PlaceAddressDto
            {
                PlaceId = "NEW_PLACE_ID",
                FormattedAddress = "New Google Address"
            });

        var dto = new UpdateAddressDto
        {
            FullName = "New User Typed Address",
            GooglePlaceId = "NEW_PLACE_ID"
        };

        var result = await _service.UpdateAddressAsync(address.Id, dto, user.Id);

        Assert.NotNull(result);
        Assert.Equal("NEW_PLACE_ID", result.GooglePlaceId);
        Assert.Equal("New Google Address", result.FullName);

        var saved = await _db.Addresses.FindAsync(address.Id);

        Assert.NotNull(saved);
        Assert.Equal("NEW_PLACE_ID", saved!.GooglePlaceId);
        Assert.Equal("New Google Address", saved.FullName);

        _placeApiMock.Verify(
            p => p.GetAddressByPlaceIdAsync("NEW_PLACE_ID"),
            Times.Once);
    }

    [Fact]
    public async Task IT_ADDR_05_DeleteAddress_NoActiveShipments_IsRemovedFromDatabase()
    {
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "testuser4"
        };

        var address = new Address
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            FullName = "To Delete",
            GooglePlaceId = "PLACE_TO_DELETE",
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        _db.Addresses.Add(address);
        await _db.SaveChangesAsync();

        await _service.DeleteAddressAsync(address.Id, user.Id);

        var saved = await _db.Addresses.FindAsync(address.Id);

        Assert.Null(saved);
    }

    [Fact]
    public async Task IT_ADDR_06_UserCannotAccessAnotherUsersAddress()
    {
        var userA = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "userA"
        };

        var userB = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "userB"
        };

        var address = new Address
        {
            Id = Guid.NewGuid(),
            UserId = userA.Id,
            FullName = "User A Address",
            GooglePlaceId = "USER_A_PLACE"
        };

        _db.Users.AddRange(userA, userB);
        _db.Addresses.Add(address);
        await _db.SaveChangesAsync();

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.GetAddressByIdAsync(address.Id, userB.Id));
    }
}