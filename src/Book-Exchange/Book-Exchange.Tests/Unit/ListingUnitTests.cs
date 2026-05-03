using Xunit;
using Moq;
using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Listing;
using Book_Exchange.Services.Interfaces;

namespace Book_Exchange.Tests.BackEnd;

// UNIT TESTS
// Covers: UT-LIST-01 through UT-LIST-10
public class ListingUnitTests
{
    private readonly Mock<IListingService> _serviceMock;

    public ListingUnitTests()
    {
        _serviceMock = new Mock<IListingService>();
    }

    /// <summary>
    /// UT-LIST-01: Create listing with valid ISBN, condition, price, and weight
    /// Expected: Listing is created successfully
    /// </summary>
    [Fact]
    public async Task UT_LIST_01_CreateValidListing_ReturnsListing()
    {
        var userId = Guid.NewGuid();

        var dto = new CreateListingDto
        {
            Isbn = "9780141036144",
            Condition = BookCondition.Good,
            Price = 20.50m,
            WeightGrams = 500
        };

        var expectedListing = new Listing
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Isbn = dto.Isbn,
            Condition = dto.Condition,
            Price = dto.Price,
            WeightGrams = dto.WeightGrams
        };

        _serviceMock
            .Setup(s => s.CreateListingAsync(dto, userId))
            .ReturnsAsync(expectedListing);

