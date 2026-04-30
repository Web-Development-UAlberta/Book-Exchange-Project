using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Book_Exchange.Models;
using Book_Exchange.Services;
using Book_Exchange.Services.Interfaces;
using Book_Exchange.Data;

// Wishlist Integration Tests
// Covers: IT-WISH-01 through IT-WISH-04 (Integration Tests)
//         IT-MATCH-01 through IT-MATCH-03 (Matching Integration Tests)

namespace Book_Exchange.Tests.Integration;

// TODO: Uncomment when WishlistService is implemented
// public class WishlistServiceIntegrationTests : IDisposable
// {
//     private readonly ApplicationDbContext _db;
//     private readonly IWishlistService _service;

//     public WishlistServiceIntegrationTests()
//     {
//         var options = new DbContextOptionsBuilder<ApplicationDbContext>()
//             .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
//             .Options;

//         _db = new ApplicationDbContext(options);

//         _service = new WishlistService(_db);
//         _service = null!;
//     }

//     public void Dispose()
//     {
//         _db.Dispose();
//     }

//     /// <summary>
//     /// IT-WISH-01: User adds ISBN to wishlist
//     /// Expected: Wishlist item is saved and shown
//     /// </summary>
//     /// <returns>
//     /// The newly added wishlist item.
//     /// </returns>
//     [Fact]
//     public async Task IT_WISH_01_AddIsbnToWishlist_ItemIsSavedAndReturned()
//     {
//         var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "testuser" };
//         _db.Users.Add(user);
//         await _db.SaveChangesAsync();

//         var result = await _service.AddWishlistItemAsync(user.Id, "9780141036144");

//         Assert.NotNull(result);
//         Assert.Equal("9780141036144", result.Isbn);
//         Assert.True(result.IsActive);

//         var saved = await _db.WishlistItems.FirstOrDefaultAsync(w => w.Id == result.Id);
//         Assert.NotNull(saved);
//     }

//     /// <summary>
//     /// IT-WISH-02: User adds duplicate ISBN
//     /// Expected: Duplicate is blocked
//     /// </summary>
//     /// <returns>
//     /// InvalidOperationException with message "Wishlist item already exists."
//     /// </returns>
//     [Fact]
//     public async Task IT_WISH_02_AddDuplicateIsbn_IsBlocked()
//     {
//         var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "testuser2" };
//         _db.Users.Add(user);
//         _db.WishlistItems.Add(new WishlistItem
//         {
//             Id = Guid.NewGuid(),
//             UserId = user.Id,
//             Isbn = "9780141036144",
//             IsActive = true
//         });
//         await _db.SaveChangesAsync();

//         await Assert.ThrowsAsync<InvalidOperationException>(
//             () => _service.AddWishlistItemAsync(user.Id, "9780141036144"));
//     }

//     /// <summary>
//     /// IT-WISH-03: Remove wishlist item
//     /// Expected: Item no longer appears (IsActive = false)
//     /// </summary>
//     /// <returns>
//     /// The deactivated wishlist item.
//     /// </returns>
//     [Fact]
//     public async Task IT_WISH_03_RemoveWishlistItem_ItemIsDeactivated()
//     {
//         var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "testuser3" };
//         var item = new WishlistItem
//         {
//             Id = Guid.NewGuid(),
//             UserId = user.Id,
//             Isbn = "9780141036144",
//             IsActive = true
//         };

//         _db.Users.Add(user);
//         _db.WishlistItems.Add(item);
//         await _db.SaveChangesAsync();

//         await _service.RemoveWishlistItemAsync(item.Id, user.Id);

//         var saved = await _db.WishlistItems.FindAsync(item.Id);
//         Assert.NotNull(saved);
//         Assert.False(saved!.IsActive);
//     }

//     /// <summary>
//     /// IT-WISH-04: User tries to update another user's wishlist item
//     /// Expected: Access is denied or operation is blocked
//     /// </summary>
//     /// <returns>
//     /// The exception thrown when attempting to remove another user's wishlist item.
//     /// </returns>
//     [Fact]
//     public async Task IT_WISH_04_RemoveAnotherUsersItem_IsBlocked()
//     {
//         var owner = new ApplicationUser { Id = Guid.NewGuid(), UserName = "owner" };
//         var other = new ApplicationUser { Id = Guid.NewGuid(), UserName = "other" };
//         var item = new WishlistItem
//         {
//             Id = Guid.NewGuid(),
//             UserId = owner.Id,
//             Isbn = "9780141036144",
//             IsActive = true
//         };

//         _db.Users.AddRange(owner, other);
//         _db.WishlistItems.Add(item);
//         await _db.SaveChangesAsync();

