using Book_Exchange.Data;
using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.ExchangeRequest;
using Book_Exchange.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Book_Exchange.Services;

public class ExchangeRequestService : IExchangeRequestService
{
    private readonly ApplicationDbContext _context;
    private readonly ITransactionService _transactionService;
    private readonly IBookSearchApi _bookSearchApi;

    public ExchangeRequestService(ApplicationDbContext context, ITransactionService transactionService, IBookSearchApi bookSearchApi)
    {
        _context = context;
        _transactionService = transactionService;
        _bookSearchApi = bookSearchApi;
    }

    public async Task<ExchangeRequest> CreateExchangeRequestAsync(
        CreateExchangeRequestDto dto,
        Guid userId)
    {
        var targetListing = await _context.Listings
            .Include(l => l.User)
            .FirstOrDefaultAsync(l => l.Id == dto.TargetListingId)
            ?? throw new KeyNotFoundException("Target listing not found.");

        if (targetListing.UserId == userId)
        {
            throw new InvalidOperationException("You cannot create an exchange request for your own listing.");
        }

        if (dto.OfferedListingIds.Count > 3)
        {
            throw new InvalidOperationException("You can offer up to 3 books.");
        }

        var duplicatePending = await _context.ExchangeRequests.AnyAsync(e =>
            e.TargetListingId == dto.TargetListingId &&
            e.RequesterId == userId &&
            e.Status == ExchangeStatus.Requested);

        if (duplicatePending)
        {
            throw new InvalidOperationException("You already have a pending request for this listing.");
        }

        var offeredListings = await _context.Listings
            .Where(l => dto.OfferedListingIds.Contains(l.Id))
            .ToListAsync();

        if (offeredListings.Any(l => l.UserId != userId))
        {
            throw new UnauthorizedAccessException("You can only offer your own listings.");
        }

        var request = new ExchangeRequest
        {
            Id = Guid.NewGuid(),
            TargetListingId = dto.TargetListingId,
            RequesterId = userId,
            Status = ExchangeStatus.Requested,
            Price = dto.Price,
            Message = dto.Message,
            CreatedAt = DateTime.UtcNow,
            ExchangeRequestItems = offeredListings.Select(l => new ExchangeRequestItem
            {
                ExchangeRequestId = Guid.NewGuid(),
                OfferedListingId = l.Id
            }).ToList()
        };

        _context.ExchangeRequests.Add(request);

        var book = await _bookSearchApi.GetBookByIsbnAsync(targetListing.Isbn);
        var bookLabel = book?.Title ?? targetListing.Isbn;

        _context.Notifications.Add(new Notification
        {
            Id = Guid.NewGuid(),
            UserId = targetListing.UserId,
            Category = NotificationCategory.ExchangeRequested,
            Title = "New Exchange Request",
            Message = $"You received a new exchange request for \"{bookLabel}\".",
            RelatedListingId = targetListing.Id,
            RelatedExchangeRequestId = request.Id,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        return request;
    }

    public async Task<ExchangeRequest> GetExchangeRequestByIdAsync(Guid exchangeRequestId)
    {
        return await _context.ExchangeRequests
            .Include(e => e.TargetListing)
                .ThenInclude(l => l.User)
            .Include(e => e.Requester)
            .Include(e => e.ExchangeRequestItems)
                .ThenInclude(i => i.OfferedListing)
            .FirstOrDefaultAsync(e => e.Id == exchangeRequestId)
            ?? throw new KeyNotFoundException("Exchange request not found.");
    }

    public async Task<IEnumerable<ExchangeRequest>> GetSentExchangeRequestsAsync(Guid userId)
    {
        return await _context.ExchangeRequests
            .Include(e => e.TargetListing)
                .ThenInclude(l => l.User)
            .Include(e => e.Requester)
            .Include(e => e.ExchangeRequestItems)
                .ThenInclude(i => i.OfferedListing)
            .Where(e => e.RequesterId == userId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ExchangeRequest>> GetReceivedExchangeRequestsAsync(Guid userId)
    {
        return await _context.ExchangeRequests
            .Include(e => e.TargetListing)
                .ThenInclude(l => l.User)
            .Include(e => e.Requester)
            .Include(e => e.ExchangeRequestItems)
                .ThenInclude(i => i.OfferedListing)
            .Where(e => e.TargetListing.UserId == userId)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task AcceptExchangeRequestAsync(Guid exchangeRequestId, Guid userId)
    {
        var request = await _context.ExchangeRequests
            .Include(e => e.TargetListing)
            .Include(e => e.ExchangeRequestItems)
                .ThenInclude(i => i.OfferedListing)
            .FirstOrDefaultAsync(e => e.Id == exchangeRequestId)
            ?? throw new KeyNotFoundException("Exchange request not found.");

        if (request.TargetListing.UserId != userId)
        {
            throw new UnauthorizedAccessException("Only the listing owner can accept this request.");
        }

        if (request.Status != ExchangeStatus.Requested)
        {
            throw new InvalidOperationException("Only requested exchange requests can be accepted.");
        }

        request.Status = ExchangeStatus.Accepted;
        request.AcceptedAt = DateTime.UtcNow;

        var book = await _bookSearchApi.GetBookByIsbnAsync(request.TargetListing.Isbn);
        var bookLabel = book?.Title ?? request.TargetListing.Isbn;

        _context.Notifications.Add(new Notification
        {
            Id = Guid.NewGuid(),
            UserId = request.RequesterId,
            Category = NotificationCategory.ExchangeAccepted,
            Title = "Exchange Request Accepted",
            Message = $"A transaction has been created for \"{bookLabel}\". You can now arrange shipping.",
            RelatedListingId = request.TargetListingId,
            RelatedExchangeRequestId = request.Id,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        var transaction = await _transactionService.CreateTransactionFromExchangeRequestAsync(request);

        _context.Notifications.AddRange(
            new Notification
            {
                Id = Guid.NewGuid(),
                UserId = request.RequesterId,
                Category = NotificationCategory.TransactionUpdate,
                Title = "Transaction Created",
                Message = $"A transaction has been created for \"{bookLabel}\". You can now arrange shipping.",
                RelatedListingId = request.TargetListingId,
                RelatedExchangeRequestId = request.Id,
                RelatedTransactionId = transaction.Id,
                CreatedAt = DateTime.UtcNow
            },
            new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Category = NotificationCategory.TransactionUpdate,
                Title = "Transaction Created",
                Message = $"A transaction has been created for \"{bookLabel}\". You can now arrange shipping.",
                RelatedListingId = request.TargetListingId,
                RelatedExchangeRequestId = request.Id,
                RelatedTransactionId = transaction.Id,
                CreatedAt = DateTime.UtcNow
            }
        );

        await _context.SaveChangesAsync();
    }

    public async Task RejectExchangeRequestAsync(Guid exchangeRequestId, Guid userId)
    {
        var request = await _context.ExchangeRequests
            .Include(e => e.TargetListing)
            .FirstOrDefaultAsync(e => e.Id == exchangeRequestId)
            ?? throw new KeyNotFoundException("Exchange request not found.");

        if (request.TargetListing.UserId != userId)
        {
            throw new UnauthorizedAccessException("Only the listing owner can reject this request.");
        }

        if (request.Status != ExchangeStatus.Requested)
        {
            throw new InvalidOperationException("Only requested exchange requests can be rejected.");
        }

        request.Status = ExchangeStatus.Rejected;
        request.CancelledAt = DateTime.UtcNow;

        var book = await _bookSearchApi.GetBookByIsbnAsync(request.TargetListing.Isbn);
        var bookLabel = book?.Title ?? request.TargetListing.Isbn;

        _context.Notifications.Add(new Notification
        {
            Id = Guid.NewGuid(),
            UserId = request.RequesterId,
            Category = NotificationCategory.ExchangeRejected,
            Title = "Exchange Request Rejected",
            Message = $"Your exchange request for \"{bookLabel}\" was rejected.",
            RelatedListingId = request.TargetListingId,
            RelatedExchangeRequestId = request.Id,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }
}