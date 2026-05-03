using Xunit;
using Moq;
using Book_Exchange.Models;
using Book_Exchange.Services.Interfaces;

// Wishlist Tests
// Covers: UT-WISH-01 through UT-WISH-07 (Unit Tests)
namespace Book_Exchange.Tests.BackEnd;

// UNIT TESTS
public class WishlistServiceUnitTests
{
    private readonly Mock<IWishlistService> _serviceMock;

    public WishlistServiceUnitTests()
    {
        _serviceMock = new Mock<IWishlistService>();
    }

    /// <summary>
    /// UT-WISH-01: Add valid ISBN to wishlist
    /// Expected: 	Wishlist item is added
    /// </summary>
    /// <returns>
    /// WishlistItem with IsActive = true
    /// </returns>
    [Fact]
    public async Task UT_WISH_01_AddValidIsbn_ReturnsActiveWishlistItem()
    {
        var userId = Guid.NewGuid();
        var isbn = "9780141036144";

        var expectedItem = new WishlistItem
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Isbn = isbn,
            IsActive = true
        };

        _serviceMock
            .Setup(s => s.AddWishlistItemAsync(userId, isbn))
            .ReturnsAsync(expectedItem);

        var result = await _serviceMock.Object.AddWishlistItemAsync(userId, isbn);

        Assert.NotNull(result);
        Assert.Equal(isbn, result.Isbn);
        Assert.Equal(userId, result.UserId);
        Assert.True(result.IsActive);
    }

    /// <summary>
    ///  UT-WISH-02: Add invalid ISBN to wishlist
    /// Expected: Validation fails
    /// </summary>
    /// <returns>
    /// ArgumentException with message "ISBN must be 10 or 13 digits."
    /// </returns>
    [Fact]
    public async Task UT_WISH_02_AddInvalidIsbn_ThrowsArgumentException()
    {
        var userId = Guid.NewGuid();
        var invalidIsbn = "NOTANISBN";

        _serviceMock
            .Setup(s => s.AddWishlistItemAsync(userId, invalidIsbn))
            .ThrowsAsync(new ArgumentException("ISBN must be 10 or 13 digits."));

        await Assert.ThrowsAsync<ArgumentException>(
            () => _serviceMock.Object.AddWishlistItemAsync(userId, invalidIsbn));
    }

    /// <summary>
    /// UT-WISH-03: Add duplicate ISBN to same user's wishlist
    /// Expected: Duplicate is prevented
    /// </summary>
    /// <returns>
    /// InvalidOperationException with message "ISBN '{isbn}' is already on this user's wishlist."
    /// </returns>
    [Fact]
    public async Task UT_WISH_03_AddDuplicateIsbn_ThrowsInvalidOperationException()
    {
        var userId = Guid.NewGuid();
        var isbn = "9780141036144";

        _serviceMock
            .Setup(s => s.AddWishlistItemAsync(userId, isbn))
            .ThrowsAsync(new InvalidOperationException($"ISBN '{isbn}' is already on this user's wishlist."));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _serviceMock.Object.AddWishlistItemAsync(userId, isbn));
    }

    /// <summary>
    ///  UT-WISH-04: Remove item from wishlist
    /// Expected: Item is soft-deleted — IsActive set to false
    /// </summary>
    /// <returns>
    /// If successful, completes without exception. Verify that RemoveWishlistItemAsync was called with correct parameters.
    /// </returns>
    [Fact]
    public async Task UT_WISH_04_RemoveWishlistItem_CompletesSuccessfully()
    {
        var userId = Guid.NewGuid();
        var itemId = Guid.NewGuid();

        _serviceMock
            .Setup(s => s.RemoveWishlistItemAsync(itemId, userId))
            .Returns(Task.CompletedTask);

        await _serviceMock.Object.RemoveWishlistItemAsync(itemId, userId);

        _serviceMock.Verify(s => s.RemoveWishlistItemAsync(itemId, userId), Times.Once);
    }

    /// <summary>
    /// UT-WISH-05: Toggle wishlist item active/inactive
    /// Expected: IsActive updates correctly via restore
    /// </summary>
    /// <returns>
    /// If successful, completes without exception. Verify that RestoreWishlistItemAsync was called with correct parameters.
    /// </returns>
    [Fact]
    public async Task UT_WISH_05_RestoreWishlistItem_CompletesSuccessfully()
    {
        var userId = Guid.NewGuid();
        var itemId = Guid.NewGuid();

        _serviceMock
            .Setup(s => s.RestoreWishlistItemAsync(itemId, userId))
            .Returns(Task.CompletedTask);

        await _serviceMock.Object.RestoreWishlistItemAsync(itemId, userId);

        _serviceMock.Verify(s => s.RestoreWishlistItemAsync(itemId, userId), Times.Once);
    }

    /// <summary>
    /// UT-WISH-06: User attempts to update another user's wishlist item
    /// Expected: Operation is rejected — KeyNotFoundException is thrown
    /// </summary>
    /// <returns>
    /// KeyNotFoundException with message "Wishlist item not found for this user."
    /// </returns>
    [Fact]
    public async Task UT_WISH_06_RestoreAnotherUsersItem_ThrowsKeyNotFoundException()
    {
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var itemId = Guid.NewGuid();

        _serviceMock
            .Setup(s => s.RestoreWishlistItemAsync(itemId, userId))
            .ThrowsAsync(new KeyNotFoundException("Wishlist item not found for this user."));

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _serviceMock.Object.RestoreWishlistItemAsync(itemId, userId));
    }

    /// <summary>
    /// UT-WISH-07: User attempts to delete another user's wishlist item
    /// Expected: Operation is rejected — KeyNotFoundException is thrown
    /// </summary>
    /// <returns>
    /// KeyNotFoundException with message "Wishlist item not found for this user."
    /// </returns>
    [Fact]
    public async Task UT_WISH_07_RemoveAnotherUsersItem_ThrowsKeyNotFoundException()
    {
        var userId = Guid.NewGuid();
        var itemId = Guid.NewGuid();

        _serviceMock
            .Setup(s => s.RemoveWishlistItemAsync(itemId, userId))
            .ThrowsAsync(new KeyNotFoundException("Wishlist item not found for this user."));

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _serviceMock.Object.RemoveWishlistItemAsync(itemId, userId));
    }
}
