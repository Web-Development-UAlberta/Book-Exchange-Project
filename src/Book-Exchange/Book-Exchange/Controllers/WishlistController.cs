using System.Security.Claims;
using Book_Exchange.Models.DTOs.Wishlist;
using Book_Exchange.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Book_Exchange.Controllers;

[Authorize]
public class WishlistController : Controller
{
    private readonly IWishlistService _wishlistService;
    private readonly IBookSearchApi _bookSearchApi;

    public WishlistController(
        IWishlistService wishlistService,
        IBookSearchApi bookSearchApi)
    {
        _wishlistService = wishlistService;
        _bookSearchApi = bookSearchApi;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? searchText, bool availableOnly = false)
    {
        var userId = GetCurrentUserId();

        var wishlist = await _wishlistService.GetWishlistByUserIdAsync(userId);
        var matchingListings = await _wishlistService.GetMatchingListingsAsync(userId);

        var matchingIsbns = matchingListings
            .Select(l => l.Isbn)
            .Distinct()
            .ToHashSet();

        var viewModel = new WishlistIndexViewDto
        {
            SearchText = searchText,
            AvailableOnly = availableOnly
        };

        foreach (var item in wishlist)
        {
            var book = await _bookSearchApi.GetBookByIsbnAsync(item.Isbn);

            var hasMatch = matchingIsbns.Contains(item.Isbn);

            var dto = new WishlistItemViewDto
            {
                Id = item.Id,
                Isbn = item.Isbn,
                IsActive = item.IsActive,
                Book = book,
                HasMatchingListing = hasMatch
            };

            viewModel.Items.Add(dto);
        }

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

        var listings = await _wishlistService.GetMatchingListingsForItemAsync(id, userId);

        return View(listings);
    }

    private Guid GetCurrentUserId()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}