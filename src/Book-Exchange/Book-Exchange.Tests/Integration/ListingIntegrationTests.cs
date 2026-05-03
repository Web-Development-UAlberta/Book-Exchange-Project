using Xunit;
using Microsoft.EntityFrameworkCore;
using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Listing;
using Book_Exchange.Services;
using Book_Exchange.Services.Interfaces;
using Book_Exchange.Data;

namespace Book_Exchange.Tests.Integration;

// Listing Integration Tests
// Covers: IT-LIST-01 through IT-LIST-05

// TODO: Uncomment when ListingService is implemented
// public class ListingServiceIntegrationTests : IDisposable
// {
//     private readonly ApplicationDbContext _db;
//     private readonly IListingService _service;
//
//     public ListingServiceIntegrationTests()
//     {
//         var options = new DbContextOptionsBuilder<ApplicationDbContext>()
//             .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
//             .Options;
//
//         _db = new ApplicationDbContext(options);
//
//         _service = new ListingService(_db);
//         // _service = null!;
//     }
//
//     public void Dispose()
//     {
//         _db.Dispose();
//     }
//
//     /// <summary>
//     /// IT-LIST-01: Authenticated user creates a listing
//     /// Expected: Listing appears in My Listings and database
//     /// </summary>
//     [Fact]
//     public async Task IT_LIST_01_AuthenticatedUserCreatesListing_ListingIsSavedAndReturned()
//     {
//         var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "testuser" };
//         _db.Users.Add(user);
//         await _db.SaveChangesAsync();
//
//         var dto = new CreateListingDto
//         {
//             Isbn = "9780141036144",
//             Condition = BookCondition.Good,
//             Price = 20.00m,
//             WeightGrams = 500
//         };
//
//         var result = await _service.CreateListingAsync(dto, user.Id);
//
//         Assert.NotNull(result);
//         Assert.Equal(user.Id, result.UserId);
//         Assert.Equal(dto.Isbn, result.Isbn);
//         Assert.Equal(dto.Condition, result.Condition);
//         Assert.Equal(dto.Price, result.Price);
//         Assert.Equal(dto.WeightGrams, result.WeightGrams);
//
//         var saved = await _db.Listings.FirstOrDefaultAsync(l => l.Id == result.Id);
//         Assert.NotNull(saved);
//
//         var myListings = await _service.GetListingsByUserIdAsync(user.Id);
//         Assert.Contains(myListings, l => l.Id == result.Id);
//     }
//
//     /// <summary>
//     /// IT-LIST-02: User creates listing with invalid ISBN
//     /// Expected: Validation error is shown and database is unchanged
//     /// </summary>
//     [Fact]
//     public async Task IT_LIST_02_CreateListingWithInvalidIsbn_IsBlockedAndDatabaseUnchanged()
//     {
//         var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "testuser2" };
//         _db.Users.Add(user);
//         await _db.SaveChangesAsync();
//
//         var dto = new CreateListingDto
//         {
//             Isbn = "BADISBN",
//             Condition = BookCondition.Good,
//             Price = 20.00m,
//             WeightGrams = 500
//         };
//
//         await Assert.ThrowsAsync<ArgumentException>(
//             () => _service.CreateListingAsync(dto, user.Id));
//
//         Assert.False(await _db.Listings.AnyAsync());
//     }
//
//     /// <summary>
//     /// IT-LIST-03: Edit existing listing
//     /// Expected: Updated values are saved and displayed
//     /// </summary>
//     [Fact]
//     public async Task IT_LIST_03_EditExistingListing_UpdatedValuesAreSaved()
//     {
//         var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "testuser3" };
//
//         var listing = new Listing
//         {
//             Id = Guid.NewGuid(),
//             UserId = user.Id,
//             Isbn = "9780141036144",
//             Condition = BookCondition.Good,
//             Price = 20.00m,
//             WeightGrams = 500,
//             CreatedAt = DateTime.UtcNow
//         };
//
//         _db.Users.Add(user);
//         _db.Listings.Add(listing);
//         await _db.SaveChangesAsync();
//
//         var dto = new UpdateListingDto
//         {
//             Condition = BookCondition.VeryGood,
//             Price = 30.00m,
//             WeightGrams = 700
//         };
//
//         await _service.UpdateListingAsync(listing.Id, dto, user.Id);
//
//         var saved = await _db.Listings.FindAsync(listing.Id);
//
//         Assert.NotNull(saved);
//         Assert.Equal(BookCondition.VeryGood, saved!.Condition);
//         Assert.Equal(30.00m, saved.Price);
//         Assert.Equal(700, saved.WeightGrams);
//     }
//
//     /// <summary>
//     /// IT-LIST-04: Delete listing
//     /// Expected: Listing is removed or marked unavailable
//     /// </summary>
//     [Fact]
//     public async Task IT_LIST_04_DeleteListing_ListingIsRemoved()
//     {
//         var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "testuser4" };
//
//         var listing = new Listing
//         {
//             Id = Guid.NewGuid(),
//             UserId = user.Id,
//             Isbn = "9780141036144",
//             Condition = BookCondition.Good,
//             Price = 20.00m,
//             WeightGrams = 500,
//             CreatedAt = DateTime.UtcNow
//         };
//
//         _db.Users.Add(user);
//         _db.Listings.Add(listing);
//         await _db.SaveChangesAsync();
//
//         await _service.DeleteListingAsync(listing.Id, user.Id);
//
//         var saved = await _db.Listings.FindAsync(listing.Id);
//
//         Assert.Null(saved);
//     }
//
//     /// <summary>
//     /// IT-LIST-05: User tries to edit another user's listing
//     /// Expected: Access is denied or operation is blocked
//     /// </summary>
//     [Fact]
//     public async Task IT_LIST_05_EditAnotherUsersListing_IsBlocked()
//     {
//         var owner = new ApplicationUser { Id = Guid.NewGuid(), UserName = "owner" };
//         var other = new ApplicationUser { Id = Guid.NewGuid(), UserName = "other" };
//
//         var listing = new Listing
//         {
//             Id = Guid.NewGuid(),
//             UserId = owner.Id,
//             Isbn = "9780141036144",
//             Condition = BookCondition.Good,
//             Price = 20.00m,
//             WeightGrams = 500,
//             CreatedAt = DateTime.UtcNow
//         };
//
//         _db.Users.AddRange(owner, other);
//         _db.Listings.Add(listing);
//         await _db.SaveChangesAsync();
//
//         var dto = new UpdateListingDto
//         {
//             Condition = BookCondition.Acceptable,
//             Price = 10.00m,
//             WeightGrams = 400
//         };
//
//         await Assert.ThrowsAsync<UnauthorizedAccessException>(
//             () => _service.UpdateListingAsync(listing.Id, dto, other.Id));
//
//         var saved = await _db.Listings.FindAsync(listing.Id);
//
//         Assert.NotNull(saved);
//         Assert.Equal(BookCondition.Good, saved!.Condition);
//         Assert.Equal(20.00m, saved.Price);
//         Assert.Equal(500, saved.WeightGrams);
//     }
// }