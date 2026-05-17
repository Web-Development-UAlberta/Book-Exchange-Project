namespace Book_Exchange.Models.DTOs.Wishlist;

using Book_Exchange.Models.DTOs.Book;
using Book_Exchange.Models;

public class WishListMatchingViewModel
{
    public Listing Listing { get; set; } = null!;
    public BookInfoDto? Book { get; set; }
}
