using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.ExchangeRequest;

namespace Book_Exchange.Services.Interfaces;

public interface IExchangeRequestService
{
    Task<ExchangeRequest> CreateExchangeRequestAsync(CreateExchangeRequestDto dto, Guid userId);
    Task<ExchangeRequest> GetExchangeRequestByIdAsync(Guid exchangeRequestId);
    Task<IEnumerable<ExchangeRequest>> GetSentExchangeRequestsAsync(Guid userId);
    Task<IEnumerable<ExchangeRequest>> GetReceivedExchangeRequestsAsync(Guid userId);
    Task AcceptExchangeRequestAsync(Guid exchangeRequestId, Guid userId);
    Task RejectExchangeRequestAsync(Guid exchangeRequestId, Guid userId);
}