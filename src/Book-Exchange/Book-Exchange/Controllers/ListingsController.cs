using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Listing;
using Book_Exchange.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// TODO: Once ORM is implemented make sure nothing has changed
namespace Book_Exchange.Controllers;

[Authorize]
public class ListingsController : Controller
{
    private readonly IListingService _listingService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ListingsController(IListingService listingService, UserManager<ApplicationUser> userManager)
    {
        _listingService = listingService;
        _userManager = userManager;
    }

    // GET /Listing
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = Guid.Parse(_userManager.GetUserId(User)!);
        var listings = await _listingService.GetListingsByUserIdAsync(userId);
        return View(listings);
    }

    // GET /Listing/Details/{id}
    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var listing = await _listingService.GetListingByIdAsync(id);
            return View(listing);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // GET /Listing/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // POST /Listing/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateListingDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var userId = Guid.Parse(_userManager.GetUserId(User)!);

        try
        {
            await _listingService.CreateListingAsync(dto, userId);
            TempData["Success"] = "Listing created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(dto);
        }
    }

    // GET /Listing/Edit/{id}
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var listing = await _listingService.GetListingByIdAsync(id);
            return View(listing);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // POST /Listing/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateListingDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var userId = Guid.Parse(_userManager.GetUserId(User)!);

        try
        {
            await _listingService.UpdateListingAsync(id, dto, userId);
            TempData["Success"] = "Listing updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(dto);
        }
    }

    // POST /Listing/Delete/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = Guid.Parse(_userManager.GetUserId(User)!);

        try
        {
            await _listingService.DeleteListingAsync(id, userId);
            TempData["Success"] = "Listing deleted successfully.";
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}