//         await Assert.ThrowsAsync<KeyNotFoundException>(
//             () => _service.RemoveWishlistItemAsync(item.Id, other.Id));
//     }

//     /// <summary>
//     /// IT-MATCH-01: User wishlist matches another user's active listing
//     /// Expected: Match suggestion appears
//     /// </summary>
//     /// <returns>
//     /// The list of matching listings for the user's wishlist.
//     /// </returns>
//     [Fact]
//     public async Task IT_MATCH_01_WishlistMatchesActiveListing_MatchIsReturned()
//     {
//         var wisher = new ApplicationUser { Id = Guid.NewGuid(), UserName = "wisher" };
//         var lister = new ApplicationUser { Id = Guid.NewGuid(), UserName = "lister" };
//         var isbn = "9780141036144";

//         var wishlistItem = new WishlistItem
//         {
//             Id = Guid.NewGuid(),
//             UserId = wisher.Id,
//             Isbn = isbn,
//             IsActive = true
//         };

//         var listing = new Listing
//         {
//             Id = Guid.NewGuid(),
//             UserId = lister.Id,
//             Isbn = isbn,
//             Status = ListingStatus.Active,
//             Price = 10.00m,
//             WeightGrams = 300,
//             Condition = BookCondition.Good,
//             CreatedAt = DateTime.UtcNow
//         };

//         _db.Users.AddRange(wisher, lister);
//         _db.WishlistItems.Add(wishlistItem);
//         _db.Listings.Add(listing);
//         await _db.SaveChangesAsync();

//         var matches = await _service.GetMatchingListingsAsync(wisher.Id);

//         Assert.NotNull(matches);
//         Assert.Single(matches);
//         Assert.Equal(isbn, matches.First().Isbn);
//     }

//     /// <summary>
//     ///  IT-MATCH-02: Matching listing belongs to same user
//     /// Expected: Match is excluded
//     /// </summary>
//     /// <returns>
//     /// An empty list of matches, since users should not see their own listings as matches to their wishlist items.
//     /// </returns>
//     [Fact]
//     public async Task IT_MATCH_02_OwnListingMatchesWishlist_IsExcluded()
//     {
//         var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "sameuser" };
//         var isbn = "9780141036144";

//         var wishlistItem = new WishlistItem
//         {
//             Id = Guid.NewGuid(),
//             UserId = user.Id,
//             Isbn = isbn,
//             IsActive = true
//         };

//         var listing = new Listing
//         {
//             Id = Guid.NewGuid(),
//             UserId = user.Id, // same user
//             Isbn = isbn,
//             Status = ListingStatus.Active,
//             Price = 10.00m,
//             WeightGrams = 300,
//             Condition = BookCondition.Good,
//             CreatedAt = DateTime.UtcNow
//         };

//         _db.Users.Add(user);
//         _db.WishlistItems.Add(wishlistItem);
//         _db.Listings.Add(listing);
//         await _db.SaveChangesAsync();

//         var matches = await _service.GetMatchingListingsAsync(user.Id);

//         Assert.NotNull(matches);
//         Assert.Empty(matches);
//     }

//     /// <summary>
//     /// IT-MATCH-03: Listing status is Completed
//     /// Expected: Match is excluded
//     /// </summary>
//     /// <returns>
//     /// An empty list of matches, since completed listings should not be considered as matches to wishlist items.
//     /// </returns>
//     [Fact]
//     public async Task IT_MATCH_03_CompletedListingMatchesWishlist_IsExcluded()
//     {
//         var wisher = new ApplicationUser { Id = Guid.NewGuid(), UserName = "wisher2" };
//         var lister = new ApplicationUser { Id = Guid.NewGuid(), UserName = "lister2" };
//         var isbn = "9780141036144";

//         var wishlistItem = new WishlistItem
//         {
//             Id = Guid.NewGuid(),
//             UserId = wisher.Id,
//             Isbn = isbn,
//             IsActive = true
//         };

//         var listing = new Listing
//         {
//             Id = Guid.NewGuid(),
//             UserId = lister.Id,
//             Isbn = isbn,
//             Status = ListingStatus.Completed, // not active
//             Price = 10.00m,
//             WeightGrams = 300,
//             Condition = BookCondition.Good,
//             CreatedAt = DateTime.UtcNow
//         };

//         _db.Users.AddRange(wisher, lister);
//         _db.WishlistItems.Add(wishlistItem);
//         _db.Listings.Add(listing);
//         await _db.SaveChangesAsync();

//         var matches = await _service.GetMatchingListingsAsync(wisher.Id);

//         Assert.NotNull(matches);
//         Assert.Empty(matches);
//     }
// }

