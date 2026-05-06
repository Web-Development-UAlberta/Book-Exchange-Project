using Xunit;
using Microsoft.EntityFrameworkCore;
using Book_Exchange.Models;
using Book_Exchange.Services;
using Book_Exchange.Services.Interfaces;
using Book_Exchange.Data;

namespace Book_Exchange.Tests.Integration;

// Wishlist Integration Tests
// Covers: IT-WISH-01 through IT-WISH-04
//         IT-MATCH-01 through IT-MATCH-03

public class WishlistServiceIntegrationTests : IDisposable
{
    private readonly ApplicationDbContext _db;
    private readonly IWishlistService _service;

    public WishlistServiceIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _db = new ApplicationDbContext(options);
        _service = new WishlistService(_db);
    }

    public void Dispose()
    {
        _db.Dispose();
    }

    /// <summary>
    /// IT-WISH-01: User adds ISBN to wishlist
    /// Expected: Wishlist item is saved and shown
    /// </summary>
    [Fact]
    public async Task IT_WISH_01_AddIsbnToWishlist_ItemIsSavedAndReturned()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "testuser" };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var result = await _service.AddWishlistItemAsync(user.Id, "9780141036144");

        Assert.NotNull(result);
        Assert.Equal("9780141036144", result.Isbn);
        Assert.True(result.IsActive);

        var saved = await _db.Wishlist.FirstOrDefaultAsync(w => w.Id == result.Id);
        Assert.NotNull(saved);
    }

    /// <summary>
    /// IT-WISH-02: User adds duplicate ISBN
    /// Expected: Existing wishlist item is returned and remains active
    /// </summary>
    [Fact]
    public async Task IT_WISH_02_AddDuplicateIsbn_ReturnsExistingActiveItem()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "testuser2" };
        var isbn = "9780141036144";

        var existingItem = new WishlistItem
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Isbn = isbn,
            IsActive = true
        };

        _db.Users.Add(user);
        _db.Wishlist.Add(existingItem);
        await _db.SaveChangesAsync();

        var result = await _service.AddWishlistItemAsync(user.Id, isbn);

        Assert.NotNull(result);
        Assert.Equal(existingItem.Id, result.Id);
        Assert.Equal(isbn, result.Isbn);
        Assert.True(result.IsActive);

        var count = await _db.Wishlist.CountAsync(w => w.UserId == user.Id && w.Isbn == isbn);
        Assert.Equal(1, count);
    }

    /// <summary>
    /// IT-WISH-03: Remove wishlist item
    /// Expected: Item no longer appears as active
    /// </summary>
    [Fact]
    public async Task IT_WISH_03_RemoveWishlistItem_ItemIsDeactivated()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "testuser3" };
        var item = new WishlistItem
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Isbn = "9780141036144",
            IsActive = true
        };

        _db.Users.Add(user);
        _db.Wishlist.Add(item);
        await _db.SaveChangesAsync();

        await _service.RemoveWishlistItemAsync(item.Id, user.Id);

        var saved = await _db.Wishlist.FindAsync(item.Id);
        Assert.NotNull(saved);
        Assert.False(saved!.IsActive);
    }

    /// <summary>
    /// IT-WISH-04: User tries to remove another user's wishlist item
    /// Expected: Access is denied or operation is blocked
    /// </summary>
    [Fact]
    public async Task IT_WISH_04_RemoveAnotherUsersItem_IsBlocked()
    {
        var owner = new ApplicationUser { Id = Guid.NewGuid(), UserName = "owner" };
        var other = new ApplicationUser { Id = Guid.NewGuid(), UserName = "other" };

        var item = new WishlistItem
        {
            Id = Guid.NewGuid(),
            UserId = owner.Id,
            Isbn = "9780141036144",
            IsActive = true
        };

        _db.Users.AddRange(owner, other);
        _db.Wishlist.Add(item);
        await _db.SaveChangesAsync();

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.RemoveWishlistItemAsync(item.Id, other.Id));
    }

    /// <summary>
    /// IT-MATCH-01: User wishlist matches another user's listing
    /// Expected: Match suggestion appears
    /// </summary>
    [Fact]
    public async Task IT_MATCH_01_WishlistMatchesActiveListing_MatchIsReturned()
    {
        var wisher = new ApplicationUser { Id = Guid.NewGuid(), UserName = "wisher" };
        var lister = new ApplicationUser { Id = Guid.NewGuid(), UserName = "lister" };
        var isbn = "9780141036144";

        var wishlistItem = new WishlistItem
        {
            Id = Guid.NewGuid(),
            UserId = wisher.Id,
            Isbn = isbn,
            IsActive = true
        };

        var listing = new Listing
        {
            Id = Guid.NewGuid(),
            UserId = lister.Id,
            Isbn = isbn,
            Price = 10.00m,
            WeightGrams = 300,
            Condition = BookCondition.Good,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.AddRange(wisher, lister);
        _db.Wishlist.Add(wishlistItem);
        _db.Listings.Add(listing);
        await _db.SaveChangesAsync();

        var matches = await _service.GetMatchingListingsAsync(wisher.Id);

        Assert.NotNull(matches);
        Assert.Single(matches);
        Assert.Equal(isbn, matches.First().Isbn);
    }

    /// <summary>
    /// IT-MATCH-02: Matching listing belongs to same user
    /// Expected: Match is excluded
    /// </summary>
    [Fact]
    public async Task IT_MATCH_02_OwnListingMatchesWishlist_IsExcluded()
    {
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "sameuser" };
        var isbn = "9780141036144";

        var wishlistItem = new WishlistItem
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Isbn = isbn,
            IsActive = true
        };

        var listing = new Listing
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Isbn = isbn,
            Price = 10.00m,
            WeightGrams = 300,
            Condition = BookCondition.Good,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        _db.Wishlist.Add(wishlistItem);
        _db.Listings.Add(listing);
        await _db.SaveChangesAsync();

        var matches = await _service.GetMatchingListingsAsync(user.Id);

        Assert.NotNull(matches);
        Assert.Empty(matches);
    }

    /// <summary>
    /// IT-MATCH-03: Inactive wishlist item matches a listing
    /// Expected: Match is excluded
    /// </summary>
    [Fact]
    public async Task IT_MATCH_03_InactiveWishlistItem_IsExcluded()
    {
        var wisher = new ApplicationUser { Id = Guid.NewGuid(), UserName = "wisher2" };
        var lister = new ApplicationUser { Id = Guid.NewGuid(), UserName = "lister2" };
        var isbn = "9780141036144";

        var wishlistItem = new WishlistItem
        {
            Id = Guid.NewGuid(),
            UserId = wisher.Id,
            Isbn = isbn,
            IsActive = false
        };

        var listing = new Listing
        {
            Id = Guid.NewGuid(),
            UserId = lister.Id,
            Isbn = isbn,
            Price = 10.00m,
            WeightGrams = 300,
            Condition = BookCondition.Good,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.AddRange(wisher, lister);
        _db.Wishlist.Add(wishlistItem);
        _db.Listings.Add(listing);
        await _db.SaveChangesAsync();

        var matches = await _service.GetMatchingListingsAsync(wisher.Id);

        Assert.NotNull(matches);
        Assert.Empty(matches);
    }
}