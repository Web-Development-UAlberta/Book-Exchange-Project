using Xunit;
using Microsoft.EntityFrameworkCore;
using Book_Exchange.Models;
using Book_Exchange.Services.Interfaces;
using Book_Exchange.Data;

// INTEGRATION TESTS
// Covers: IT-TRANS-01 through IT-TRANS-03 (Integration Tests)

namespace Book_Exchange.Tests.Integration;


// TODO: Uncomment when TransactionService is implemented
// public class TransactionServiceIntegrationTests : IDisposable
// {
//     private readonly ApplicationDbContext _db;
//     private readonly ITransactionService _service;

//     public TransactionServiceIntegrationTests()
//     {
//         var options = new DbContextOptionsBuilder<ApplicationDbContext>()
//             .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
//             .Options;

//         _db = new ApplicationDbContext(options);
//         _service = new TransactionService(_db);
//     }

//     public void Dispose() => _db.Dispose();

//     // Helper: seed a Listing and ExchangeRequest with the given status
//     private async Task<ExchangeRequest> SeedExchangeRequestAsync(ExchangeStatus status)
//     {
//         var listing = new Listing
//         {
//             Id = Guid.NewGuid(),
//             UserId = Guid.NewGuid(),
//             Isbn = "9780593311615",
//             Condition = BookCondition.Good,
//             Price = 10.00m,
//             WeightGrams = 300,
//             Status = ListingStatus.Active
//         };

//         var request = new ExchangeRequest
//         {
//             Id = Guid.NewGuid(),
//             RequesterId = Guid.NewGuid(),
//             TargetListingId = listing.Id,
//             Type = ExchangeType.BuySell,
//             Status = status,
//             CreatedAt = DateTime.UtcNow
//         };

//         _db.Listings.Add(listing);
//         _db.ExchangeRequests.Add(request);
//         await _db.SaveChangesAsync();

//         return request;
//     }

//     /// <summary>
//     /// IT-TRANS-01: Accepted exchange creates transaction
//     /// Expected: Transaction is linked to ExchangeRequest
//     /// </summary>
//     /// <returns>
//     /// Transaction linked to the ExchangeRequest
//     /// </returns>
//     [Fact]
//     public async Task IT_TRANS_01_AcceptedExchange_CreatesTransactionLinkedToRequest()
//     {
//         var acceptedRequest = await SeedExchangeRequestAsync(ExchangeStatus.Accepted);

//         var transaction = await _service.CreateTransactionFromExchangeRequestAsync(acceptedRequest);

//         Assert.NotNull(transaction);
//         Assert.Equal(acceptedRequest.Id, transaction.ExchangeRequestId);
//         Assert.Equal(TransactionStatus.Confirmed, transaction.Status);

//         var persisted = await _db.Transactions.FindAsync(transaction.Id);
//         Assert.NotNull(persisted);
//         Assert.Equal(acceptedRequest.Id, persisted.ExchangeRequestId);
//     }

//     /// <summary>
//     /// IT-TRANS-02: Completing transaction updates listing status
//     /// Expected: Related listing(s) are no longer active
//     /// </summary>
//     /// <returns>
//     /// If successful, completes without exception. Verify that CompleteTransactionAsync was called with correct parameters.
//     /// </returns>
//     [Fact]
//     public async Task IT_TRANS_02_CompletingTransaction_DeactivatesRelatedListings()
//     {
//         var acceptedRequest = await SeedExchangeRequestAsync(ExchangeStatus.Accepted);
//         var transaction = await _service.CreateTransactionFromExchangeRequestAsync(acceptedRequest);
//         var userId = Guid.NewGuid();

//         await _service.CompleteTransactionAsync(transaction.Id, userId);

//         var completed = await _db.Transactions.FindAsync(transaction.Id);
//         Assert.NotNull(completed);
//         Assert.Equal(TransactionStatus.Completed, completed.Status);
//         Assert.NotNull(completed.CompletedAt);

//         var listing = await _db.Listings.FindAsync(acceptedRequest.TargetListingId);
//         Assert.NotNull(listing);
//         Assert.NotEqual(ListingStatus.Active, listing.Status);
//     }

//     /// <summary>
//     /// IT-TRANS-03: Transaction history is requested
//     /// Expected: User sees only their relevant transactions
//     /// </summary>
//     /// <returns>
//     /// List of transactions belonging to the specified user
//     /// </returns>
//     [Fact]
//     public async Task IT_TRANS_03_GetTransactionHistory_ReturnsOnlyUsersTransactions()
//     {
//         var userId = Guid.NewGuid();

//         // Seed two transactions belonging to this user
//         for (int i = 0; i < 2; i++)
//         {
//             var listing = new Listing
//             {
//                 Id = Guid.NewGuid(),
//                 UserId = Guid.NewGuid(),
//                 Isbn = "9780593311615",
//                 Condition = BookCondition.Good,
//                 Price = 10.00m,
//                 WeightGrams = 300,
//                 Status = ListingStatus.Active
//             };

//             var request = new ExchangeRequest
//             {
//                 Id = Guid.NewGuid(),
//                 RequesterId = userId,
//                 TargetListingId = listing.Id,
//                 Type = ExchangeType.BuySell,
//                 Status = ExchangeStatus.Accepted,
//                 CreatedAt = DateTime.UtcNow
//             };

//             var tx = new Transaction
//             {
//                 Id = Guid.NewGuid(),
//                 ExchangeRequestId = request.Id,
//                 ExchangeRequest = request,
//                 Status = TransactionStatus.Confirmed,
//                 CreatedAt = DateTime.UtcNow
//             };

//             _db.Listings.Add(listing);
//             _db.ExchangeRequests.Add(request);
//             _db.Transactions.Add(tx);
//         }

//         // Seed one unrelated transaction for a different user
//         _db.Transactions.Add(new Transaction
//         {
//             Id = Guid.NewGuid(),
//             ExchangeRequestId = Guid.NewGuid(),
//             Status = TransactionStatus.Confirmed,
//             CreatedAt = DateTime.UtcNow
//         });

//         await _db.SaveChangesAsync();

//         var results = await _service.GetTransactionsByUserIdAsync(userId);

//         Assert.NotNull(results);
//         Assert.Equal(2, results.Count());
//     }
// }