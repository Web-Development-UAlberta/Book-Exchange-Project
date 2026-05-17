using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Book;

namespace Book_Exchange.Models.DTOs.ExchangeRequest;

public class ExchangeRequestViewDto
{
    public Guid Id { get; set; }

    public Guid TargetListingId { get; set; }
    public string TargetIsbn { get; set; } = null!;
    public BookInfoDto? TargetBook { get; set; }

    public Guid RequesterId { get; set; }
    public string RequesterName { get; set; } = "Unknown";

    public Guid OwnerId { get; set; }
    public string OwnerName { get; set; } = "Unknown";

    public ExchangeStatus Status { get; set; }

    public decimal? Price { get; set; }
    public string? Message { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? CancelledAt { get; set; }

    public List<string> OfferedIsbns { get; set; } = new();
    public List<BookInfoDto> OfferedBooks { get; set; } = new();
}