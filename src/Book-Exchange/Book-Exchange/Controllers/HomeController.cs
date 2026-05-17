using Book_Exchange.Data;
using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Listing;
using Book_Exchange.Models.ViewModels;
using Book_Exchange.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Book_Exchange.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IBookSearchApi _bookSearchApi;
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(
        ApplicationDbContext context,
        IBookSearchApi bookSearchApi,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _bookSearchApi = bookSearchApi;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction(nameof(Dashboard));

        var listings = await _context.Listings
            .Include(l => l.User)
            .OrderByDescending(l => l.CreatedAt)
            .Take(4)
            .ToListAsync();

        var dtos = new List<ListingViewDto>();

        foreach (var l in listings)
        {
            dtos.Add(new ListingViewDto
            {
                Id = l.Id,
                UserId = l.UserId,
                Isbn = l.Isbn,
                Condition = l.Condition,
                Price = l.Price,
                WeightGrams = l.WeightGrams,
                CreatedAt = l.CreatedAt,
                SellerName = l.User?.UserName ?? "Unknown",
                Book = await _bookSearchApi.GetBookByIsbnAsync(l.Isbn)
            });
        }

        return View(dtos);
    }

    [Authorize]
    public async Task<IActionResult> Dashboard()
    {
        var userId = Guid.Parse(_userManager.GetUserId(User)!);
        var user = await _userManager.GetUserAsync(User);

        var myListings = await _context.Listings
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();

        var wishlistItems = await _context.Wishlist
            .Where(w => w.UserId == userId && w.IsActive)
            .OrderByDescending(w => w.Id)
            .ToListAsync();

        var dashboardWishlistItems = new List<DashboardWishlistItemViewModel>();

        foreach (var item in wishlistItems)
        {
            var book = await _bookSearchApi.GetBookByIsbnAsync(item.Isbn);

            dashboardWishlistItems.Add(new DashboardWishlistItemViewModel
            {
                Id = item.Id,
                Isbn = item.Isbn,
                IsActive = item.IsActive,
                BookTitle = !string.IsNullOrWhiteSpace(book?.Title)
                    ? book.Title
                    : item.Isbn,
                BookAuthor = book?.Authors?.FirstOrDefault()
            });
        }

        var dashboardListingItems = new List<DashboardListingItemViewModel>();

        foreach (var listing in myListings.Take(5))
        {
            var book = await _bookSearchApi.GetBookByIsbnAsync(listing.Isbn);

            dashboardListingItems.Add(new DashboardListingItemViewModel
            {
                Id = listing.Id,
                Isbn = listing.Isbn,
                Condition = listing.Condition,
                Price = listing.Price,
                BookTitle = !string.IsNullOrWhiteSpace(book?.Title)
                    ? book.Title
                    : listing.Isbn,
                BookAuthor = book?.Authors?.FirstOrDefault()
            });
        }

        var unreadMessageCount = await _context.Messages
            .Where(m => m.ReceiverId == userId && !m.IsRead)
            .CountAsync();

        var recentNotifications = await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(5)
            .ToListAsync();

        var allUserTransactions = await _context.Transactions
            .Include(t => t.ExchangeRequest)
                .ThenInclude(er => er.Requester)
            .Include(t => t.StatusHistory)
            .Where(t => t.ExchangeRequest.RequesterId == userId
                     || _context.Listings
                            .Where(l => l.UserId == userId)
                            .Select(l => l.Id)
                            .Contains(t.ExchangeRequest.TargetListingId))
            .ToListAsync();

        var openTransactions = allUserTransactions
            .Where(t =>
            {
                var latest = t.StatusHistory
                    .OrderByDescending(h => h.UpdatedAt)
                    .Select(h => h.Status)
                    .FirstOrDefault();

                return latest == TransactionStatus.Confirmed
                    || latest == TransactionStatus.Shipped;
            })
            .Take(5)
            .ToList();

        var completedTransactions = allUserTransactions
            .Where(t => t.StatusHistory
                .OrderByDescending(h => h.UpdatedAt)
                .Select(h => h.Status)
                .FirstOrDefault() == TransactionStatus.Completed)
            .ToList();

        var completedIds = completedTransactions.Select(t => t.Id).ToHashSet();

        var reviews = await _context.Reviews
            .Where(r => completedIds.Contains(r.TransactionId)
                     && r.ReviewerId != userId)
            .ToListAsync();

        double? averageRating = reviews.Any()
            ? reviews.Average(r => r.Rating)
            : null;

        var vm = new DashboardViewModel
        {
            UserName = user?.UserName ?? "User",
            AverageRating = averageRating,
            TradeCount = completedTransactions.Count,
            ActiveListingCount = myListings.Count,
            WishlistCount = wishlistItems.Count,
            OpenTransactionCount = openTransactions.Count,
            UnreadMessageCount = unreadMessageCount,
            OpenTransactions = openTransactions,
            RecentNotifications = recentNotifications,
            WishlistItems = dashboardWishlistItems,
            MyActiveListings = dashboardListingItems
        };

        return View(vm);
    }

    public async Task<IActionResult> Search(string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return RedirectToAction(nameof(Index));

        var books = await _bookSearchApi.SearchBooksAsync(q, 20);
        ViewBag.Query = q;

        return View(books);
    }
}