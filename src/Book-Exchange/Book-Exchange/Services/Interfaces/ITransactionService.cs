using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Transaction;

namespace Book_Exchange.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionViewModel> GetTransactionByIdAsync(Guid transactionId, Guid currentUserId);
        Task<IEnumerable<TransactionViewModel>> GetTransactionsByUserIdAsync(Guid userId);
        Task<Transaction> CreateTransactionFromExchangeRequestAsync(ExchangeRequest exchangeRequest);
        Task MarkAsShippedAsync(Guid transactionId, Guid userId);
        Task CompleteTransactionAsync(Guid transactionId, Guid userId);
        Task CancelTransactionAsync(Guid transactionId, Guid userId);
        Task DisputeTransactionAsync(Guid transactionId, Guid userId);
    }
}