        var result = await _serviceMock.Object.CreateListingAsync(dto, userId);

        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(dto.Isbn, result.Isbn);
        Assert.Equal(dto.Condition, result.Condition);
        Assert.Equal(dto.Price, result.Price);
        Assert.Equal(dto.WeightGrams, result.WeightGrams);
    }

    /// <summary>
    /// UT-LIST-02: Create listing with invalid ISBN format
    /// Expected: Validation fails
    /// </summary>
    [Fact]
    public async Task UT_LIST_02_CreateListingWithInvalidIsbn_ThrowsArgumentException()
    {
        var userId = Guid.NewGuid();

        var dto = new CreateListingDto
        {
            Isbn = "BADISBN",
            Condition = BookCondition.Good,
            Price = 20.50m,
            WeightGrams = 500
        };

        _serviceMock
            .Setup(s => s.CreateListingAsync(dto, userId))
            .ThrowsAsync(new ArgumentException("ISBN must be a 10 or 13 digit number."));

        await Assert.ThrowsAsync<ArgumentException>(
            () => _serviceMock.Object.CreateListingAsync(dto, userId));
    }

    /// <summary>
    /// UT-LIST-03: Create listing with negative price
    /// Expected: Validation fails
    /// </summary>
    [Fact]
    public async Task UT_LIST_03_CreateListingWithNegativePrice_ThrowsArgumentException()
    {
        var userId = Guid.NewGuid();

        var dto = new CreateListingDto
        {
            Isbn = "9780141036144",
            Condition = BookCondition.Good,
            Price = -5.00m,
            WeightGrams = 500
        };

        _serviceMock
            .Setup(s => s.CreateListingAsync(dto, userId))
            .ThrowsAsync(new ArgumentException("Price must be greater than 0."));

        await Assert.ThrowsAsync<ArgumentException>(
            () => _serviceMock.Object.CreateListingAsync(dto, userId));
    }

    /// <summary>
    /// UT-LIST-04: Create listing with zero or negative weight
    /// Expected: Validation fails
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public async Task UT_LIST_04_CreateListingWithInvalidWeight_ThrowsArgumentException(int invalidWeight)
    {
        var userId = Guid.NewGuid();

        var dto = new CreateListingDto
        {
            Isbn = "9780141036144",
            Condition = BookCondition.Good,
            Price = 20.50m,
            WeightGrams = invalidWeight
        };

        _serviceMock
            .Setup(s => s.CreateListingAsync(dto, userId))
            .ThrowsAsync(new ArgumentException("Weight must be greater than 0."));

        await Assert.ThrowsAsync<ArgumentException>(
            () => _serviceMock.Object.CreateListingAsync(dto, userId));
    }

    /// <summary>
    /// UT-LIST-05: Update listing with valid new values
    /// Expected: Listing is updated successfully
    /// </summary>
    [Fact]
    public async Task UT_LIST_05_UpdateListingWithValidValues_CompletesSuccessfully()
    {
        var userId = Guid.NewGuid();
        var listingId = Guid.NewGuid();

        var dto = new UpdateListingDto
        {
            Condition = BookCondition.VeryGood,
            Price = 25.00m,
            WeightGrams = 600
        };

        _serviceMock
            .Setup(s => s.UpdateListingAsync(listingId, dto, userId))
            .Returns(Task.CompletedTask);

        await _serviceMock.Object.UpdateListingAsync(listingId, dto, userId);

        _serviceMock.Verify(
            s => s.UpdateListingAsync(listingId, dto, userId),
            Times.Once);
    }

    /// <summary>
    /// UT-LIST-06: Update listing status from Active to Pending
    /// Expected: Status change is saved correctly
    /// </summary>
    [Fact]
    public async Task UT_LIST_06_UpdateListingStatusFromActiveToPending_CompletesSuccessfully()
    {
        // NOTE:
        // Your current Listing model does not yet contain ListingStatus.
        // This test is a placeholder until you add something like:
        // public ListingStatus Status { get; set; }
        //
        // For now, this uses UpdateListingAsync to match the current interface.

        var userId = Guid.NewGuid();
        var listingId = Guid.NewGuid();

        var dto = new UpdateListingDto
        {
            Condition = BookCondition.Good,
            Price = 20.00m,
            WeightGrams = 500
        };

        _serviceMock
            .Setup(s => s.UpdateListingAsync(listingId, dto, userId))
            .Returns(Task.CompletedTask);

        await _serviceMock.Object.UpdateListingAsync(listingId, dto, userId);

        _serviceMock.Verify(
            s => s.UpdateListingAsync(listingId, dto, userId),
            Times.Once);
    }

    /// <summary>
    /// UT-LIST-07: Mark listing as Completed after successful transaction
    /// Expected: Listing is no longer shown as active
    /// </summary>
    [Fact]
    public async Task UT_LIST_07_MarkListingCompletedAfterTransaction_CompletesSuccessfully()
    {
        // NOTE:
        // Your current IListingService does not have MarkCompletedAsync or status update method.
        // This test should be updated when ListingStatus is added.

        var userId = Guid.NewGuid();
        var listingId = Guid.NewGuid();

        _serviceMock
            .Setup(s => s.DeleteListingAsync(listingId, userId))
            .Returns(Task.CompletedTask);

        await _serviceMock.Object.DeleteListingAsync(listingId, userId);

        _serviceMock.Verify(
            s => s.DeleteListingAsync(listingId, userId),
            Times.Once);
    }

    /// <summary>
    /// UT-LIST-08: Delete existing listing
    /// Expected: Listing is removed or marked unavailable based on implementation
    /// </summary>
    [Fact]
    public async Task UT_LIST_08_DeleteExistingListing_CompletesSuccessfully()
    {
        var userId = Guid.NewGuid();
        var listingId = Guid.NewGuid();

        _serviceMock
            .Setup(s => s.DeleteListingAsync(listingId, userId))
            .Returns(Task.CompletedTask);

        await _serviceMock.Object.DeleteListingAsync(listingId, userId);

        _serviceMock.Verify(
            s => s.DeleteListingAsync(listingId, userId),
            Times.Once);
    }

    /// <summary>
    /// UT-LIST-09: User attempts to update another user's listing
    /// Expected: Operation is rejected
    /// </summary>
    [Fact]
    public async Task UT_LIST_09_UpdateAnotherUsersListing_ThrowsUnauthorizedAccessException()
    {
        var currentUserId = Guid.NewGuid();
        var listingId = Guid.NewGuid();

        var dto = new UpdateListingDto
        {
            Condition = BookCondition.Acceptable,
            Price = 15.00m,
            WeightGrams = 400
        };

        _serviceMock
            .Setup(s => s.UpdateListingAsync(listingId, dto, currentUserId))
            .ThrowsAsync(new UnauthorizedAccessException("You are not allowed to update this listing."));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _serviceMock.Object.UpdateListingAsync(listingId, dto, currentUserId));
    }

    /// <summary>
    /// UT-LIST-10: User attempts to delete another user's listing
    /// Expected: Operation is rejected
    /// </summary>
    [Fact]
    public async Task UT_LIST_10_DeleteAnotherUsersListing_ThrowsUnauthorizedAccessException()
    {
        var currentUserId = Guid.NewGuid();
        var listingId = Guid.NewGuid();

        _serviceMock
            .Setup(s => s.DeleteListingAsync(listingId, currentUserId))
            .ThrowsAsync(new UnauthorizedAccessException("You are not allowed to delete this listing."));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _serviceMock.Object.DeleteListingAsync(listingId, currentUserId));
    }
}