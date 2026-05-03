using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Address;
using Book_Exchange.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// TODO: Once ORM is implemented make sure nothing has changed.
namespace Book_Exchange.Controllers;

[Authorize]
public class AddressController : Controller
{
    private readonly IAddressService _addressService;
    private readonly UserManager<ApplicationUser> _userManager;

    public AddressController(IAddressService addressService, UserManager<ApplicationUser> userManager)
    {
        _addressService = addressService;
        _userManager = userManager;
    }

    // GET /Address
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = Guid.Parse(_userManager.GetUserId(User)!);
        var addresses = await _addressService.GetAddressesByUserIdAsync(userId);
        return View(addresses);
    }

    // GET /Address/Details/{id}
    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var userId = Guid.Parse(_userManager.GetUserId(User)!);

        try
        {
            var address = await _addressService.GetAddressByIdAsync(id, userId);
            return View(address);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // GET /Address/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // POST /Address/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateAddressDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var userId = Guid.Parse(_userManager.GetUserId(User)!);

        try
        {
            await _addressService.CreateAddressAsync(dto, userId);
            TempData["Success"] = "Address saved successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(dto);
        }
    }

    // GET /Address/Edit/{id}
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var userId = Guid.Parse(_userManager.GetUserId(User)!);

        try
        {
            var address = await _addressService.GetAddressByIdAsync(id, userId);
            var dto = new UpdateAddressDto
            {
                FullName = address.FullName,
                GooglePlaceId = address.GooglePlaceId
            };
            return View(dto);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // POST /Address/Edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateAddressDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var userId = Guid.Parse(_userManager.GetUserId(User)!);

        try
        {
            await _addressService.UpdateAddressAsync(id, dto, userId);
            TempData["Success"] = "Address updated successfully.";
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

    // POST /Address/Delete/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = Guid.Parse(_userManager.GetUserId(User)!);

        try
        {
            await _addressService.DeleteAddressAsync(id, userId);
            TempData["Success"] = "Address deleted successfully.";
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