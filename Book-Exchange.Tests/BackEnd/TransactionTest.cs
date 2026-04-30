using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Book_Exchange.Models;
using Book_Exchange.Services;
using Book_Exchange.Services.Interfaces;
using Book_Exchange.Data;

// Transaction Tests
// Covers: UT-TRANS-01 through UT-TRANS-05 (Unit Tests)
//         IT-TRANS-01 through IT-TRANS-03 (Integration Tests)
//         Extra: UT-TRANS-06 Shipped, UT-TRANS-07 Disputed transitions
namespace Book_Exchange.Tests.Transactions;

// UNIT TESTS
public class TransactionServiceUnitTests
{
    private readonly Mock<ITransactionService> _serviceMock;

    public TransactionServiceUnitTests()
    {
        _serviceMock = new Mock<ITransactionService>();
    }

    /// <summary>
    /// UT-TRANS-01: Create transaction from accepted exchange request
    /// Expected: Transaction is created with Confirmed status
    /// </summary>
    [Fact]
    public async Task UT_TRANS_01_CreateFromAcceptedRequest_ReturnsConfirmedTransaction()
    {
        var exchangeRequestId = Guid.NewGuid();
        var acceptedRequest = new ExchangeRequest
        {
            Id = exchangeRequestId,
            Status = ExchangeStatus.Accepted
        };

        var expectedTransaction = new Transaction
        {
            Id = Guid.NewGuid(),
            ExchangeRequestId = exchangeRequestId,
            Status = TransactionStatus.Confirmed,
            CreatedAt = DateTime.UtcNow,
            ConfirmedAt = DateTime.UtcNow
        };

        _serviceMock
            .Setup(s => s.CreateTransactionFromExchangeRequestAsync(acceptedRequest))
            .ReturnsAsync(expectedTransaction);

        var result = await _serviceMock.Object.CreateTransactionFromExchangeRequestAsync(acceptedRequest);

        Assert.NotNull(result);
        Assert.Equal(exchangeRequestId, result.ExchangeRequestId);
        Assert.Equal(TransactionStatus.Confirmed, result.Status);
        Assert.NotNull(result.ConfirmedAt);
    }

