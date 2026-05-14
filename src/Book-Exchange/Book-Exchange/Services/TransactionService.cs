using System.Runtime.CompilerServices;
using Book_Exchange.Data;
using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Transaction;
using Book_Exchange.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Book_Exchange.Services;

public class TransactionService : ITransactionService
{
    private readonly ApplicationDbContext _context;
    private readonly IBookSearchApi _bookSearchApi;

    public TransactionService(ApplicationDbContext context, IBookSearchApi bookSearchApi)
    {
        _context = context;
        _bookSearchApi = bookSearchApi;
    }

    // helper to get book title
    private async Task<string> GetTitleAsync(string isbn)
    {
        var results = await _bookSearchApi.SearchBooksAsync(isbn, 1);
        return results.FirstOrDefault()?.Title ?? isbn;
    }

    /// <summary>
    /// GetTransactionByIdAsync
    /// - Loads the transaction with all related data for the ViewModel
    /// - Throws KeyNotFoundException if the transaction does not exist
    /// - CurrentUser is used to determind HasReview for the transaction
    /// </summary>
    /// <param name="transactionId"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public async Task<TransactionViewModel> GetTransactionByIdAsync(Guid transactionId, Guid currentUserId)
    {
        var transaction = await _context.Transactions
            .Include(t => t.ExchangeRequest)
                .ThenInclude(er => er.Requester)
            .Include(t => t.ExchangeRequest)
                .ThenInclude(er => er.TargetListing)
                    .ThenInclude(l => l.User)
            .Include(t => t.ExchangeRequest)
                .ThenInclude(er => er.ExchangeRequestItems)
                    .ThenInclude(l => l.OfferedListing)
            .Include(t => t.StatusHistory)
            .Include(t => t.Shipments)
            .Include(t => t.Reviews)
            .FirstOrDefaultAsync(t => t.Id == transactionId)
            ?? throw new KeyNotFoundException($"Transaction {transactionId} not found.");

        return await MapToViewModelAsync(transaction, currentUserId);
    }

