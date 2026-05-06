using Book_Exchange.Data;
using Book_Exchange.Models;
using Book_Exchange.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Book_Exchange.Services;

public class MatchingService : IMatchingService
{
    private readonly ApplicationDbContext _context;

    public MatchingService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<WishlistItem>> FindMatchingWishlistItemsAsync(
        string isbn,
        Guid listingOwnerId)
    {
        isbn = isbn.Trim().Replace("-", "");

        return await _context.Wishlist
            .Where(w =>
                w.IsActive &&
                w.Isbn == isbn &&
                w.UserId != listingOwnerId)
            .ToListAsync();
    }

    public async Task CreateMatchNotificationsAsync(Listing listing)
    {
        var matches = await FindMatchingWishlistItemsAsync(
            listing.Isbn,
            listing.UserId);

        foreach (var match in matches)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = match.UserId,
                Category = NotificationCategory.MatchFound,
                Title = "Book Match Found",
                Message = $"A book from your wishlist is now available. ISBN: {listing.Isbn}",
                RelatedListingId = listing.Id,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
        }

        await _context.SaveChangesAsync();
    }
}