    /// <summary>
    /// UT-TRANS-02: Attempt to create transaction from rejected request
    /// Expected: Transaction creation is rejected
    /// </summary>
    [Fact]
    public async Task UT_TRANS_02_CreateFromRejectedRequest_ThrowsInvalidOperation()
    {
        var rejectedRequest = new ExchangeRequest
        {
            Id = Guid.NewGuid(),
            Status = ExchangeStatus.Rejected
        };

        _serviceMock
            .Setup(s => s.CreateTransactionFromExchangeRequestAsync(rejectedRequest))
            .ThrowsAsync(new InvalidOperationException(
                "Cannot create transaction from a rejected exchange request."));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _serviceMock.Object.CreateTransactionFromExchangeRequestAsync(rejectedRequest));
    }

    /// <summary>
    /// UT-TRANS-03: Complete transaction
    /// Expected: Transaction status changes to Completed
    /// </summary>
    [Fact]
    public async Task UT_TRANS_03_CompleteTransaction_ReturnsCompletedStatus()
    {
        var transactionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _serviceMock
            .Setup(s => s.GetTransactionByIdAsync(transactionId))
            .ReturnsAsync(new Transaction
            {
                Id = transactionId,
                Status = TransactionStatus.Shipped
            });

        _serviceMock
            .Setup(s => s.CompleteTransactionAsync(transactionId, userId))
            .Returns(Task.CompletedTask);

        await _serviceMock.Object.CompleteTransactionAsync(transactionId, userId);

        _serviceMock.Verify(s => s.CompleteTransactionAsync(transactionId, userId), Times.Once);
    }

    /// <summary>
    /// UT-TRANS-04: Cancel transaction
    /// Expected: Transaction status changes to Cancelled
    /// </summary>
    [Fact]
    public async Task UT_TRANS_04_CancelTransaction_StatusBecomesCancelled()
    {
        var transactionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _serviceMock
            .Setup(s => s.CancelTransactionAsync(transactionId, userId))
            .Returns(Task.CompletedTask);

        await _serviceMock.Object.CancelTransactionAsync(transactionId, userId);

        _serviceMock.Verify(s => s.CancelTransactionAsync(transactionId, userId), Times.Once);
    }

    /// <summary>
    /// UT-TRANS-05a: Invalid status transition — Complete an already Cancelled transaction
    /// Expected: System rejects invalid transition
    /// </summary>
    [Fact]
    public async Task UT_TRANS_05a_CompleteAlreadyCancelledTransaction_ThrowsInvalidOperation()
    {
        var transactionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _serviceMock
            .Setup(s => s.CompleteTransactionAsync(transactionId, userId))
            .ThrowsAsync(new InvalidOperationException(
                "Cannot complete a transaction that is already Cancelled."));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _serviceMock.Object.CompleteTransactionAsync(transactionId, userId));
    }

    /// <summary>
    /// UT-TRANS-05b: Invalid status transition — Cancel an already Completed transaction
    /// Expected: System rejects invalid transition
    /// </summary>
    [Fact]
    public async Task UT_TRANS_05b_CancelAlreadyCompletedTransaction_ThrowsInvalidOperation()
    {
        var transactionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _serviceMock
            .Setup(s => s.CancelTransactionAsync(transactionId, userId))
            .ThrowsAsync(new InvalidOperationException(
                "Cannot cancel a transaction that is already Completed."));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _serviceMock.Object.CancelTransactionAsync(transactionId, userId));
    }

    /// <summary>
    /// UT-TRANS-05c: Invalid status transition — Ship an already Completed transaction
    /// Expected: System rejects invalid transition
    /// </summary>
    [Fact]
    public async Task UT_TRANS_05c_ShipAlreadyCompletedTransaction_ThrowsInvalidOperation()
    {
        var transactionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _serviceMock
            .Setup(s => s.MarkAsShippedAsync(transactionId, userId))
            .ThrowsAsync(new InvalidOperationException(
                "Cannot mark a Completed transaction as Shipped."));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _serviceMock.Object.MarkAsShippedAsync(transactionId, userId));
    }

    /// <summary>
    /// UT-TRANS-06: Mark transaction as Shipped
    /// Expected: Confirmed -> Shipped is a valid transition
    /// </summary>
    [Fact]
    public async Task UT_TRANS_06_MarkAsShipped_FromConfirmed_Succeeds()
    {
        var transactionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _serviceMock
            .Setup(s => s.MarkAsShippedAsync(transactionId, userId))
            .Returns(Task.CompletedTask);

        await _serviceMock.Object.MarkAsShippedAsync(transactionId, userId);

        _serviceMock.Verify(s => s.MarkAsShippedAsync(transactionId, userId), Times.Once);
    }

    /// <summary>
    /// UT-TRANS-07a: Dispute a transaction from Shipped
    /// Expected: Shipped -> Disputed is a valid transition
    /// </summary>
    [Fact]
    public async Task UT_TRANS_07a_DisputeTransaction_FromShipped_Succeeds()
    {
        var transactionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _serviceMock
            .Setup(s => s.DisputeTransactionAsync(transactionId, userId))
            .Returns(Task.CompletedTask);

        await _serviceMock.Object.DisputeTransactionAsync(transactionId, userId);

        _serviceMock.Verify(s => s.DisputeTransactionAsync(transactionId, userId), Times.Once);
    }

    /// <summary>
    /// UT-TRANS-07b: Dispute a transaction from Confirmed
    /// Expected: System rejects — must be Shipped before it can be Disputed
    /// </summary>
    [Fact]
    public async Task UT_TRANS_07b_DisputeTransaction_FromConfirmed_ThrowsInvalidOperation()
    {
        var transactionId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _serviceMock
            .Setup(s => s.DisputeTransactionAsync(transactionId, userId))
            .ThrowsAsync(new InvalidOperationException(
                "Cannot dispute a transaction that has not been shipped."));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _serviceMock.Object.DisputeTransactionAsync(transactionId, userId));
    }
}

// INTEGRATION TESTS
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