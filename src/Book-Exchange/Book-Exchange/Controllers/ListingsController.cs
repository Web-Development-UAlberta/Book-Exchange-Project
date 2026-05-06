using System.Security.Claims;
using Book_Exchange.Data;
using Book_Exchange.Models.DTOs.Listing;
using Book_Exchange.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Book_Exchange.Controllers;

[Authorize]
public class ListingController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IListingService _listingService;
    private readonly IBookSearchApi _bookSearchApi;

    public ListingController(
        ApplicationDbContext context,
        IListingService listingService,
        IBookSearchApi bookSearchApi)
    {
        _context = context;
        _listingService = listingService;
        _bookSearchApi = bookSearchApi;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var listings = await _context.Listings
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
                Book = await _bookSearchApi.GetBookByIsbnAsync(listing.Isbn)
            });
        }

        return View(viewModels);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(Guid id)
    {
        var listing = await _listingService.GetListingByIdAsync(id);
        var book = await _bookSearchApi.GetBookByIsbnAsync(listing.Isbn);

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
            Book = book
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