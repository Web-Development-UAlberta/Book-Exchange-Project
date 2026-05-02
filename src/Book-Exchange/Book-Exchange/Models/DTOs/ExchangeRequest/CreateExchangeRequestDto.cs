using Book_Exchange.Models;
using System.ComponentModel.DataAnnotations;

namespace Book_Exchange.Models.DTOs.ExchangeRequest;

// TODO: make sure nothing changes when the ORM is done. This is the DTO for creating an exchange request, so it should be separate from the ExchangeRequest model.
public class CreateExchangeRequestDto
{
    [Required(ErrorMessage = "Target listing ID is required.")]
    public Guid TargetListingId { get; set; }

    [Required(ErrorMessage = "Exchange type is required.")]
    public ExchangeType Type { get; set; }

    // Only populated for BookSwap and BookSwapWithCash — max 3 offered books per scope rules
    [MaxLength(3, ErrorMessage = "You may offer a maximum of 3 books.")]
    public List<Guid> OfferedListingIds { get; set; } = new();

    // Only populated for BuySell and BookSwapWithCash
    [Range(0.01, double.MaxValue, ErrorMessage = "Cash amount must be greater than zero.")]
    public decimal? CashAmount { get; set; }
}