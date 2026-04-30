namespace Book_Exchange.Models;

public class WishlistItemViewModel
{
    public Guid Id { get; set; }
    public string Isbn { get; set; } = null!;
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? CoverImageUrl { get; set; }
    public bool IsActive { get; set; }
    public bool IsAvailable { get; set; }
    public int MatchingListingCount { get; set; }
}