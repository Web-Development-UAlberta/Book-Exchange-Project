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

// TODO: Uncomment when ReviewService is implemented
// public class ReviewServiceIntegrationTests : IDisposable
// {
//     private readonly ApplicationDbContext _db;
//     private readonly IReviewService _service;
//
//     public ReviewServiceIntegrationTests()
//     {
//         var options = new DbContextOptionsBuilder<ApplicationDbContext>()
//             .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
//             .Options;
//
//         _db = new ApplicationDbContext(options);
//
//         _service = new ReviewService(_db);
//         // _service = null!;
//     }
//
//     public void Dispose()
//     {
//         _db.Dispose();
//     }
//
//     /// <summary>
//     /// IT-REVIEW-01: User submits review after completed transaction
//     /// Expected: Review is saved and linked to transaction
//     /// </summary>
//     [Fact]
//     public async Task IT_REVIEW_01_SubmitReviewAfterCompletedTransaction_ReviewIsSaved()
//     {
//         var reviewer = new ApplicationUser { Id = Guid.NewGuid(), UserName = "reviewer" };
//         var owner = new ApplicationUser { Id = Guid.NewGuid(), UserName = "owner" };
//
//         var listing = new Listing
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
//             TargetListingId = listing.Id,
//             RequesterId = reviewer.Id,
//             Status = ExchangeStatus.Accepted,
//             CreatedAt = DateTime.UtcNow,
//             AcceptedAt = DateTime.UtcNow
//         };
//
//         var transaction = new Transaction
//         {
//             Id = Guid.NewGuid(),
//             ExchangeRequestId = exchangeRequest.Id,
//             CreatedAt = DateTime.UtcNow,
//             CompletedAt = DateTime.UtcNow
//         };
//
//         var statusHistory = new TransactionStatusHistory
//         {
//             Id = Guid.NewGuid(),
//             TransactionId = transaction.Id,
//             Status = TransactionStatus.Completed,
//             UpdatedByUserId = owner.Id,
//             UpdatedAt = DateTime.UtcNow
//         };
//
//         _db.Users.AddRange(reviewer, owner);
//         _db.Listings.Add(listing);
//         _db.ExchangeRequests.Add(exchangeRequest);
//         _db.Transactions.Add(transaction);
//         _db.TransactionStatusHistories.Add(statusHistory);
//         await _db.SaveChangesAsync();
//
//         var dto = new CreateReviewDto
//         {
//             TransactionId = transaction.Id,
//             Rating = 5,
//             Comment = "Great exchange."
//         };
//
//         var result = await _service.CreateReviewAsync(dto, reviewer.Id);
//
//         Assert.NotNull(result);
//         Assert.Equal(transaction.Id, result.TransactionId);
//         Assert.Equal(reviewer.Id, result.ReviewerId);
//         Assert.Equal(5, result.Rating);
//
//         var saved = await _db.Reviews.FirstOrDefaultAsync(r => r.Id == result.Id);
//
//         Assert.NotNull(saved);
//         Assert.Equal(transaction.Id, saved!.TransactionId);
//     }
//
//     /// <summary>
//     /// IT-REVIEW-02: User profile displays updated rating
//     /// Expected: Aggregate rating is updated on profile page
//     /// </summary>
//     [Fact]
//     public async Task IT_REVIEW_02_UserProfileDisplaysUpdatedRating_AverageRatingIsUpdated()
//     {
//         var reviewee = new ApplicationUser { Id = Guid.NewGuid(), UserName = "reviewee" };
//         var reviewer1 = new ApplicationUser { Id = Guid.NewGuid(), UserName = "reviewer1" };
//         var reviewer2 = new ApplicationUser { Id = Guid.NewGuid(), UserName = "reviewer2" };
//
//         var transaction1 = new Transaction
//         {
//             Id = Guid.NewGuid(),
//             ExchangeRequestId = Guid.NewGuid(),
//             CreatedAt = DateTime.UtcNow,
//             CompletedAt = DateTime.UtcNow
//         };
//
//         var transaction2 = new Transaction
//         {
//             Id = Guid.NewGuid(),
//             ExchangeRequestId = Guid.NewGuid(),
//             CreatedAt = DateTime.UtcNow,
//             CompletedAt = DateTime.UtcNow
//         };
//
//         _db.Users.AddRange(reviewee, reviewer1, reviewer2);
//         _db.Transactions.AddRange(transaction1, transaction2);
//
//         _db.Reviews.AddRange(
//             new Review
//             {
//                 Id = Guid.NewGuid(),
//                 TransactionId = transaction1.Id,
//                 ReviewerId = reviewer1.Id,
//                 Rating = 5,
//                 Comment = "Excellent.",
//                 CreatedAt = DateTime.UtcNow
//             },
//             new Review
//             {
//                 Id = Guid.NewGuid(),
//                 TransactionId = transaction2.Id,
//                 ReviewerId = reviewer2.Id,
//                 Rating = 3,
//                 Comment = "Good.",
//                 CreatedAt = DateTime.UtcNow
//             }
//         );
//
//         await _db.SaveChangesAsync();
//
//         var average = await _service.GetAverageRatingForUserAsync(reviewee.Id);
//
//         Assert.Equal(4.00, average);
//     }
// }