using System.Security.Claims;
using Book_Exchange.Areas.Listing;
using Book_Exchange.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// TODO: Once ORM is implemented make sure nothing has changed
namespace Book_Exchange.Areas.Listing;

[Authorize]
public class ListingsController : Controller
{
    private readonly IListingService _listingService;

    public ListingsController(IListingService listingService)
    {
        _listingService = listingService;
    }

    private Guid GetCurrentUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User not found.");
        return Guid.Parse(raw);
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var listings = await _listingService.GetListingsByUserIdAsync(GetCurrentUserId());
        return View(listings);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var listing = await _listingService.GetListingByIdAsync(id);
        return View(listing);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateListingDto dto)
    {
        await _listingService.CreateListingAsync(dto, GetCurrentUserId());
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var listing = await _listingService.GetListingByIdAsync(id);
        return View(listing);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, UpdateListingDto dto)
    {
        await _listingService.UpdateListingAsync(id, dto, GetCurrentUserId());
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _listingService.DeleteListingAsync(id, GetCurrentUserId());
        return RedirectToAction(nameof(Index));
    }
}