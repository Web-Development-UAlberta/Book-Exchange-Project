using Book_Exchange.Models;
using Book_Exchange.Models.DTOs;

namespace Book_Exchange.Services.Interfaces;
// TODO: Once ORM is implemented make sure nothing changes. 
public interface IExchangeRequestService
{
    Task<ExchangeRequest> CreateExchangeRequestAsync(CreateExchangeRequestDto dto, Guid userId);
    Task<ExchangeRequest> GetExchangeRequestByIdAsync(Guid exchangeRequestId);
    Task<IEnumerable<ExchangeRequest>> GetSentExchangeRequestsAsync(Guid userId);
    Task<IEnumerable<ExchangeRequest>> GetReceivedExchangeRequestsAsync(Guid userId);
    Task AcceptExchangeRequestAsync(Guid exchangeRequestId, Guid userId);
    Task RejectExchangeRequestAsync(Guid exchangeRequestId, Guid userId);
}