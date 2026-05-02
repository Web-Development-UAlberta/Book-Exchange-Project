using Book_Exchange.Models;
using Book_Exchange.Services.Interfaces;
using Book_Exchange.Models.DTOs.ExchangeRequest;

namespace Book_Exchange.Services;

// public class ExchangeRequestService : IExchangeRequestService
// {
// TODO: Implement the methods defined in IExchangeRequestService once ORM is set up and database context is available.
// TODO: Rework this service based on the Exchange models
// private readonly ApplicationDbContext _context;

// CreateExchangeRequestAsync
// - TargetListingId must reference a valid existing listing
// - A user cannot create an exchange request against their own listing
// - Exchange type is derived from the submitted data (no explicit Type field):
//     - No OfferedListingIds + CashAmount present  → Buy/Sell
//     - OfferedListingIds present, no CashAmount   → Swap
//     - OfferedListingIds present + CashAmount     → Swap with cash
// - OfferedListingIds must be empty for a Buy/Sell request
// - OfferedListingIds must contain 1–3 items for a Swap or Swap with cash request
// - OfferedListingIds must reference valid existing listings owned by the requester
// - CashAmount must be null for a books-only Swap
// - CashAmount must be provided and > 0 for Buy/Sell and Swap with cash
// - UserId is taken from the logged-in user, not from the form
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
// - Exchange request must be in Requested status to accept
// - Accepting triggers transaction creation via ITransactionService
// - Listing status is no longer updated here (ListingStatus removed; state is derived)
// public Task AcceptExchangeRequestAsync(Guid exchangeRequestId, Guid userId)
// {
//     throw new NotImplementedException();
// }

// RejectExchangeRequestAsync
// - Only the owner of the target listing can reject
// - Exchange request must be in Requested status to reject
// public Task RejectExchangeRequestAsync(Guid exchangeRequestId, Guid userId)
// {
//     throw new NotImplementedException();
// }
// }