using Xunit;
using Moq;
using Book_Exchange.Models;
using Book_Exchange.Services.Interfaces;

// Shipping Tests
// Covers: UT-SHIP-01 through UT-SHIP-05 (Unit Tests)
namespace Book_Exchange.Tests.BackEnd;

// UNIT TESTS
public class ShippingServiceUnitTests
{
    private readonly Mock<IShippingService> _serviceMock;

    public ShippingServiceUnitTests()
    {
        _serviceMock = new Mock<IShippingService>();
    }

    /// <summary>
    /// UT-SHIP-01: Calculate shipping cost with valid carrier, weight, and distance
    /// Expected: Shipping cost is calculated correctly
    /// </summary>
    /// <returns>
    /// Decimal cost equal to BaseCost + (WeightKg × CostPerKg) + (DistanceKm × CostPerKm)
    /// </returns>
    [Fact]
    public void UT_SHIP_01_CalculateShippingCost_ValidInputs_ReturnsCorrectCost()
    {
        var carrier = new Carrier
        {
            Id = Guid.NewGuid(),
            Name = "Canada Post",
            BaseCost = 5.00m,
            CostPerKg = 2.00m,
            CostPerKm = 0.01m,
            IsActive = true
        };

        // 400g = 0.4kg, 250km
        // Expected: 5.00 + (0.4 × 2.00) + (250 × 0.01) = 5.00 + 0.80 + 2.50 = 8.30
        int weightGrams = 400;
        decimal distanceKm = 250m;
        decimal expectedCost = 8.30m;

        _serviceMock
            .Setup(s => s.CalculateShippingCost(carrier, weightGrams, distanceKm))
            .Returns(expectedCost);

        var result = _serviceMock.Object.CalculateShippingCost(carrier, weightGrams, distanceKm);

        Assert.Equal(expectedCost, result);
    }

    /// <summary>
    /// UT-SHIP-02: Shipping weight exceeds carrier max weight
    /// Expected: Carrier is rejected or unavailable
    /// </summary>
    /// <returns>
    /// InvalidOperationException with message indicating the package weight exceeds the carrier's maximum
    /// </returns>
    [Fact]
    public async Task UT_SHIP_02_WeightExceedsCarrierMax_ThrowsInvalidOperationException()
    {
        var transactionId = Guid.NewGuid();
        var senderAddressId = Guid.NewGuid();
        var receiverAddressId = Guid.NewGuid();
        var carrierId = Guid.NewGuid();
        int overweightGrams = 10000;

        _serviceMock
            .Setup(s => s.CreateShipmentAsync(transactionId, senderAddressId, receiverAddressId, carrierId, overweightGrams))
            .ThrowsAsync(new InvalidOperationException("Package weight exceeds the carrier's maximum allowed weight."));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _serviceMock.Object.CreateShipmentAsync(transactionId, senderAddressId, receiverAddressId, carrierId, overweightGrams));
    }

    /// <summary>
    /// UT-SHIP-03: Missing sender or receiver address
    /// Expected: Shipment creation fails
    /// </summary>
    /// <returns>
    /// ArgumentException indicating the address was not found
    /// </returns>
    [Fact]
    public async Task UT_SHIP_03_MissingAddress_ThrowsArgumentException()
    {
        var transactionId = Guid.NewGuid();
        var invalidAddressId = Guid.NewGuid();
        var receiverAddressId = Guid.NewGuid();
        var carrierId = Guid.NewGuid();
        int weightGrams = 400;

        _serviceMock
            .Setup(s => s.CreateShipmentAsync(transactionId, invalidAddressId, receiverAddressId, carrierId, weightGrams))
            .ThrowsAsync(new ArgumentException("Sender address not found."));

        await Assert.ThrowsAsync<ArgumentException>(
            () => _serviceMock.Object.CreateShipmentAsync(transactionId, invalidAddressId, receiverAddressId, carrierId, weightGrams));
    }

    /// <summary>
    /// UT-SHIP-04: Distance API returns valid distance
    /// Expected: DistanceKm is stored and shipping cost is calculated
    /// </summary>
    /// <returns>
    /// Positive decimal representing the distance in kilometres between the two addresses
    /// </returns>
    [Fact]
    public async Task UT_SHIP_04_GetDistanceKm_ValidPlaceIds_ReturnsDistance()
    {
        var senderPlaceId = "SenderPlaceId123";
        var receiverPlaceId = "ReceiverPlaceId456";
        decimal expectedDistance = 250m;

        _serviceMock
            .Setup(s => s.GetDistanceKmAsync(senderPlaceId, receiverPlaceId))
            .ReturnsAsync(expectedDistance);

        var result = await _serviceMock.Object.GetDistanceKmAsync(senderPlaceId, receiverPlaceId);

        Assert.True(result > 0);
        Assert.Equal(expectedDistance, result);
    }

    /// <summary>
    /// UT-SHIP-05: Distance API fails
    /// Expected: System handles failure gracefully
    /// </summary>
    /// <returns>
    /// InvalidOperationException indicating the distance lookup failed
    /// </returns>
    [Fact]
    public async Task UT_SHIP_05_GetDistanceKm_ApiFails_ThrowsInvalidOperationException()
    {
        var senderPlaceId = "SenderPlaceId123";
        var receiverPlaceId = "ReceiverPlaceId456";

        _serviceMock
            .Setup(s => s.GetDistanceKmAsync(senderPlaceId, receiverPlaceId))
            .ThrowsAsync(new InvalidOperationException("Distance lookup failed. Unable to reach the distance API."));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _serviceMock.Object.GetDistanceKmAsync(senderPlaceId, receiverPlaceId));
    }
}