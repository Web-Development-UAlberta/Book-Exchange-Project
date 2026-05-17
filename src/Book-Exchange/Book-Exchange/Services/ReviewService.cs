using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Review;
using Book_Exchange.Services.Interfaces;
using Book_Exchange.Data;
using Microsoft.EntityFrameworkCore;

namespace Book_Exchange.Services;

public class ReviewService : IReviewService
{
    private readonly ApplicationDbContext _context;

    public ReviewService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// CreateReviewAsync
    /// - ReviewId is taken from the logged-in user, not from a form
    /// - TransactionId must reference a valid existing transaction
    /// - The transaction must have a status of Completed before a review can be submitted
    /// - RevieweeId must be the other party in the transaction (if reviewer is sender, reviewee is receiver and vice versa)
    /// - Only submit one review per transaction (duplicate check)
    /// - Rating must be between 1 and 5 inclusive
    /// - Comment is optional max 1000 characters
    /// - Throws InvalidOperationException if transaction is not complete or is a duplicate review
    /// - Throws UnauthorizedAccessException if reviewer is not part of the transaction
    /// - Throws KeyNotFoundException if transactionId does not exist
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="reviewerId"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="KeyNotFoundException"></exception>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<Review> CreateReviewAsync(CreateReviewDto dto, Guid reviewerId)
    {

        if (dto.Rating < 1 || dto.Rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5.");

        var transaction = await _context.Transactions
            .Include(t => t.StatusHistory)
            .Include(t => t.ExchangeRequest)
                .ThenInclude(er => er.TargetListing)
            .FirstOrDefaultAsync(t => t.Id == dto.TransactionId);

        if (transaction is null)
            throw new KeyNotFoundException($"Transaction {dto.TransactionId} not found.");

        var requesterId = transaction.ExchangeRequest.RequesterId;
        var listingOwnerId = transaction.ExchangeRequest.TargetListing.UserId;

        if (reviewerId != requesterId && reviewerId != listingOwnerId)
            throw new UnauthorizedAccessException("You are not a party to this transaction.");

        var latestStatus = transaction.StatusHistory
            .OrderByDescending(h => h.UpdatedAt)
            .FirstOrDefault();

        if (latestStatus is null || latestStatus.Status != TransactionStatus.Completed)
            throw new InvalidOperationException("Transaction must be completed before submitting a review.");

        var alreadyReviewed = await _context.Reviews
            .AnyAsync(r => r.TransactionId == dto.TransactionId && r.ReviewerId == reviewerId);

        if (alreadyReviewed)
            throw new InvalidOperationException("You have already reviewed this transaction.");

        var review = new Review
        {
            Id = Guid.NewGuid(),
            TransactionId = dto.TransactionId,
            ReviewerId = reviewerId,
            Rating = dto.Rating,
            Comment = dto.Comment,
            CreatedAt = DateTime.UtcNow
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        return review;
    }

    /// <summary>
    /// GetReviewsByUserIdAsync
    /// - Returns all reviews where the user is the reviewee (RevieweeId = userId)
    /// - Ordered by CreatedAt descending (most recent first)
    /// - Returns empty list if no reviews found for the user
    /// - Include navigation property so views can access reviewer username
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<IEnumerable<Review>> GetReviewsByUserIdAsync(Guid userId)
    {
        return await _context.Reviews
            .Include(r => r.Reviewer)
            .Include(r => r.Transaction)
                .ThenInclude(t => t.ExchangeRequest)
                    .ThenInclude(er => er.TargetListing)
            .Where(r =>
                r.ReviewerId != userId &&
                (
                    r.Transaction.ExchangeRequest.RequesterId == userId ||
                    r.Transaction.ExchangeRequest.TargetListing.UserId == userId
                ))
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// GetAverageRatingForUserAsync
    /// - Returns the average of all rating values where the user is the reviewee (RevieweeId = userId)
    /// - Returns 0.0 if the user has no reviews
    /// - Round to 2 decimal places
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<double> GetAverageRatingForUserAsync(Guid userId)
    {
        var ratings = await _context.Reviews
            .Include(r => r.Transaction)
                .ThenInclude(t => t.ExchangeRequest)
                    .ThenInclude(er => er.TargetListing)
            .Where(r =>
                r.ReviewerId != userId &&
                (
                    r.Transaction.ExchangeRequest.RequesterId == userId ||
                    r.Transaction.ExchangeRequest.TargetListing.UserId == userId
                ))
            .Select(r => (double)r.Rating)
            .ToListAsync();

        if (!ratings.Any())
            return 0.0;

        return Math.Round(ratings.Average(), 2);
    }

    /// <summary>
    /// GetReviewByIdAsync
    /// - Returns the review if it exists
    /// - Throws KeyNotFoundException if reviewId does not exist
    /// - Populates Reviewer and Reviewee name from ApplicationUser table
    /// </summary>
    /// <param name="reviewId"></param>
    /// <returns></returns>
    public async Task<Review> GetReviewByIdAsync(Guid reviewId)
    {
        var review = await _context.Reviews
            .Include(r => r.Reviewer)
            .Include(r => r.Transaction)
                .ThenInclude(t => t.ExchangeRequest)
                    .ThenInclude(er => er.TargetListing)
                        .ThenInclude(l => l.User)
            .FirstOrDefaultAsync(r => r.Id == reviewId);

        if (review is null)
            throw new KeyNotFoundException($"Review {reviewId} not found.");

        return review;
    }
}