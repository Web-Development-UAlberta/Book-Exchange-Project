using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Wishlist;

namespace Book_Exchange.Services.Interfaces;

// TODO: Once ORM is implemented make sure nothing changes. 
public interface IWishlistService
{
    Task<WishlistItem> GetWishlistItemByIdAsync(Guid wishlistItemId, Guid userId);
    Task<IEnumerable<WishlistItem>> GetWishlistByUserIdAsync(Guid userId);
    Task<WishlistItem> AddWishlistItemAsync(Guid userId, string isbn);
    Task RemoveWishlistItemAsync(Guid wishlistItemId, Guid userId);
    Task RestoreWishlistItemAsync(Guid wishlistItemId, Guid userId);
    Task<IEnumerable<Listing>> GetMatchingListingsAsync(Guid userId);
    Task<IEnumerable<Listing>> GetMatchingListingsForItemAsync(Guid wishlistItemId, Guid userId);
}