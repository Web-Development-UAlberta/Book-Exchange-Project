using System.Security.Claims;
using Book_Exchange.Data;
using Book_Exchange.Models.ViewModels;
using Book_Exchange.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Book_Exchange.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IBookSearchApi _bookSearchApi;

    public ProfileController(
        ApplicationDbContext context,
        IBookSearchApi bookSearchApi)
    {
        _context = context;
        _bookSearchApi = bookSearchApi;
    }

    // ── GET /Profile
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var vm = await BuildProfileViewModelAsync(userId, isOwnProfile: true);

        if (vm is null)
            return NotFound();

        return View(vm);
    }

    // ── GET /Profile/{id}
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> View(Guid id)
    {
        var currentUserId = User.Identity?.IsAuthenticated == true
            ? Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)
            : Guid.Empty;

        var vm = await BuildProfileViewModelAsync(id, isOwnProfile: currentUserId == id);

        if (vm is null)
            return NotFound();

        return View("Index", vm);
    }

    private async Task<ProfileViewModel?> BuildProfileViewModelAsync(Guid userId, bool isOwnProfile)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            return null;

        var listings = await _context.Listings
            .AsNoTracking()
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();

        var reviewsReceived = await _context.Reviews
            .AsNoTracking()
            .Where(r => r.ReviewerId != userId &&
                        r.Transaction.ExchangeRequest.TargetListing.UserId == userId ||
                        r.ReviewerId != userId &&
                        r.Transaction.ExchangeRequest.RequesterId == userId)
            .Include(r => r.Reviewer)
            .OrderByDescending(r => r.CreatedAt)
            .Take(10)
            .ToListAsync();

        var tradeCount = await _context.Transactions
            .AsNoTracking()
            .CountAsync(t =>
                t.ExchangeRequest.RequesterId == userId ||
                t.ExchangeRequest.TargetListing.UserId == userId);

        double averageRating = reviewsReceived.Count > 0
            ? Math.Round(reviewsReceived.Average(r => (double)r.Rating), 1)
            : 0.0;

        var listingItems = new List<ProfileListingItem>();
        foreach (var listing in listings)
        {
            var book = await _bookSearchApi.GetBookByIsbnAsync(listing.Isbn);
            listingItems.Add(new ProfileListingItem
            {
                Id = listing.Id,
                Isbn = listing.Isbn,
                BookTitle = book?.Title ?? listing.Isbn,
                Condition = listing.Condition,
                Price = listing.Price
            });
        }

        // MemberSince: ASP.NET Identity doesn't store CreatedAt by default.
        // Derive it from the oldest listing / transaction.
        var oldestActivity = listings.MinBy(l => l.CreatedAt)?.CreatedAt ?? DateTime.UtcNow;

        return new ProfileViewModel
        {
            Id = user.Id,
            UserName = user.UserName ?? "Unknown",
            MemberSince = oldestActivity,
            AverageRating = averageRating,
            TradeCount = tradeCount,
            ListingCount = listings.Count,
            Reviews = reviewsReceived.Select(r => new ProfileReviewItem
            {
                ReviewerUserName = r.Reviewer.UserName ?? "User",
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            }).ToList(),
            Listings = listingItems,
            IsOwnProfile = isOwnProfile
        };
    }
}