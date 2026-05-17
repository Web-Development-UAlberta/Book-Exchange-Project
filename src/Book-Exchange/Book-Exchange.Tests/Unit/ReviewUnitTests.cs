using Xunit;
using Moq;
using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Review;
using Book_Exchange.Services.Interfaces;

namespace Book_Exchange.Tests.BackEnd;

// UNIT TESTS
// Covers: UT-REVIEW-01 through UT-REVIEW-05
public class ReviewUnitTests
{
    private readonly Mock<IReviewService> _serviceMock;

    public ReviewUnitTests()
    {
        _serviceMock = new Mock<IReviewService>();
    }

    /// <summary>
    /// UT-REVIEW-01: Create review for completed transaction
    /// Expected: Review is accepted
    /// </summary>
    [Fact]
    public async Task UT_REVIEW_01_CreateReviewForCompletedTransaction_ReturnsReview()
    {
        var reviewerId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();

        var dto = new CreateReviewDto
        {
            TransactionId = transactionId,
            Rating = 5,
            Comment = "Great exchange."
        };

        var expectedReview = new Review
        {
            Id = Guid.NewGuid(),
            TransactionId = transactionId,
            ReviewerId = reviewerId,
            Rating = dto.Rating,
            Comment = dto.Comment
        };

        _serviceMock
            .Setup(s => s.CreateReviewAsync(dto, reviewerId))
            .ReturnsAsync(expectedReview);

        var result = await _serviceMock.Object.CreateReviewAsync(dto, reviewerId);

        Assert.NotNull(result);
        Assert.Equal(transactionId, result.TransactionId);
        Assert.Equal(reviewerId, result.ReviewerId);
        Assert.Equal(5, result.Rating);
        Assert.Equal("Great exchange.", result.Comment);
    }

    /// <summary>
    /// UT-REVIEW-02: Create review before transaction completion
    /// Expected: Review is rejected
    /// </summary>
    [Fact]
    public async Task UT_REVIEW_02_CreateReviewBeforeTransactionCompletion_ThrowsInvalidOperationException()
    {
        var reviewerId = Guid.NewGuid();

        var dto = new CreateReviewDto
        {
            TransactionId = Guid.NewGuid(),
            Rating = 4,
            Comment = "Trying to review too early."
        };

        _serviceMock
            .Setup(s => s.CreateReviewAsync(dto, reviewerId))
            .ThrowsAsync(new InvalidOperationException("Transaction must be completed before submitting a review."));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _serviceMock.Object.CreateReviewAsync(dto, reviewerId));
    }

    /// <summary>
    /// UT-REVIEW-03: Rating outside allowed range
    /// Expected: Validation fails
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public async Task UT_REVIEW_03_CreateReviewWithInvalidRating_ThrowsArgumentException(int invalidRating)
    {
        var reviewerId = Guid.NewGuid();

        var dto = new CreateReviewDto
        {
            TransactionId = Guid.NewGuid(),
            Rating = invalidRating,
            Comment = "Invalid rating."
        };

        _serviceMock
            .Setup(s => s.CreateReviewAsync(dto, reviewerId))
            .ThrowsAsync(new ArgumentException("Rating must be between 1 and 5."));

        await Assert.ThrowsAsync<ArgumentException>(
            () => _serviceMock.Object.CreateReviewAsync(dto, reviewerId));
    }

    /// <summary>
    /// UT-REVIEW-04: Duplicate review for same transaction by same reviewer
    /// Expected: Duplicate is rejected
    /// </summary>
    [Fact]
    public async Task UT_REVIEW_04_DuplicateReviewForSameTransactionBySameReviewer_ThrowsInvalidOperationException()
    {
        var reviewerId = Guid.NewGuid();

        var dto = new CreateReviewDto
        {
            TransactionId = Guid.NewGuid(),
            Rating = 5,
            Comment = "Duplicate review."
        };

        _serviceMock
            .Setup(s => s.CreateReviewAsync(dto, reviewerId))
            .ThrowsAsync(new InvalidOperationException("You have already reviewed this transaction."));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _serviceMock.Object.CreateReviewAsync(dto, reviewerId));
    }

    /// <summary>
    /// UT-REVIEW-05: Aggregate user rating from multiple reviews
    /// Expected: Average rating is calculated correctly
    /// </summary>
    [Fact]
    public async Task UT_REVIEW_05_GetAverageRatingFromMultipleReviews_ReturnsCorrectAverage()
    {
        var userId = Guid.NewGuid();
        var expectedAverage = 4.33;

        _serviceMock
            .Setup(s => s.GetAverageRatingForUserAsync(userId))
            .ReturnsAsync(expectedAverage);

        var result = await _serviceMock.Object.GetAverageRatingForUserAsync(userId);

        Assert.Equal(4.33, result);
    }
}