using Xunit;
using Microsoft.EntityFrameworkCore;
using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Review;
using Book_Exchange.Services;
using Book_Exchange.Services.Interfaces;
using Book_Exchange.Data;

namespace Book_Exchange.Tests.Integration;

// Review Integration Tests
// Covers: IT-REVIEW-01 through IT-REVIEW-02

public class ReviewServiceIntegrationTests : IDisposable
{
    private readonly ApplicationDbContext _db;
    private readonly IReviewService _service;

    public ReviewServiceIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _db = new ApplicationDbContext(options);
        _service = new ReviewService(_db);
    }

    public void Dispose()
    {
        _db.Dispose();
    }

    // Helpers - to seed data for tests
    /// <summary>
    /// Seeds a complete chain: Listing → ExchangeRequest → Transaction → Completed status.
    /// Returns the Transaction Id and the two party Ids (requesterId, listingOwnerId).
    /// </summary>
    private async Task<(Guid transactionId, Guid requesterId, Guid listingOwnerId)>
        SeedCompletedTransactionAsync()
    {
        var requester = new ApplicationUser { Id = Guid.NewGuid(), UserName = "requester" };
        var owner = new ApplicationUser { Id = Guid.NewGuid(), UserName = "owner" };

        var listing = new Listing
        {
            Id = Guid.NewGuid(),
            UserId = owner.Id,
            Isbn = "9780141036144",
            Condition = BookCondition.Good,
            Price = 25.00m,
            WeightGrams = 500,
            CreatedAt = DateTime.UtcNow
        };

        var exchangeRequest = new ExchangeRequest
        {
            Id = Guid.NewGuid(),
            TargetListingId = listing.Id,
            RequesterId = requester.Id,
            Status = ExchangeStatus.Accepted,
            CreatedAt = DateTime.UtcNow,
            AcceptedAt = DateTime.UtcNow
        };

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            ExchangeRequestId = exchangeRequest.Id,
            CreatedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow
        };

        var statusHistory = new TransactionStatusHistory
        {
            Id = Guid.NewGuid(),
            TransactionId = transaction.Id,
            Status = TransactionStatus.Completed,
            UpdatedByUserId = owner.Id,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Users.AddRange(requester, owner);
        _db.Listings.Add(listing);
        _db.ExchangeRequests.Add(exchangeRequest);
        _db.Transactions.Add(transaction);
        _db.TransactionStatusHistories.Add(statusHistory);
        await _db.SaveChangesAsync();

        return (transaction.Id, requester.Id, owner.Id);
    }

    /// <summary>
    /// IT-REVIEW-01: User submits review after completed transaction
    /// Expected: Review is saved and linked to transaction
    /// </summary>
    [Fact]
    public async Task IT_REVIEW_01_SubmitReviewAfterCompletedTransaction_ReviewIsSaved()
    {
        var (transactionId, reviewerId, _) = await SeedCompletedTransactionAsync();

        var dto = new CreateReviewDto
        {
            TransactionId = transactionId,
            Rating = 5,
            Comment = "Great exchange."
        };

        var result = await _service.CreateReviewAsync(dto, reviewerId);

        Assert.NotNull(result);
        Assert.Equal(transactionId, result.TransactionId);
        Assert.Equal(reviewerId, result.ReviewerId);
        Assert.Equal(5, result.Rating);

        var saved = await _db.Reviews.FirstOrDefaultAsync(r => r.Id == result.Id);
        Assert.NotNull(saved);
        Assert.Equal(transactionId, saved!.TransactionId);
    }

    /// <summary>
    /// IT-REVIEW-02: User profile displays updated rating
    /// Expected: Aggregate average is calculated correctly across multiple reviews.
    /// </summary>
    [Fact]
    public async Task IT_REVIEW_02_UserProfileDisplaysUpdatedRating_AverageRatingIsUpdated()
    {
        var reviewee = new ApplicationUser { Id = Guid.NewGuid(), UserName = "reviewee" };
        var reviewer1 = new ApplicationUser { Id = Guid.NewGuid(), UserName = "reviewer1" };
        var reviewer2 = new ApplicationUser { Id = Guid.NewGuid(), UserName = "reviewer2" };
        _db.Users.AddRange(reviewee, reviewer1, reviewer2);
        await _db.SaveChangesAsync();

        var listing1 = new Listing
        {
            Id = Guid.NewGuid(),
            UserId = reviewee.Id,
            Isbn = "9780141036144",
            Condition = BookCondition.Good,
            Price = 10m,
            WeightGrams = 400,
            CreatedAt = DateTime.UtcNow
        };
        var er1 = new ExchangeRequest
        {
            Id = Guid.NewGuid(),
            TargetListingId = listing1.Id,
            RequesterId = reviewer1.Id,
            Status = ExchangeStatus.Accepted,
            CreatedAt = DateTime.UtcNow,
            AcceptedAt = DateTime.UtcNow
        };
        var tx1 = new Transaction
        {
            Id = Guid.NewGuid(),
            ExchangeRequestId = er1.Id,
            CreatedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow
        };

        var listing2 = new Listing
        {
            Id = Guid.NewGuid(),
            UserId = reviewee.Id,
            Isbn = "9780451524935",
            Condition = BookCondition.Good,
            Price = 12m,
            WeightGrams = 350,
            CreatedAt = DateTime.UtcNow
        };
        var er2 = new ExchangeRequest
        {
            Id = Guid.NewGuid(),
            TargetListingId = listing2.Id,
            RequesterId = reviewer2.Id,
            Status = ExchangeStatus.Accepted,
            CreatedAt = DateTime.UtcNow,
            AcceptedAt = DateTime.UtcNow
        };
        var tx2 = new Transaction
        {
            Id = Guid.NewGuid(),
            ExchangeRequestId = er2.Id,
            CreatedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow
        };

        _db.Listings.AddRange(listing1, listing2);
        _db.ExchangeRequests.AddRange(er1, er2);
        _db.Transactions.AddRange(tx1, tx2);

        _db.Reviews.AddRange(
            new Review
            {
                Id = Guid.NewGuid(),
                TransactionId = tx1.Id,
                ReviewerId = reviewer1.Id,
                Rating = 5,
                Comment = "Excellent.",
                CreatedAt = DateTime.UtcNow
            },
            new Review
            {
                Id = Guid.NewGuid(),
                TransactionId = tx2.Id,
                ReviewerId = reviewer2.Id,
                Rating = 3,
                Comment = "Good.",
                CreatedAt = DateTime.UtcNow
            }
        );

        await _db.SaveChangesAsync();

        var average = await _service.GetAverageRatingForUserAsync(reviewee.Id);

        Assert.Equal(4.00, average);
    }
}