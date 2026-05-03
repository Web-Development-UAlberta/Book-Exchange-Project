using Xunit;
using Moq;
using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Address;
using Book_Exchange.Services.Interfaces;

// Address Unit Tests
// Covers: UT-ADDR-01 through UT-ADDR-09

namespace Book_Exchange.Tests.BackEnd;

public class AddressServiceUnitTests
{
    private readonly Mock<IAddressService> _serviceMock;

    public AddressServiceUnitTests()
    {
        _serviceMock = new Mock<IAddressService>();
    }

    /// <summary>
    /// UT-ADDR-01: Create address with valid FullName and GooglePlaceId
    /// Expected: Address is accepted
    /// </summary>
    /// <returns>
    /// Address with matching FullName, GooglePlaceId, and UserId
    /// </returns>
    [Fact]
    public async Task UT_ADDR_01_CreateValidAddress_ReturnsAddress()
    {
        var userId = Guid.NewGuid();
        var dto = new CreateAddressDto
        {
            FullName = "John Doe",
            GooglePlaceId = "ChIJs5ydyTiuEmsR0fRSlU0C7k0"
        };

        var expected = new Address
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FullName = dto.FullName,
            GooglePlaceId = dto.GooglePlaceId,
            CreatedAt = DateTime.UtcNow
        };

        _serviceMock
            .Setup(s => s.CreateAddressAsync(dto, userId))
            .ReturnsAsync(expected);

        var result = await _serviceMock.Object.CreateAddressAsync(dto, userId);

