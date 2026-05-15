using Book_Exchange.Data;
using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Wishlist;
using Book_Exchange.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Book_Exchange.Controllers;

[Authorize]
public class WishlistController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWishlistService _wishlistService;
    private readonly IBookSearchApi _bookSearchApi;

    public WishlistController(
        ApplicationDbContext context,
        IWishlistService wishlistService,
        IBookSearchApi bookSearchApi)
    {
        _context = context;
        _wishlistService = wishlistService;
        _bookSearchApi = bookSearchApi;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? searchText, bool availableOnly = false)
    {
        var userId = GetCurrentUserId();

        var wishlist = await _wishlistService.GetWishlistByUserIdAsync(userId);
        var matchingListings = await _wishlistService.GetMatchingListingsAsync(userId);

        var viewModel = new WishlistIndexViewDto
        {
            SearchText = searchText,
            AvailableOnly = availableOnly
        };

        foreach (var item in wishlist)
        {
            var book = await _bookSearchApi.GetBookByIsbnAsync(item.Isbn);

            var matchingListingsForItem = matchingListings
                .Where(l => l.Isbn == item.Isbn)
                .ToList();

            var hasAvailableMatch = false;

            foreach (var listing in matchingListingsForItem)
            {
                if (await IsListingAvailableAsync(listing.Id))
                {
                    hasAvailableMatch = true;
                    break;
                }
            }

            var dto = new WishlistItemViewDto
            {
                Id = item.Id,
                Isbn = item.Isbn,
                IsActive = item.IsActive,
                Book = book,
                HasMatchingListing = hasAvailableMatch
            };

            viewModel.Items.Add(dto);
        }

        viewModel.TotalActiveCount = viewModel.Items.Count(i => i.IsActive);

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            viewModel.Items = viewModel.Items
                .Where(i =>
                    i.Isbn.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    (i.Book?.Title?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (i.Book?.Authors.Any(a => a.Contains(searchText, StringComparison.OrdinalIgnoreCase)) ?? false))
                .ToList();
        }

        if (availableOnly)
        {
            viewModel.Items = viewModel.Items
                .Where(i => i.HasMatchingListing)
                .ToList();
        }

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> SearchBooks(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return Json(new List<object>());
        }

        var books = await _bookSearchApi.SearchBooksAsync(searchText, 10);

        var result = books.Select(b => new
        {
            title = b.Title,
            authors = b.Authors,
            isbn13 = b.Isbn13,
            isbn10 = b.Isbn10,
            thumbnail = b.ThumbnailUrl,
            publishedYear = b.PublishedYear
        });

        return Json(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(string isbn)
    {
        var userId = GetCurrentUserId();

        await _wishlistService.AddWishlistItemAsync(userId, isbn);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(Guid id)
    {
        var userId = GetCurrentUserId();

        var wishlistItem = await _context.Wishlist
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

        if (wishlistItem == null)
        {
            return NotFound();
        }

        var hasActiveExchange = await _context.ExchangeRequests
            .AnyAsync(er =>
                (
                    er.Status == ExchangeStatus.Requested ||
                    er.Status == ExchangeStatus.Accepted ||
                    er.Status == ExchangeStatus.Completed
                )
                &&
                (
                    er.TargetListing.Isbn == wishlistItem.Isbn ||
                    er.ExchangeRequestItems.Any(item =>
                        item.OfferedListing.Isbn == wishlistItem.Isbn)
                ));

        if (hasActiveExchange)
        {
            TempData["ErrorMessage"] =
                "You cannot remove this wishlist item because it is connected to an active exchange request.";

            return RedirectToAction(nameof(Index));
        }

        await _wishlistService.RemoveWishlistItemAsync(id, userId);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(Guid id)
    {
        var userId = GetCurrentUserId();

        await _wishlistService.RestoreWishlistItemAsync(id, userId);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Matches(Guid id)
    {
        var userId = GetCurrentUserId();

        var listings = await _wishlistService
            .GetMatchingListingsForItemAsync(id, userId);

        var model = new List<WishListMatchingViewModel>();

        if (listings == null || !listings.Any())
        {
            return View(model);
        }

        foreach (var listing in listings)
        {
            if (!await IsListingAvailableAsync(listing.Id))
            {
                continue;
            }

            var book = await _bookSearchApi.GetBookByIsbnAsync(listing.Isbn);

            model.Add(new WishListMatchingViewModel
            {
                Listing = listing,
                Book = book
            });
        }

        return View(model);
    }

    /// <summary>
    /// A listing is considered NOT available if it appears in any active
    /// exchange request either:
    ///
    /// 1. As the target listing of the exchange request
    ///    ExchangeRequest.TargetListingId
    ///
    /// OR
    ///
    /// 2. As an offered listing inside ExchangeRequestItems
    ///    ExchangeRequestItem.OfferedListingId
    ///
    /// Active statuses:
    /// - Requested
    /// - Accepted
    /// - Completed
    ///
    /// Listings used only in Rejected or Cancelled requests
    /// are still considered available.
    /// </summary>
    private async Task<bool> IsListingAvailableAsync(Guid listingId)
    {
        return !await _context.ExchangeRequests
            .AnyAsync(er =>
                (
                    er.Status == ExchangeStatus.Requested ||
                    er.Status == ExchangeStatus.Accepted ||
                    er.Status == ExchangeStatus.Completed
                )
                &&
                (
                    er.TargetListingId == listingId ||
                    er.ExchangeRequestItems.Any(item => item.OfferedListingId == listingId)
                ));
    }

    private Guid GetCurrentUserId()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NotifyMe(Guid id)
    {
        var userId = GetCurrentUserId();

        await _wishlistService.RequestNotificationAsync(id, userId);

        TempData["Success"] = "You'll be notified when this book becomes available.";

        return RedirectToAction(nameof(Index));
    }

}