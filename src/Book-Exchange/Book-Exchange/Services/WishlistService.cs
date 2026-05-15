using System.Text.RegularExpressions;
using Book_Exchange.Data;
using Book_Exchange.Models;
using Book_Exchange.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Book_Exchange.Models.DTOs.Notification;

namespace Book_Exchange.Services;

public class WishlistService : IWishlistService
{
    private readonly ApplicationDbContext _context;
    private readonly INotificationService _notificationService;

    public WishlistService(ApplicationDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<WishlistItem> GetWishlistItemByIdAsync(Guid wishlistItemId, Guid userId)
    {
        return await _context.Wishlist
            .FirstOrDefaultAsync(w => w.Id == wishlistItemId && w.UserId == userId)
            ?? throw new KeyNotFoundException("Wishlist item not found.");
    }

    public async Task<IEnumerable<WishlistItem>> GetWishlistByUserIdAsync(Guid userId)
    {
        return await _context.Wishlist
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.IsActive)
            .ThenBy(w => w.Isbn)
            .ToListAsync();
    }

    public async Task<WishlistItem> AddWishlistItemAsync(Guid userId, string isbn)
    {
        isbn = NormalizeIsbn(isbn);

        if (!IsValidIsbn(isbn))
        {
            throw new ArgumentException("ISBN must be a valid ISBN 10 or ISBN 13.");
        }

        var existing = await _context.Wishlist
            .FirstOrDefaultAsync(w => w.UserId == userId && w.Isbn == isbn);

        if (existing != null)
        {
            existing.IsActive = true;
            await _context.SaveChangesAsync();
            await CreateWishlistMatchNotificationsAsync(existing);
            return existing;
        }

        var item = new WishlistItem
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Isbn = isbn,
            IsActive = true
        };

        _context.Wishlist.Add(item);
        await _context.SaveChangesAsync();
        await CreateWishlistMatchNotificationsAsync(item);
        return item;
    }

    private async Task CreateWishlistMatchNotificationsAsync(WishlistItem item)
    {
        var matchingListings = await _context.Listings
            .Where(l =>
                l.Isbn == item.Isbn &&
                l.UserId != item.UserId)
            .ToListAsync();

        foreach (var listing in matchingListings)
        {
            var alreadyExists = await _context.Notifications.AnyAsync(n =>
                n.UserId == item.UserId &&
                n.RelatedListingId == listing.Id &&
                n.Category == NotificationCategory.MatchFound &&
                !n.IsRead);

            if (alreadyExists)
            {
                continue;
            }

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = item.UserId,
                Category = NotificationCategory.MatchFound,
                Title = "Wishlist Match Found",
                Message = $"A book from your wishlist is available now. ISBN: {item.Isbn}",
                RelatedListingId = listing.Id,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.Notifications.Add(notification);
        }

        await _context.SaveChangesAsync();
    }

    public async Task RemoveWishlistItemAsync(Guid wishlistItemId, Guid userId)
    {
        var item = await GetWishlistItemByIdAsync(wishlistItemId, userId);

        item.IsActive = false;

        await _context.SaveChangesAsync();
    }

    public async Task RestoreWishlistItemAsync(Guid wishlistItemId, Guid userId)
    {
        var item = await GetWishlistItemByIdAsync(wishlistItemId, userId);

        item.IsActive = true;

        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Listing>> GetMatchingListingsAsync(Guid userId)
    {
        var activeIsbns = await _context.Wishlist
            .Where(w => w.UserId == userId && w.IsActive)
            .Select(w => w.Isbn)
            .ToListAsync();

        return await _context.Listings
            .Include(l => l.User)
            .Where(l => activeIsbns.Contains(l.Isbn) && l.UserId != userId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Listing>> GetMatchingListingsForItemAsync(Guid wishlistItemId, Guid userId)
    {
        var item = await GetWishlistItemByIdAsync(wishlistItemId, userId);

        return await _context.Listings
            .Include(l => l.User)
            .Where(l => l.Isbn == item.Isbn && l.UserId != userId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    private static string NormalizeIsbn(string isbn)
    {
        return isbn.Trim().Replace("-", "").Replace(" ", "").ToUpper();
    }

    private static bool IsValidIsbn(string isbn)
    {
        return Regex.IsMatch(isbn, @"^([0-9]{13}|[0-9X]{10})$");
    }

    public async Task RequestNotificationAsync(Guid wishlistItemId, Guid userId)
    {
        var item = await GetWishlistItemByIdAsync(wishlistItemId, userId);

        // Avoid duplicate pending notification requests for the same ISBN
        var alreadyRequested = await _context.Notifications.AnyAsync(n =>
            n.UserId == userId &&
            n.Category == NotificationCategory.WishlistAvailable &&
            n.Message.Contains(item.Isbn) &&
            !n.IsRead);

        if (alreadyRequested) return;

        await _notificationService.CreateNotificationAsync(new CreateNotificationDto
        {
            UserId = userId,
            Category = NotificationCategory.WishlistAvailable,
            Title = "Wishlist Notification Requested",
            Message = $"You'll be notified when a listing appears for ISBN: {item.Isbn}"
        });
    }
}