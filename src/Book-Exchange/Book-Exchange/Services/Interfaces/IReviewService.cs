using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Review;

namespace Book_Exchange.Services.Interfaces;

public interface IReviewService
{
    Task<Review> CreateReviewAsync(CreateReviewDto dto, Guid reviewerId);

    Task<IEnumerable<Review>> GetReviewsByUserIdAsync(Guid userId);

    Task<double> GetAverageRatingForUserAsync(Guid userId);

    Task<Review> GetReviewByIdAsync(Guid reviewId);
}
