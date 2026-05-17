using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.ExchangeRequest;
using Book_Exchange.Models.DTOs.Shipping;
using Book_Exchange.Services;
using Book_Exchange.Services.Interfaces;
using Book_Exchange.Data;

namespace Book_Exchange.Tests.Integration;

// Exchange Request Integration Tests
// Covers: IT-EXCH-01 through IT-EXCH-05

public class ExchangeRequestServiceIntegrationTests : IDisposable
{
    private readonly ApplicationDbContext _db;
    private readonly IExchangeRequestService _service;

    public ExchangeRequestServiceIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _db = new ApplicationDbContext(options);

        var transactionServiceMock = new Mock<ITransactionService>();

        transactionServiceMock
            .Setup(s => s.CreateTransactionFromExchangeRequestAsync(It.IsAny<ExchangeRequest>()))
            .ReturnsAsync((ExchangeRequest req) => new Transaction
            {
                Id = Guid.NewGuid(),
                ExchangeRequestId = req.Id,
                CreatedAt = DateTime.UtcNow
            });

        var shippingServiceMock = new Mock<IShippingService>();

        shippingServiceMock
            .Setup(s => s.GetLowestQuoteBetweenUsersAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<int>()))
            .ReturnsAsync((ShippingQuoteDto?)null);

        var bookSearchApiMock = new Mock<IBookSearchApi>();

        bookSearchApiMock
            .Setup(s => s.GetBookByIsbnAsync(It.IsAny<string>()))
            .ReturnsAsync((Book_Exchange.Models.DTOs.Book.BookInfoDto?)null);

        _service = new ExchangeRequestService(
            _db,
            transactionServiceMock.Object,
            shippingServiceMock.Object,
            bookSearchApiMock.Object);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task IT_EXCH_01_CreateBuySellExchangeRequest_IsSavedWithRequestedStatus()
    {
        var owner = new ApplicationUser { Id = Guid.NewGuid(), UserName = "owner" };
        var requester = new ApplicationUser { Id = Guid.NewGuid(), UserName = "requester" };

        var targetListing = new Listing
        {
            Id = Guid.NewGuid(),
            UserId = owner.Id,
            User = owner,
            Isbn = "9780141036144",
            Condition = BookCondition.Good,
            Price = 25.00m,
            WeightGrams = 500,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.AddRange(owner, requester);
        _db.Listings.Add(targetListing);
        await _db.SaveChangesAsync();

        var dto = new CreateExchangeRequestDto
        {
            TargetListingId = targetListing.Id,
            OfferedListingIds = new List<Guid>(),
            Price = 25.00m
        };

        var result = await _service.CreateExchangeRequestAsync(dto, requester.Id);

        Assert.NotNull(result);
        Assert.Equal(targetListing.Id, result.TargetListingId);
        Assert.Equal(ExchangeStatus.Requested, result.Status);

        var saved = await _db.ExchangeRequests.FindAsync(result.Id);
        Assert.NotNull(saved);
    }

    [Fact]
    public async Task IT_EXCH_02_DuplicatePendingRequest_ThrowsInvalidOperation()
    {
        var owner = new ApplicationUser { Id = Guid.NewGuid(), UserName = "owner2" };
        var requester = new ApplicationUser { Id = Guid.NewGuid(), UserName = "requester2" };

        var targetListing = new Listing
        {
            Id = Guid.NewGuid(),
            UserId = owner.Id,
            User = owner,
            Isbn = "9780141036144",
            Condition = BookCondition.Good,
            Price = 25.00m,
            WeightGrams = 500,
            CreatedAt = DateTime.UtcNow
        };

        var existingRequest = new ExchangeRequest
        {
            Id = Guid.NewGuid(),
            TargetListingId = targetListing.Id,
            TargetListing = targetListing,
            RequesterId = requester.Id,
            Requester = requester,
            Status = ExchangeStatus.Requested,
            Price = 25.00m,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.AddRange(owner, requester);
        _db.Listings.Add(targetListing);
        _db.ExchangeRequests.Add(existingRequest);
        await _db.SaveChangesAsync();

        var dto = new CreateExchangeRequestDto
        {
            TargetListingId = targetListing.Id,
            OfferedListingIds = new List<Guid>(),
            Price = 25.00m
        };

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CreateExchangeRequestAsync(dto, requester.Id));
    }

    [Fact]
    public async Task IT_EXCH_03_CreateBookSwapRequest_SavesOfferedListings()
    {
        var owner = new ApplicationUser { Id = Guid.NewGuid(), UserName = "owner3" };
        var requester = new ApplicationUser { Id = Guid.NewGuid(), UserName = "requester3" };

        var targetListing = new Listing
        {
            Id = Guid.NewGuid(),
            UserId = owner.Id,
            User = owner,
            Isbn = "9780141036144",
            Condition = BookCondition.Good,
            Price = 25.00m,
            WeightGrams = 500,
            CreatedAt = DateTime.UtcNow
        };

        var offeredListing = new Listing
        {
            Id = Guid.NewGuid(),
            UserId = requester.Id,
            User = requester,
            Isbn = "9780062316097",
            Condition = BookCondition.Good,
            Price = 12.50m,
            WeightGrams = 300,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.AddRange(owner, requester);
        _db.Listings.AddRange(targetListing, offeredListing);
        await _db.SaveChangesAsync();

        var dto = new CreateExchangeRequestDto
        {
            TargetListingId = targetListing.Id,
            OfferedListingIds = new List<Guid> { offeredListing.Id },
            Price = 12.50m
        };

        var result = await _service.CreateExchangeRequestAsync(dto, requester.Id);

        Assert.NotNull(result);
        Assert.Equal(ExchangeStatus.Requested, result.Status);

        var saved = await _db.ExchangeRequests
            .Include(e => e.ExchangeRequestItems)
            .FirstOrDefaultAsync(e => e.Id == result.Id);

        Assert.NotNull(saved);
        Assert.Single(saved!.ExchangeRequestItems);
        Assert.Contains(saved.ExchangeRequestItems, i => i.OfferedListingId == offeredListing.Id);
    }

    [Fact]
    public async Task IT_EXCH_04_ListingOwnerAcceptsExchangeRequest_StatusAcceptedAndNotificationCreated()
    {
        var owner = new ApplicationUser { Id = Guid.NewGuid(), UserName = "owner4" };
        var requester = new ApplicationUser { Id = Guid.NewGuid(), UserName = "requester4" };

        var targetListing = new Listing
        {
            Id = Guid.NewGuid(),
            UserId = owner.Id,
            User = owner,
            Isbn = "9780141036144",
            Condition = BookCondition.Good,
            Price = 25.00m,
            WeightGrams = 500,
            CreatedAt = DateTime.UtcNow
        };

        var exchangeRequest = new ExchangeRequest
        {
            Id = Guid.NewGuid(),
            TargetListingId = targetListing.Id,
            TargetListing = targetListing,
            RequesterId = requester.Id,
            Requester = requester,
            Status = ExchangeStatus.Requested,
            Price = 25.00m,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.AddRange(owner, requester);
        _db.Listings.Add(targetListing);
        _db.ExchangeRequests.Add(exchangeRequest);
        await _db.SaveChangesAsync();

        await _service.AcceptExchangeRequestAsync(exchangeRequest.Id, owner.Id);

        var savedRequest = await _db.ExchangeRequests.FindAsync(exchangeRequest.Id);

        Assert.NotNull(savedRequest);
        Assert.Equal(ExchangeStatus.Accepted, savedRequest!.Status);
        Assert.NotNull(savedRequest.AcceptedAt);

        var notification = await _db.Notifications
            .FirstOrDefaultAsync(n =>
                n.UserId == requester.Id &&
                n.RelatedExchangeRequestId == exchangeRequest.Id &&
                n.Category == NotificationCategory.ExchangeAccepted);

        Assert.NotNull(notification);
    }

    [Fact]
    public async Task IT_EXCH_05_ListingOwnerRejectsExchangeRequest_StatusRejectedAndNotificationCreated()
    {
        var owner = new ApplicationUser { Id = Guid.NewGuid(), UserName = "owner5" };
        var requester = new ApplicationUser { Id = Guid.NewGuid(), UserName = "requester5" };

        var targetListing = new Listing
        {
            Id = Guid.NewGuid(),
            UserId = owner.Id,
            User = owner,
            Isbn = "9780141036144",
            Condition = BookCondition.Good,
            Price = 25.00m,
            WeightGrams = 500,
            CreatedAt = DateTime.UtcNow
        };

        var exchangeRequest = new ExchangeRequest
        {
            Id = Guid.NewGuid(),
            TargetListingId = targetListing.Id,
            TargetListing = targetListing,
            RequesterId = requester.Id,
            Requester = requester,
            Status = ExchangeStatus.Requested,
            Price = 25.00m,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.AddRange(owner, requester);
        _db.Listings.Add(targetListing);
        _db.ExchangeRequests.Add(exchangeRequest);
        await _db.SaveChangesAsync();

        await _service.RejectExchangeRequestAsync(exchangeRequest.Id, owner.Id);

        var savedRequest = await _db.ExchangeRequests.FindAsync(exchangeRequest.Id);

        Assert.NotNull(savedRequest);
        Assert.Equal(ExchangeStatus.Rejected, savedRequest!.Status);

        var notification = await _db.Notifications
            .FirstOrDefaultAsync(n =>
                n.UserId == requester.Id &&
                n.RelatedExchangeRequestId == exchangeRequest.Id &&
                n.Category == NotificationCategory.ExchangeRejected);

        Assert.NotNull(notification);
    }
}