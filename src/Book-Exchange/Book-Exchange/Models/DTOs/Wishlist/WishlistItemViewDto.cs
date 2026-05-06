using Book_Exchange.Models.DTOs.Book;

namespace Book_Exchange.Models.DTOs.Wishlist;

public class WishlistItemViewDto
{
    public Guid Id { get; set; }

    public string Isbn { get; set; } = null!;

    public bool IsActive { get; set; }

    public BookInfoDto? Book { get; set; }

    public bool HasMatchingListing { get; set; }
}