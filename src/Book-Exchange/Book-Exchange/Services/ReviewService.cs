using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Review;
using Book_Exchange.Services.Interfaces;
using Book_Exchange.Data;

namespace Book_Exchange.Services;
// TODO: Implement once ORM is set up and database context is available.
public class ReviewService : IReviewService
{
    private readonly ApplicationDbContext _context;

    public ReviewService(ApplicationDbContext context)
    {
        _context = context;
    }

    //CreateREviewAsync
    // - ReviewId is taken from the logged-in user, not from a form
    // - TransactionId must reference a valid existing transaction
    // - The transaction must have a status of Completed before a review can be submitted
    // - RevieweeId must be the other party in the transaction (if reviewer is sender, reviewee is receiver and vice versa)
    // - Only submit one review per transaction (duplicate check)
    // - Rating must be between 1 and 5 inclusive
    // - Comment is optional max 1000 characters
    // - Throws InvalidOperationException if transaction is not complete or is a duplicate review
    // - Throws UnauthorizedAccessException if reviewer is not part of the transaction
    // - Throws KeyNotFoundException if transactionId does not exist
    public Task<Review> CreateReviewAsync(CreateReviewDto dto, Guid reviewerId)
    {
        throw new NotImplementedException();
    }

    // GetReviewsByUserIdAsync
    // - Returns all reviews where the user is the reviewee (RevieweeId = userId)
    // - Ordered by CreatedAt descending (most recent first)
    // - Returns empty list if no reviews found for the user
    // - Include nvaigation property so views can access reviewer username
    public Task<IEnumerable<Review>> GetReviewsByUserIdAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    // GetAverageRatingForUserAsync
    // - Returns the average of all rating values where the user is the reviewee (RevieweeId = userId)
    // - Returns 0.0 if the user has no reviews
    // - Round to 2 decimal places
    public Task<double> GetAverageRatingForUserAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    // GetReviewByIdAsync
    // - Returns the review if it exists
    // - Throws KeyNotFoundException if reviewId does not exist
    // - Populates Reviewer and Reviewee name from ApplicationUser table
    public Task<Review> GetReviewByIdAsync(Guid reviewId)
    {
        throw new NotImplementedException();
    }
}