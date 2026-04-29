using Book_Exchange.Models;

namespace Book_Exchange.Models.DTOs;

// TODO: make sure nothing changes when the ORM is done. This is the DTO for creating an exchange request, so it should be separate from the ExchangeRequest model.
public class CreateExchangeRequestDto
{
    public Guid TargetListingId { get; set; }
    // TODO: ExhangeRequest Model not yet defined, but this should be an enum of the type of exchange request (BuySell, BookSwap, BookSwapWithCash)
    // public ExchangeRequestType Type { get; set; }
    public List<Guid> OfferedListingIds { get; set; } = new();
    public decimal? CashAmount { get; set; }
}