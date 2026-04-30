using Book_Exchange.Models;
using Book_Exchange.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Book_Exchange.Services;

public class WishlistService : IWishlistService
{
    // TODO: Implement once ORM is set up and database context is available.
    // private readonly ApplicationDbContext _context;

    // public WishlistService(ApplicationDbContext context)
    // {
    //     _context = context;
    // }

    //GetWishlistItembyIdAsync
    // - Returns the wishlist item if it exists
    // - Throws KeyNotFoundException if the wishlist item does not exist
    // - UserId must match the item's UserId (users cannot view each other's wishlist items directly)
    public Task<WishlistItem> GetWishlistItemByIdAsync(Guid wishlistItemId, Guid userId)
        => throw new NotImplementedException();

    // GetWishlistByUserIdAsync
    // - Returns all wishlist items belonging to the user
    // - Returns empty list if the user has no wishlist items
    public Task<IEnumerable<WishlistItem>> GetWishlistByUserIdAsync(Guid userId)
        => throw new NotImplementedException();

    // AddWishlistItemAsync
    // - ISBN must not be null or whitespace
    // - Throws InvalidOperationException if the ISBN is already on the user's wishlist (active or inactive)
    // - Creates the item with IsActive = true
    public Task<WishlistItem> AddWishlistItemAsync(Guid userId, string isbn)
        => throw new NotImplementedException();

    // RemoveWishlistItemAsync
    // - Throws KeyNotFoundException if the item does not exist or does not belong to the user
    // - Soft deletes the item by setting IsActive = false
    public Task RemoveWishlistItemAsync(Guid wishlistItemId, Guid userId)
        => throw new NotImplementedException();

    // RestoreWishlistItemAsync
    // - Throws KeyNotFoundException if the item does not exist or does not belong to the user
    // - Reactivates a previously removed item by setting IsActive = true
    public Task RestoreWishlistItemAsync(Guid wishlistItemId, Guid userId)
        => throw new NotImplementedException();

    // GetMatchingListingsAsync
    // - Returns active listings whose ISBN matches any of the user's active wishlist ISBNs
    // - Excludes listings owned by the user themselves
    // - Excludes listings that are not in Active status
    // - Returns empty list if there are no matches
    public Task<IEnumerable<Listing>> GetMatchingListingsAsync(Guid userId)
        => throw new NotImplementedException();

    // GetMatchingListingsForItemAsync
    // - Throws KeyNotFoundException if the wishlist item does not exist or does not belong to the user
    // - Returns empty list if the wishlist item is not active
    // - Returns active listings whose ISBN matches the wishlist item's ISBN
    // - Excludes listings owned by the user themselves
    public Task<IEnumerable<Listing>> GetMatchingListingsForItemAsync(Guid wishlistItemId, Guid userId)
        => throw new NotImplementedException();
}
