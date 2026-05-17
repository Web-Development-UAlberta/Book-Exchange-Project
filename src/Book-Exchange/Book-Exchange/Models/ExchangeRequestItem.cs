namespace Book_Exchange.Models;

public class ExchangeRequestItem
{
    public Guid ExchangeRequestId { get; set; }
    public ExchangeRequest ExchangeRequest { get; set; } = null!;

    public Guid OfferedListingId { get; set; }
    public Listing OfferedListing { get; set; } = null!;
}