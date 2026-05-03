using Xunit;
using Microsoft.EntityFrameworkCore;
using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.ExchangeRequest;
using Book_Exchange.Services;
using Book_Exchange.Services.Interfaces;
using Book_Exchange.Data;

namespace Book_Exchange.Tests.Integration;

// Exchange Request Integration Tests
// Covers: IT-EXCH-01 through IT-EXCH-05

// TODO: Uncomment when ExchangeRequestService is implemented
// public class ExchangeRequestServiceIntegrationTests : IDisposable
// {
//     private readonly ApplicationDbContext _db;
//     private readonly IExchangeRequestService _service;
//
//     public ExchangeRequestServiceIntegrationTests()
//     {
//         var options = new DbContextOptionsBuilder<ApplicationDbContext>()
//             .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
//             .Options;
//
//         _db = new ApplicationDbContext(options);
//
//         _service = new ExchangeRequestService(_db);
//         // _service = null!;
//     }
//
//     public void Dispose()
//     {
//         _db.Dispose();
//     }
//
//     /// <summary>
//     /// IT-EXCH-01: User creates BuySell exchange request
//     /// Expected: ExchangeRequest is saved with Requested status
//     /// </summary>
//     [Fact]
//     public async Task IT_EXCH_01_CreateBuySellExchangeRequest_IsSavedWithRequestedStatus()
//     {
//         var owner = new ApplicationUser { Id = Guid.NewGuid(), UserName = "owner" };
//         var requester = new ApplicationUser { Id = Guid.NewGuid(), UserName = "requester" };
//
//         var targetListing = new Listing
//         {
//             Id = Guid.NewGuid(),
//             UserId = owner.Id,
//             Isbn = "9780141036144",
//             Condition = BookCondition.Good,
//             Price = 25.00m,
//             WeightGrams = 500,
//             CreatedAt = DateTime.UtcNow
//         };
//
//         _db.Users.AddRange(owner, requester);
//         _db.Listings.Add(targetListing);
//         await _db.SaveChangesAsync();
//
//         var dto = new CreateExchangeRequestDto
//         {
//             TargetListingId = targetListing.Id,
//             OfferedListingIds = new List<Guid>(),
//             CashAmount = 25.00m
//         };
//
//         var result = await _service.CreateExchangeRequestAsync(dto, requester.Id);
//
//         Assert.NotNull(result);
//         Assert.Equal(targetListing.Id, result.TargetListingId);
//         Assert.Equal(requester.Id, result.RequesterId);
//         Assert.Equal(ExchangeStatus.Requested, result.Status);
//         Assert.Equal(25.00m, result.Price);
//
//         var saved = await _db.ExchangeRequests.FirstOrDefaultAsync(e => e.Id == result.Id);
//         Assert.NotNull(saved);
//         Assert.Equal(ExchangeStatus.Requested, saved!.Status);
//     }
//
//     /// <summary>
//     /// IT-EXCH-02: User creates BookSwap exchange request
//     /// Expected: ExchangeRequest and ExchangeRequestItems are saved
//     /// </summary>
//     [Fact]
//     public async Task IT_EXCH_02_CreateBookSwapExchangeRequest_RequestAndItemsAreSaved()
//     {
//         var owner = new ApplicationUser { Id = Guid.NewGuid(), UserName = "owner2" };
//         var requester = new ApplicationUser { Id = Guid.NewGuid(), UserName = "requester2" };
//
//         var targetListing = new Listing
//         {
//             Id = Guid.NewGuid(),
//             UserId = owner.Id,
//             Isbn = "9780141036144",
//             Condition = BookCondition.Good,
//             Price = 25.00m,
//             WeightGrams = 500,
//             CreatedAt = DateTime.UtcNow
//         };
//
//         var offeredListing = new Listing
//         {
//             Id = Guid.NewGuid(),
//             UserId = requester.Id,
//             Isbn = "9780439023528",
//             Condition = BookCondition.VeryGood,
//             Price = 15.00m,
//             WeightGrams = 450,
//             CreatedAt = DateTime.UtcNow
//         };
//
//         _db.Users.AddRange(owner, requester);
//         _db.Listings.AddRange(targetListing, offeredListing);
//         await _db.SaveChangesAsync();
//
//         var dto = new CreateExchangeRequestDto
//         {
//             TargetListingId = targetListing.Id,
//             OfferedListingIds = new List<Guid> { offeredListing.Id },
//             CashAmount = null
//         };
//
//         var result = await _service.CreateExchangeRequestAsync(dto, requester.Id);
//
//         Assert.NotNull(result);
//         Assert.Equal(ExchangeStatus.Requested, result.Status);
//         Assert.Null(result.Price);
//
//         var saved = await _db.ExchangeRequests
//             .Include(e => e.ExchangeRequestItems)
//             .FirstOrDefaultAsync(e => e.Id == result.Id);
//
//         Assert.NotNull(saved);
//         Assert.Single(saved!.ExchangeRequestItems);
//         Assert.Contains(saved.ExchangeRequestItems, i => i.OfferedListingId == offeredListing.Id);
//     }
//
//     /// <summary>
//     /// IT-EXCH-03: User creates BookSwapWithCash exchange request
//     /// Expected: Request includes offered listing(s) and Price
//     /// </summary>
//     [Fact]
//     public async Task IT_EXCH_03_CreateBookSwapWithCashExchangeRequest_IncludesItemsAndPrice()
//     {
//         var owner = new ApplicationUser { Id = Guid.NewGuid(), UserName = "owner3" };
//         var requester = new ApplicationUser { Id = Guid.NewGuid(), UserName = "requester3" };
//
//         var targetListing = new Listing
//         {
//             Id = Guid.NewGuid(),
//             UserId = owner.Id,
//             Isbn = "9780141036144",
//             Condition = BookCondition.Good,
//             Price = 30.00m,
//             WeightGrams = 500,
//             CreatedAt = DateTime.UtcNow
//         };
//
//         var offeredListing = new Listing
//         {
//             Id = Guid.NewGuid(),
//             UserId = requester.Id,
//             Isbn = "9780439023528",
//             Condition = BookCondition.Acceptable,
//             Price = 10.00m,
//             WeightGrams = 400,
//             CreatedAt = DateTime.UtcNow
//         };
//
//         _db.Users.AddRange(owner, requester);
//         _db.Listings.AddRange(targetListing, offeredListing);
//         await _db.SaveChangesAsync();
//
//         var dto = new CreateExchangeRequestDto
//         {
//             TargetListingId = targetListing.Id,
//             OfferedListingIds = new List<Guid> { offeredListing.Id },
//             CashAmount = 12.50m
//         };
//
//         var result = await _service.CreateExchangeRequestAsync(dto, requester.Id);
//
//         Assert.NotNull(result);
//         Assert.Equal(ExchangeStatus.Requested, result.Status);
//         Assert.Equal(12.50m, result.Price);
//
//         var saved = await _db.ExchangeRequests
//             .Include(e => e.ExchangeRequestItems)
//             .FirstOrDefaultAsync(e => e.Id == result.Id);
//
//         Assert.NotNull(saved);
//         Assert.Equal(12.50m, saved!.Price);
//         Assert.Single(saved.ExchangeRequestItems);
//         Assert.Contains(saved.ExchangeRequestItems, i => i.OfferedListingId == offeredListing.Id);
//     }
//
//     /// <summary>
//     /// IT-EXCH-04: Listing owner accepts exchange request
//     /// Expected: ExchangeRequest status becomes Accepted and Transaction is created
//     /// </summary>
//     [Fact]
//     public async Task IT_EXCH_04_ListingOwnerAcceptsExchangeRequest_StatusAcceptedAndTransactionCreated()
//     {
//         var owner = new ApplicationUser { Id = Guid.NewGuid(), UserName = "owner4" };
//         var requester = new ApplicationUser { Id = Guid.NewGuid(), UserName = "requester4" };
//
//         var targetListing = new Listing
//         {
//             Id = Guid.NewGuid(),
//             UserId = owner.Id,
//             Isbn = "9780141036144",
//             Condition = BookCondition.Good,
//             Price = 25.00m,
//             WeightGrams = 500,
//             CreatedAt = DateTime.UtcNow
//         };
//
//         var exchangeRequest = new ExchangeRequest
//         {
//             Id = Guid.NewGuid(),
//             TargetListingId = targetListing.Id,
//             RequesterId = requester.Id,
//             Status = ExchangeStatus.Requested,
//             Price = 25.00m,
//             CreatedAt = DateTime.UtcNow
//         };
//
//         _db.Users.AddRange(owner, requester);
//         _db.Listings.Add(targetListing);
//         _db.ExchangeRequests.Add(exchangeRequest);
//         await _db.SaveChangesAsync();
//
//         await _service.AcceptExchangeRequestAsync(exchangeRequest.Id, owner.Id);
//
//         var savedRequest = await _db.ExchangeRequests.FindAsync(exchangeRequest.Id);
//         var savedTransaction = await _db.Transactions
//             .FirstOrDefaultAsync(t => t.ExchangeRequestId == exchangeRequest.Id);
//
//         Assert.NotNull(savedRequest);
//         Assert.Equal(ExchangeStatus.Accepted, savedRequest!.Status);
//         Assert.NotNull(savedRequest.AcceptedAt);
//         Assert.NotNull(savedTransaction);
//     }
//
//     /// <summary>
//     /// IT-EXCH-05: Listing owner rejects exchange request
//     /// Expected: ExchangeRequest status becomes Rejected and no Transaction is created
//     /// </summary>
//     [Fact]
//     public async Task IT_EXCH_05_ListingOwnerRejectsExchangeRequest_StatusRejectedAndNoTransactionCreated()
//     {
//         var owner = new ApplicationUser { Id = Guid.NewGuid(), UserName = "owner5" };
//         var requester = new ApplicationUser { Id = Guid.NewGuid(), UserName = "requester5" };
//
//         var targetListing = new Listing
//         {
//             Id = Guid.NewGuid(),
//             UserId = owner.Id,
//             Isbn = "9780141036144",
//             Condition = BookCondition.Good,
//             Price = 25.00m,
//             WeightGrams = 500,
//             CreatedAt = DateTime.UtcNow
//         };
//
//         var exchangeRequest = new ExchangeRequest
//         {
//             Id = Guid.NewGuid(),
//             TargetListingId = targetListing.Id,
//             RequesterId = requester.Id,
//             Status = ExchangeStatus.Requested,
//             Price = 25.00m,
//             CreatedAt = DateTime.UtcNow
//         };
//
//         _db.Users.AddRange(owner, requester);
//         _db.Listings.Add(targetListing);
//         _db.ExchangeRequests.Add(exchangeRequest);
//         await _db.SaveChangesAsync();
//
//         await _service.RejectExchangeRequestAsync(exchangeRequest.Id, owner.Id);
//
//         var savedRequest = await _db.ExchangeRequests.FindAsync(exchangeRequest.Id);
//         var transactionExists = await _db.Transactions
//             .AnyAsync(t => t.ExchangeRequestId == exchangeRequest.Id);
//
//         Assert.NotNull(savedRequest);
//         Assert.Equal(ExchangeStatus.Rejected, savedRequest!.Status);
//         Assert.False(transactionExists);
//     }
// }