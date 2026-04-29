using Book_Exchange.Models;
using Book_Exchange.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Book_Exchange.Services;

public class TransactionService : ITransactionService
{
    // TODO: Implement once ORM is set up and database context is available.

    // private readonly ApplicationDbContext _context;

    // GetTransactionByIdAsync
    // - Returns the transaction if it exists
    // - Throws KeyNotFoundException if the transaction does not exist
    // - User must be a participant in the transaction to view it
    public Task<Transaction> GetTransactionByIdAsync(Guid transactionId)
        => throw new NotImplementedException();

    // GetTransactionsByUserIdAsync
    // - Returns all transactions where the user is a participant
    // - Participant means they were either side of the accepted exchange request
    // - Returns empty list if user has no transactions
    public Task<IEnumerable<Transaction>> GetTransactionsByUserIdAsync(Guid userId)
        => throw new NotImplementedException();

    // ConfirmTransactionAsync
    // - Status must be Confirmed to proceed
    // - Sets ConfirmedAt timestamp
    // - Only a participant in the transaction can confirm
    public Task ConfirmTransactionAsync(Guid transactionId, Guid userId)
        => throw new NotImplementedException();

    // CompleteTransactionAsync
    // - Status must be Confirmed before it can be Completed
    // - Sets CompletedAt timestamp
    // - Marks all related listings as no longer Active
    // - Only a participant in the transaction can complete
    public Task CompleteTransactionAsync(Guid transactionId, Guid userId)
        => throw new NotImplementedException();

    // CancelTransactionAsync
    // - Cannot cancel a transaction that is already Completed
    // - Sets CancelledAt timestamp
    // - Only a participant in the transaction can cancel
    public Task CancelTransactionAsync(Guid transactionId, Guid userId)
        => throw new NotImplementedException();
}