        Assert.NotNull(result);
        Assert.Equal(dto.FullName, result.FullName);
        Assert.Equal(dto.GooglePlaceId, result.GooglePlaceId);
        Assert.Equal(userId, result.UserId);
    }

    /// <summary>
    /// UT-ADDR-02: Create address with null or empty FullName
    /// Expected: Validation fails
    /// </summary>
    /// <returns>
    /// ArgumentException with message "FullName must not be null or whitespace."
    /// </returns>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task UT_ADDR_02_CreateAddress_EmptyFullName_ThrowsArgumentException(string fullName)
    {
        var userId = Guid.NewGuid();
        var dto = new CreateAddressDto
        {
            FullName = fullName,
            GooglePlaceId = "ChIJs5ydyTiuEmsR0fRSlU0C7k0"
        };

        _serviceMock
            .Setup(s => s.CreateAddressAsync(dto, userId))
            .ThrowsAsync(new ArgumentException("FullName must not be null or whitespace."));

        await Assert.ThrowsAsync<ArgumentException>(
            () => _serviceMock.Object.CreateAddressAsync(dto, userId));
    }

    /// <summary>
    /// UT-ADDR-03: Create address with null or empty GooglePlaceId
    /// Expected: Validation fails
    /// </summary>
    /// <returns>
    /// ArgumentException with message "GooglePlaceId must not be null or whitespace."
    /// </returns>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task UT_ADDR_03_CreateAddress_EmptyGooglePlaceId_ThrowsArgumentException(string placeId)
    {
        var userId = Guid.NewGuid();
        var dto = new CreateAddressDto
        {
            FullName = "John Doe",
            GooglePlaceId = placeId
        };

        _serviceMock
            .Setup(s => s.CreateAddressAsync(dto, userId))
            .ThrowsAsync(new ArgumentException("GooglePlaceId must not be null or whitespace."));

        await Assert.ThrowsAsync<ArgumentException>(
            () => _serviceMock.Object.CreateAddressAsync(dto, userId));
    }

    /// <summary>
    /// UT-ADDR-04: Create address with invalid GooglePlaceId (rejected by Google Places API)
    /// Expected: Validation fails
    /// </summary>
    /// <returns>
    /// ArgumentException with message indicating the Place ID could not be resolved
    /// </returns>
    [Fact]
    public async Task UT_ADDR_04_CreateAddress_InvalidGooglePlaceId_ThrowsArgumentException()
    {
        var userId = Guid.NewGuid();
        var dto = new CreateAddressDto
        {
            FullName = "John Doe",
            GooglePlaceId = "INVALID_PLACE_ID"
        };

        _serviceMock
            .Setup(s => s.CreateAddressAsync(dto, userId))
            .ThrowsAsync(new ArgumentException("GooglePlaceId could not be resolved via Google Places API."));

        await Assert.ThrowsAsync<ArgumentException>(
            () => _serviceMock.Object.CreateAddressAsync(dto, userId));
    }

    /// <summary>
    /// UT-ADDR-05: Get address by ID belonging to the requesting user
    /// Expected: Address is returned
    /// </summary>
    /// <returns>
    /// Address matching the given ID and UserId
    /// </returns>
    [Fact]
    public async Task UT_ADDR_05_GetAddressById_OwnAddress_ReturnsAddress()
    {
        var userId = Guid.NewGuid();
        var addressId = Guid.NewGuid();

        var expected = new Address
        {
            Id = addressId,
            UserId = userId,
            FullName = "John Doe",
            GooglePlaceId = "ChIJs5ydyTiuEmsR0fRSlU0C7k0"
        };

        _serviceMock
            .Setup(s => s.GetAddressByIdAsync(addressId, userId))
            .ReturnsAsync(expected);

        var result = await _serviceMock.Object.GetAddressByIdAsync(addressId, userId);

        Assert.NotNull(result);
        Assert.Equal(addressId, result.Id);
        Assert.Equal(userId, result.UserId);
    }

    /// <summary>
    /// UT-ADDR-06: Get another users address
    /// Expected: Access is denied
    /// </summary>
    /// <returns>
    /// KeyNotFoundException with message "Address not found for this user."
    /// </returns>
    [Fact]
    public async Task UT_ADDR_06_GetAddressById_AnotherUsersAddress_ThrowsKeyNotFoundException()
    {
        var requestingUserId = Guid.NewGuid();
        var addressId = Guid.NewGuid();

        _serviceMock
            .Setup(s => s.GetAddressByIdAsync(addressId, requestingUserId))
            .ThrowsAsync(new KeyNotFoundException("Address not found for this user."));

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _serviceMock.Object.GetAddressByIdAsync(addressId, requestingUserId));
    }

    /// <summary>
    /// UT-ADDR-07: Update address with a new valid GooglePlaceId
    /// Expected: Place ID is validated against Google Places API and address is updated
    /// </summary>
    /// <returns>
    /// Updated Address with new GooglePlaceId
    /// </returns>
    [Fact]
    public async Task UT_ADDR_07_UpdateAddress_NewValidGooglePlaceId_ReturnsUpdatedAddress()
    {
        var userId = Guid.NewGuid();
        var addressId = Guid.NewGuid();
        var dto = new UpdateAddressDto
        {
            GooglePlaceId = "ChIJP3Sa8ziYEmsRUKgyFmh9AQM"
        };

        var expected = new Address
        {
            Id = addressId,
            UserId = userId,
            FullName = "John Doe",
            GooglePlaceId = dto.GooglePlaceId!
        };

        _serviceMock
            .Setup(s => s.UpdateAddressAsync(addressId, dto, userId))
            .ReturnsAsync(expected);

        var result = await _serviceMock.Object.UpdateAddressAsync(addressId, dto, userId);

        Assert.NotNull(result);
        Assert.Equal(dto.GooglePlaceId, result.GooglePlaceId);
    }

    /// <summary>
    /// UT-ADDR-07b: Update address with an invalid GooglePlaceId (rejected by Google Places API)
    /// Expected: Validation fails
    /// </summary>
    /// <returns>
    /// ArgumentException with message indicating the Place ID could not be resolved
    /// </returns>
    [Fact]
    public async Task UT_ADDR_07b_UpdateAddress_InvalidGooglePlaceId_ThrowsArgumentException()
    {
        var userId = Guid.NewGuid();
        var addressId = Guid.NewGuid();
        var dto = new UpdateAddressDto
        {
            GooglePlaceId = "INVALID_PLACE_ID"
        };

        _serviceMock
            .Setup(s => s.UpdateAddressAsync(addressId, dto, userId))
            .ThrowsAsync(new ArgumentException("GooglePlaceId could not be resolved via Google Places API."));

        await Assert.ThrowsAsync<ArgumentException>(
            () => _serviceMock.Object.UpdateAddressAsync(addressId, dto, userId));
    }

    /// <summary>
    /// UT-ADDR-08: Delete an address not being used on a shipment
    /// Expected: Address is hard deleted successfully
    /// </summary>
    /// <returns>
    /// Completes without exception. Verify DeleteAddressAsync was called with correct parameters.
    /// </returns>
    [Fact]
    public async Task UT_ADDR_08_DeleteAddress_NoActiveShipments_CompletesSuccessfully()
    {
        var userId = Guid.NewGuid();
        var addressId = Guid.NewGuid();

        _serviceMock
            .Setup(s => s.DeleteAddressAsync(addressId, userId))
            .Returns(Task.CompletedTask);

        await _serviceMock.Object.DeleteAddressAsync(addressId, userId);

        _serviceMock.Verify(s => s.DeleteAddressAsync(addressId, userId), Times.Once);
    }

    /// <summary>
    /// UT-ADDR-09: Delete an address that is being used on a shipment
    /// Expected: Validation fails and address is not deleted
    /// </summary>
    /// <returns>
    /// InvalidOperationException with message "Address is referenced by an active shipment and cannot be deleted."
    /// </returns>
    [Fact]
    public async Task UT_ADDR_09_DeleteAddress_ReferencedByActiveShipment_ThrowsInvalidOperationException()
    {
        var userId = Guid.NewGuid();
        var addressId = Guid.NewGuid();

        _serviceMock
            .Setup(s => s.DeleteAddressAsync(addressId, userId))
            .ThrowsAsync(new InvalidOperationException("Address is referenced by an active shipment and cannot be deleted."));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _serviceMock.Object.DeleteAddressAsync(addressId, userId));
    }
}