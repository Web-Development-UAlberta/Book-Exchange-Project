using Book_Exchange.Models;
using Book_Exchange.Models.DTOs;
using Book_Exchange.Services.Interfaces;

namespace Book_Exchange.Services;

// public class ExchangeRequestService : IExchangeRequestService
// {
// TODO: Implement the methods defined in IExchangeRequestService once ORM is set up and database context is available.
// TODO: ExchangeRequest Model not yet defined
// private readonly ApplicationDbContext _context;

// CreateExchangeRequestAsync
// - TargetListingId must reference a valid existing listing
// - A user cannot create an exchange request against their own listing
// - Type must be one of: BuySell, BookSwap, BookSwapWithCash
// - OfferedListingIds must be empty for BuySell
// - OfferedListingIds must have 1-3 items for BookSwap and BookSwapWithCash
// - OfferedListingIds must reference valid existing listings owned by the requester
// - CashAmount is only used for BookSwapWithCash, ignored otherwise
// - UserId is taken from the logged in user, not from a form
// public Task<ExchangeRequest> CreateExchangeRequestAsync(CreateExchangeRequestDto dto, Guid userId)
// {
//     throw new NotImplementedException();
// }

// GetExchangeRequestByIdAsync
// - Returns the exchange request if it exists
// - Throws KeyNotFoundException if the exchange request does not exist
// public Task<ExchangeRequest> GetExchangeRequestByIdAsync(Guid exchangeRequestId)
// {
//     throw new NotImplementedException();
// }

// GetSentExchangeRequestsAsync
// - Returns all exchange requests sent by the specified user
// - Returns an empty list if the user has not sent any exchange requests
// public Task<IEnumerable<ExchangeRequest>> GetSentExchangeRequestsAsync(Guid userId)
// {
//     throw new NotImplementedException();
// }

// GetReceivedExchangeRequestsAsync
// - Returns all exchange requests received on listings owned by the specified user
// - Returns an empty list if the user has not received any exchange requests
// public Task<IEnumerable<ExchangeRequest>> GetReceivedExchangeRequestsAsync(Guid userId)
// {
//     throw new NotImplementedException();
// }

// AcceptExchangeRequestAsync
// - Only the owner of the target listing can accept
// - Exchange request must be in Pending status to accept
// - Accepting triggers transaction creation
// public Task AcceptExchangeRequestAsync(Guid exchangeRequestId, Guid userId)
// {
//     throw new NotImplementedException();
// }

// RejectExchangeRequestAsync
// - Only the owner of the target listing can reject
// - Exchange request must be in Pending status to reject
// public Task RejectExchangeRequestAsync(Guid exchangeRequestId, Guid userId)
// {
//     throw new NotImplementedException();
// }
// }