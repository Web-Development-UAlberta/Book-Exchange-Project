using Book_Exchange.Models;

namespace Book_Exchange.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<Transaction> GetTransactionByIdAsync(Guid transactionId);
        Task<IEnumerable<Transaction>> GetTransactionsByUserIdAsync(Guid userId);
        Task<Transaction> CreateTransactionFromExchangeRequestAsync(ExchangeRequest exchangeRequest);
        Task MarkAsShippedAsync(Guid transactionId, Guid userId);
        Task CompleteTransactionAsync(Guid transactionId, Guid userId);
        Task CancelTransactionAsync(Guid transactionId, Guid userId);
        Task DisputeTransactionAsync(Guid transactionId, Guid userId);
    }
}