namespace Book_Exchange.Models.DTOs.Wishlist;

public class WishlistIndexViewDto
{
    public string? SearchText { get; set; }

    public bool AvailableOnly { get; set; }

    public List<WishlistItemViewDto> Items { get; set; } = new();
}