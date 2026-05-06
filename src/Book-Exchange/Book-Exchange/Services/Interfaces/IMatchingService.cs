using Book_Exchange.Models;

namespace Book_Exchange.Services.Interfaces;

public interface IMatchingService
{
    Task<IEnumerable<WishlistItem>> FindMatchingWishlistItemsAsync(string isbn, Guid listingOwnerId);
    Task CreateMatchNotificationsAsync(Listing listing);
}