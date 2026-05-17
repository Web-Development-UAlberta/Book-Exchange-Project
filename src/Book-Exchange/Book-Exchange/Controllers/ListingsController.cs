using Book_Exchange.Data;
using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Listing;
using Book_Exchange.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Book_Exchange.Controllers;

[Authorize]
public class ListingController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IListingService _listingService;
    private readonly IBookSearchApi _bookSearchApi;
    private readonly IShippingService _shippingService;

    public ListingController(
        ApplicationDbContext context,
        IListingService listingService,
        IBookSearchApi bookSearchApi,
        IShippingService shippingService)
    {
        _context = context;
        _listingService = listingService;
        _bookSearchApi = bookSearchApi;
        _shippingService = shippingService;
    }

    /// <summary>
    /// A listing is considered NOT available if it appears in any active
    /// exchange request either:
    /// 
    /// 1. As the target listing of the exchange request
    ///    (ExchangeRequest.TargetListingId)
    /// 
    /// OR
    /// 
    /// 2. As an offered listing inside ExchangeRequestItems
    ///    (ExchangeRequestItem.OfferedListingId)
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

    public async Task<IActionResult> Index()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var listings = await _context.Listings
            .Where(l => l.UserId == userId)
            .Include(l => l.User)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();

        var viewModels = new List<ListingViewDto>();

        foreach (var listing in listings)
        {
            viewModels.Add(new ListingViewDto
            {
                Id = listing.Id,
                UserId = listing.UserId,
                Isbn = listing.Isbn,
                Condition = listing.Condition,
                Price = listing.Price,
                WeightGrams = listing.WeightGrams,
                CreatedAt = listing.CreatedAt,
                SellerName = listing.User.UserName ?? "Unknown",
                Book = await _bookSearchApi.GetBookByIsbnAsync(listing.Isbn),
                IsAvailable = await IsListingAvailableAsync(listing.Id)
            });
        }

        return View(viewModels);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var listing = await _listingService.GetListingByIdAsync(id);
        var book = await _bookSearchApi.GetBookByIsbnAsync(listing.Isbn);


        var userId = listing.User.Id;

        var reviewQuery = _context.Reviews
            .AsNoTracking()
            .Where(r =>
                r.ReviewerId != userId &&
                (
                    r.Transaction.ExchangeRequest.TargetListing.UserId == userId ||
                    r.Transaction.ExchangeRequest.RequesterId == userId
                ));

        var reviewCount = await reviewQuery.CountAsync();

        var averageRating = reviewCount > 0
            ? Math.Round(await reviewQuery.AverageAsync(r => (double)r.Rating), 1)
            : 0.0;

        var tradeCount = await _context.Transactions
            .AsNoTracking()
            .CountAsync(t =>
                t.ExchangeRequest.RequesterId == userId ||
                t.ExchangeRequest.TargetListing.UserId == userId);
        
        var shippingEstimate = currentUserId != listing.UserId
            ? await _shippingService.GetLowestQuoteBetweenUsersAsync(
                senderUserId: listing.UserId,
                receiverUserId: currentUserId,
                packageWeightGrams: listing.WeightGrams)
            : null;

        var vm = new ListingViewDto
        {
            Id = listing.Id,
            UserId = listing.UserId,
            Isbn = listing.Isbn,
            Condition = listing.Condition,
            Price = listing.Price,
            WeightGrams = listing.WeightGrams,
            CreatedAt = listing.CreatedAt,
            SellerName = listing.User.UserName ?? "Unknown",
            SellerRating = averageRating,
            SellerReviewerCount = reviewCount,
            SellerTradeCount = tradeCount,
            Book = book,
            ShippingEstimate = shippingEstimate,
            IsAvailable = await IsListingAvailableAsync(listing.Id)
        };

        return View(vm);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateListingDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateListingDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var listing = await _listingService.CreateListingAsync(dto, userId);

        return RedirectToAction(nameof(Details), new { id = listing.Id });
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
            isbn13 = b.Isbn13,
            isbn10 = b.Isbn10,
            thumbnail = b.ThumbnailUrl,
            authors = b.Authors,
            publishedYear = b.PublishedYear
        });

        return Json(result);
    }
}