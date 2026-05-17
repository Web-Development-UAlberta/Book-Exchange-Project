using System.ComponentModel.DataAnnotations;

namespace Book_Exchange.Models.DTOs.ExchangeRequest;

public class CreateExchangeRequestDto
{
    [Required]
    public Guid TargetListingId { get; set; }

    public List<Guid> OfferedListingIds { get; set; } = new();

    [Range(0, 10000)]
    public decimal? Price { get; set; }

    [StringLength(1000)]
    public string? Message { get; set; }
}