using Xunit;
using Microsoft.EntityFrameworkCore;
using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Transaction;
using Book_Exchange.Models.DTOs.Book;
using Book_Exchange.Services;
using Book_Exchange.Data;
using Moq;
using Book_Exchange.Services.Interfaces;

// INTEGRATION TESTS
// Covers: IT-TRANS-01 through IT-TRANS-03 (Integration Tests)

namespace Book_Exchange.Tests.Integration;

public class TransactionServiceIntegrationTests : IDisposable
{
    private readonly ApplicationDbContext _db;
    private readonly TransactionService _service;

    public TransactionServiceIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _db = new ApplicationDbContext(options);

        var mockBookSearch = new Mock<IBookSearchApi>();

        mockBookSearch
            .Setup(b => b.SearchBooksAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(new List<BookInfoDto>
            {
                new BookInfoDto { Title = "Test Book" }
            });

        _service = new TransactionService(_db, mockBookSearch.Object);
    }

    public void Dispose() => _db.Dispose();

    // Helpers - to seed data for tests
    private async Task<(ApplicationUser requester, ApplicationUser listingOwner)> SeedUsersAsync()
    {
        var requester = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "requester",
            Email = "requester@test.com",
            NormalizedEmail = "REQUESTER@TEST.COM",
            NormalizedUserName = "REQUESTER",
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var listingOwner = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = "listingowner",
            Email = "owner@test.com",
            NormalizedEmail = "OWNER@TEST.COM",
            NormalizedUserName = "LISTINGOWNER",
            SecurityStamp = Guid.NewGuid().ToString()
        };

        _db.Users.AddRange(requester, listingOwner);
        await _db.SaveChangesAsync();

        return (requester, listingOwner);
    }

    private async Task<ExchangeRequest> SeedAcceptedExchangeRequestAsync(
        Guid requesterId,
        Guid listingOwnerId,
        decimal? price = 10.00m)
    {
        var listing = new Listing
        {
            Id = Guid.NewGuid(),
            UserId = listingOwnerId,
            Isbn = "9780593311615",
            Condition = BookCondition.Good,
            Price = price ?? 10.00m,
            WeightGrams = 300,
            CreatedAt = DateTime.UtcNow
        };

        var request = new ExchangeRequest
        {
            Id = Guid.NewGuid(),
            RequesterId = requesterId,
            TargetListingId = listing.Id,
            Status = ExchangeStatus.Accepted,
            Price = price,
            CreatedAt = DateTime.UtcNow,
            AcceptedAt = DateTime.UtcNow
        };

        _db.Listings.Add(listing);
        _db.ExchangeRequests.Add(request);
        await _db.SaveChangesAsync();

        // Reload with navigation properties the service depends on
        return await _db.ExchangeRequests
            .Include(er => er.TargetListing)
                .ThenInclude(l => l.User)
            .Include(er => er.Requester)
            .Include(er => er.ExchangeRequestItems)
            .FirstAsync(er => er.Id == request.Id);
    }

    /// <summary>
    /// IT-TRANS-01: Accepted exchange creates transaction
    /// Expected: Transaction is created with Confirmed status linked to ExchangeRequest
    /// </summary>
    [Fact]
    public async Task IT_TRANS_01_AcceptedExchange_CreatesTransactionLinkedToRequest()
    {
        var (requester, listingOwner) = await SeedUsersAsync();
        var acceptedRequest = await SeedAcceptedExchangeRequestAsync(requester.Id, listingOwner.Id);

        var transaction = await _service.CreateTransactionFromExchangeRequestAsync(acceptedRequest);

        Assert.NotNull(transaction);
        Assert.Equal(acceptedRequest.Id, transaction.ExchangeRequestId);

        var persisted = await _db.Transactions
            .Include(t => t.StatusHistory)
            .FirstOrDefaultAsync(t => t.Id == transaction.Id);

        Assert.NotNull(persisted);
        Assert.Equal(acceptedRequest.Id, persisted.ExchangeRequestId);

        var status = persisted.StatusHistory
            .OrderByDescending(h => h.UpdatedAt)
            .FirstOrDefault()?.Status;

        Assert.Equal(TransactionStatus.Confirmed, status);
    }

    /// <summary>
    /// IT-TRANS-02: Completing a shipped transaction sets CompletedAt timestamp
    /// Expected: Transaction status history contains Completed entry and CompletedAt is set
    /// </summary>
    [Fact]
    public async Task IT_TRANS_02_CompletingShippedTransaction_SetsCompletedAt()
    {
        var (requester, listingOwner) = await SeedUsersAsync();
        var acceptedRequest = await SeedAcceptedExchangeRequestAsync(requester.Id, listingOwner.Id);

        var transaction = await _service.CreateTransactionFromExchangeRequestAsync(acceptedRequest);

        // Shipped then completed
        await _service.MarkAsShippedAsync(transaction.Id, requester.Id);
        await _service.CompleteTransactionAsync(transaction.Id, requester.Id);

        var completed = await _db.Transactions
            .Include(t => t.StatusHistory)
            .FirstOrDefaultAsync(t => t.Id == transaction.Id);

        Assert.NotNull(completed);
        Assert.NotNull(completed.CompletedAt);

        var currentStatus = completed.StatusHistory
            .OrderByDescending(h => h.UpdatedAt)
            .FirstOrDefault()?.Status;

        Assert.Equal(TransactionStatus.Completed, currentStatus);
    }

    /// <summary>
    /// IT-TRANS-03: GetTransactionsByUserIdAsync returns only the user's transactions
    /// Expected: User sees only transactions they are a participant in
    /// </summary>
    [Fact]
    public async Task IT_TRANS_03_GetTransactionHistory_ReturnsOnlyUsersTransactions()
    {
        var (requester, listingOwner) = await SeedUsersAsync();

        // Seed two transactions for this user
        for (int i = 0; i < 2; i++)
        {
            var request = await SeedAcceptedExchangeRequestAsync(requester.Id, listingOwner.Id);
            await _service.CreateTransactionFromExchangeRequestAsync(request);
        }

        // Seed one unrelated transaction for a different user
        var (otherRequester, otherOwner) = await SeedUsersAsync();
        var otherRequest = await SeedAcceptedExchangeRequestAsync(otherRequester.Id, otherOwner.Id);
        await _service.CreateTransactionFromExchangeRequestAsync(otherRequest);

        var results = await _service.GetTransactionsByUserIdAsync(requester.Id);

        Assert.NotNull(results);
        Assert.Equal(2, results.Count());
    }
}