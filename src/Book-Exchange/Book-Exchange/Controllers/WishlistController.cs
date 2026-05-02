using Book_Exchange.Models;
using Book_Exchange.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Book_Exchange.Controllers;

[Authorize]
public class WishlistController : Controller
{
    private readonly IWishlistService _wishlistService;
    private readonly UserManager<ApplicationUser> _userManager;

    public WishlistController(IWishlistService wishlistService, UserManager<ApplicationUser> userManager)
    {
        _wishlistService = wishlistService;
        _userManager = userManager;
    }

    // GET /Wishlist
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = Guid.Parse(_userManager.GetUserId(User)!);

        var wishlistItems = await _wishlistService.GetWishlistByUserIdAsync(userId);
        var matchingListings = await _wishlistService.GetMatchingListingsAsync(userId);

        // Group matching listings by ISBN to count how many matches each wishlist item has
        var matchCountByIsbn = matchingListings
            .GroupBy(l => l.Isbn)
            .ToDictionary(g => g.Key, g => g.Count());

        var viewModels = wishlistItems.Select(item => new WishlistItemViewModel
        {
            Id = item.Id,
            Isbn = item.Isbn,
            // TODO: Replace with real book metadata lookup once mock API service is wired up
            Title = "Unknown Title",
            Author = "Unknown Author",
            CoverImageUrl = null,
            IsActive = item.IsActive,
            IsAvailable = matchCountByIsbn.ContainsKey(item.Isbn),
            MatchingListingCount = matchCountByIsbn.GetValueOrDefault(item.Isbn, 0)
        }).ToList();

        return View(viewModels);
    }

    // GET /Wishlist/Matches/{id}
    [HttpGet]
    public async Task<IActionResult> Matches(Guid id)
    {
        var userId = Guid.Parse(_userManager.GetUserId(User)!);

        try
        {
            var listings = await _wishlistService.GetMatchingListingsForItemAsync(id, userId);
            return View(listings);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // POST /Wishlist/Add
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(AddWishlistItemDto dto)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please enter a valid ISBN.";
            return RedirectToAction(nameof(Index));
        }

        var userId = Guid.Parse(_userManager.GetUserId(User)!);

        try
        {
            await _wishlistService.AddWishlistItemAsync(userId, dto.Isbn);
            TempData["Success"] = "Book added to your wishlist.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    // POST /Wishlist/Remove/{id} - Soft-deletes (deactivates) a wishlist item.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(Guid id)
    {
        var userId = Guid.Parse(_userManager.GetUserId(User)!);

        try
        {
            await _wishlistService.RemoveWishlistItemAsync(id, userId);
            TempData["Success"] = "Book removed from your wishlist.";
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    // POST /Wishlist/Restore/{id} - Reactivates a previously removed wishlist item.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(Guid id)
    {
        var userId = Guid.Parse(_userManager.GetUserId(User)!);

        try
        {
            await _wishlistService.RestoreWishlistItemAsync(id, userId);
            TempData["Success"] = "Book restored to your wishlist.";
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }
}