    /// <summary>
    /// GetTransactionsByUserIdAsync
    /// - Returns all transactions where the user is a participant
    /// - Participant means they were either side of the accepted exchange request
    /// - Returns empty list if user has no transactions
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<IEnumerable<TransactionViewModel>> GetTransactionsByUserIdAsync(Guid userId)
    {
        var transactions = await _context.Transactions
            .Include(t => t.ExchangeRequest)
                .ThenInclude(er => er.Requester)
            .Include(t => t.ExchangeRequest)
                .ThenInclude(er => er.TargetListing)
                    .ThenInclude(l => l.User)
            .Include(t => t.ExchangeRequest)
                .ThenInclude(er => er.ExchangeRequestItems)
                    .ThenInclude(l => l.OfferedListing)
            .Include(t => t.StatusHistory)
            .Include(t => t.Shipments)
            .Include(t => t.Reviews)
            .Where(t =>
                t.ExchangeRequest.RequesterId == userId ||
                t.ExchangeRequest.TargetListing.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        var viewModels = await Task.WhenAll(
                transactions.Select(t => MapToViewModelAsync(t, userId)));
        return viewModels;
    }

    /// <summary>
    /// CreateTransactionFromExchangeRequestAsync
    /// - Only creates a transaction if the exchange request is in Accepted status
    /// - Sets transaction status to Confirmed
    /// - Sets CreatedAt timestamp
    /// </summary>
    /// <param name="exchangeRequest"></param>
    /// <returns></returns>
    public async Task<Transaction> CreateTransactionFromExchangeRequestAsync(ExchangeRequest exchangeRequest)
    {
        if (exchangeRequest.Status != ExchangeStatus.Accepted)
            throw new InvalidOperationException("Only accepted exchange requests can create a transaction.");

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            ExchangeRequestId = exchangeRequest.Id,
            TotalValue = exchangeRequest.Price,
            CreatedAt = DateTime.UtcNow
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        var initialStatus = new TransactionStatusHistory
        {
            Id = Guid.NewGuid(),
            TransactionId = transaction.Id,
            Status = TransactionStatus.Confirmed,
            UpdatedByUserId = exchangeRequest.RequesterId,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Add(initialStatus);
        await _context.SaveChangesAsync();

        return transaction;
    }

    /// <summary>
    /// MarkAsShippedAsync
    /// - Status must be Confirmed before it can be marked as Shipped
    /// - Only the user who is shipping can mark as shipped
    /// </summary>
    /// <param name="transactionId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task MarkAsShippedAsync(Guid transactionId, Guid userId)
    {
        var transaction = await GetTransactionWithStatusAsync(transactionId);

        var currentStatus = GetCurrentStatus(transaction);

        if (currentStatus != TransactionStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed transactions can be marked as shipped.");

        EnsureParticipant(transaction, userId);

        _context.Add(new TransactionStatusHistory
        {
            Id = Guid.NewGuid(),
            TransactionId = transaction.Id,
            Status = TransactionStatus.Shipped,
            UpdatedByUserId = userId,
            UpdatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// CompleteTransactionAsync
    /// - Status must be Confirmed before it can be Completed
    /// - Sets CompletedAt timestamp
    /// - Marks all related listings as no longer Active
    /// - Only a participant in the transaction can complete
    /// </summary>
    /// <param name="transactionId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task CompleteTransactionAsync(Guid transactionId, Guid userId)
    {
        var transaction = await GetTransactionWithStatusAsync(transactionId);

        var currentStatus = GetCurrentStatus(transaction);

        if (currentStatus != TransactionStatus.Shipped)
            throw new InvalidOperationException("Only shipped transactions can be marked as complete.");

        EnsureParticipant(transaction, userId);

        transaction.CompletedAt = DateTime.UtcNow;

        _context.Add(new TransactionStatusHistory
        {
            Id = Guid.NewGuid(),
            TransactionId = transaction.Id,
            Status = TransactionStatus.Completed,
            UpdatedByUserId = userId,
            UpdatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// CancelTransactionAsync
    /// - Cannot cancel a transaction that is already Completed
    /// - Sets CancelledAt timestamp
    /// - Only a participant in the transaction can cancel
    /// </summary>
    /// <param name="transactionId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task CancelTransactionAsync(Guid transactionId, Guid userId)
    {
        var transaction = await GetTransactionWithStatusAsync(transactionId);

        var currentStatus = GetCurrentStatus(transaction);

        if (currentStatus != TransactionStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed transactions can be cancelled.");

        EnsureParticipant(transaction, userId);

        transaction.CancelledAt = DateTime.UtcNow;

        _context.Add(new TransactionStatusHistory
        {
            Id = Guid.NewGuid(),
            TransactionId = transaction.Id,
            Status = TransactionStatus.Cancelled,
            UpdatedByUserId = userId,
            UpdatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }

    /// <summary>
    ///  DisputeTransactionAsync
    /// - Cannot dispute a transaction that is already Completed
    /// - Sets DisputedAt timestamp
    /// - Only a participant in the transaction can dispute
    /// </summary>
    /// <param name="transactionId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>  
    public async Task DisputeTransactionAsync(Guid transactionId, Guid userId)
    {
        var transaction = await GetTransactionWithStatusAsync(transactionId);

        var currentStatus = GetCurrentStatus(transaction);

        if (currentStatus != TransactionStatus.Shipped)
            throw new InvalidOperationException("Only shipped transactions can be disputed.");

        EnsureParticipant(transaction, userId);

        _context.Add(new TransactionStatusHistory
        {
            Id = Guid.NewGuid(),
            TransactionId = transaction.Id,
            Status = TransactionStatus.Disputed,
            UpdatedByUserId = userId,
            UpdatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }

    // Helper methods for status checks, participant checks, and mapping to view models

    /// <summary>
    /// GetTransactionWithStatusAsync
    /// - Loads the transaction with its status history for status checks
    /// </summary>
    /// <param name="transactionId"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    private async Task<Transaction> GetTransactionWithStatusAsync(Guid transactionId)
    {
        return await _context.Transactions
            .Include(t => t.ExchangeRequest)
                .ThenInclude(er => er.TargetListing)
            .Include(t => t.StatusHistory)
            .FirstOrDefaultAsync(t => t.Id == transactionId)
                ?? throw new KeyNotFoundException($"Transaction {transactionId} not found.");
    }

    /// <summary>
    ///  GetCurrentStatus
    /// Resolves current status from StatusHistory — single source of truth
    /// </summary>
    /// <param name="transaction"></param>
    /// <returns></returns>
    private static TransactionStatus GetCurrentStatus(Transaction transaction)
    {
        return transaction.StatusHistory
            .OrderByDescending(h => h.UpdatedAt)
            .FirstOrDefault()?.Status ?? TransactionStatus.Confirmed;
    }

    /// <summary>
    /// EnsureParticipant
    /// - Checks if the user is either the requester or the listing owner in the exchange request
    /// - Throws UnauthorizedAccessException if the user is not a participant in the transaction
    /// </summary>
    /// <param name="transaction"></param>
    /// <param name="userId"></param>
    /// <exception cref="UnauthorizedAccessException"></exception>
    private static void EnsureParticipant(Transaction transaction, Guid userId)
    {
        var requesterId = transaction.ExchangeRequest.RequesterId;
        var listingOwnerId = transaction.ExchangeRequest.TargetListing.UserId;

        if (userId != requesterId && userId != listingOwnerId)
            throw new UnauthorizedAccessException("You are not a participant in this transaction.");
    }

    /// <summary>
    /// Maps a Transaction to a TransactionViewModel
    /// Description is derived from the exchange request type:
    ///   - No offered items + price present  → Sell: {ISBN}
    ///   - Offered items + no price          → Swap: {ISBN} ↔ {offered ISBNs}
    ///   - Offered items + price             → Swap + Cash: {ISBN} ↔ {offered ISBNs}
    /// </summary>
    /// <param name="transaction"></param>
    /// <param name="currentUserId"></param>
    /// <returns></returns> 
    private async Task<TransactionViewModel> MapToViewModelAsync(Transaction transaction, Guid currentUserId)
    {
        var er = transaction.ExchangeRequest;
        var currentStatus = GetCurrentStatus(transaction);

        var isActive = currentStatus is
            TransactionStatus.Confirmed or
            TransactionStatus.Shipped or
            TransactionStatus.Disputed;

        var withUser = currentUserId == er.RequesterId
            ? er.TargetListing.User.UserName ?? "Unknown"
            : er.Requester.UserName ?? "Unknown";

        var targetTitle = await GetTitleAsync(er.TargetListing.Isbn);
        var offeredItems = er.ExchangeRequestItems.ToList();

        string description;
        if (!offeredItems.Any() && er.Price.HasValue)
        {
            description = $"Sell: {targetTitle}";
        }
        else if (offeredItems.Any() && !er.Price.HasValue)
        {
            var offeredTitles = await Task.WhenAll(
                offeredItems.Select(i => GetTitleAsync(i.OfferedListing.Isbn)));
            description = $"Swap: {targetTitle} \u2194 {string.Join(", ", offeredTitles)}";
        }
        else
        {
            var offeredTitles = await Task.WhenAll(
                offeredItems.Select(i => GetTitleAsync(i.OfferedListing.Isbn)));
            description = $"Swap + Cash: {targetTitle} \u2194 {string.Join(", ", offeredTitles)}";
        }

        // Get the most recent non-cancelled shipment ID if one exists
        var shipmentId = transaction.Shipments
            .Where(s => s.Status != ShipmentStatus.Cancelled)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefault()?.Id;

        // HasReview is per-user — only show Leave Review if they haven't reviewed yet
        var hasReview = transaction.Reviews.Any(r => r.ReviewerId == currentUserId);

        return new TransactionViewModel
        {
            Id = transaction.Id,
            Description = description,
            WithUserName = withUser,
            TotalValue = transaction.TotalValue,
            CreatedAt = transaction.CreatedAt,
            CurrentStatus = currentStatus,
            IsActive = isActive,
            ShipmentId = shipmentId,
            HasReview = hasReview
        };
    }
}