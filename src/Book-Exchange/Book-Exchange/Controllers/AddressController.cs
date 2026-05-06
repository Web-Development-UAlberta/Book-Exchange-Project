using System.Security.Claims;
using Book_Exchange.Models.DTOs.Address;
using Book_Exchange.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Book_Exchange.Controllers;

[Authorize]
public class AddressesController : Controller
{
    private readonly IAddressService _addressService;
    private readonly IPlaceApiService _placeApiService;

    public AddressesController(
        IAddressService addressService,
        IPlaceApiService placeApiService)
    {
        _addressService = addressService;
        _placeApiService = placeApiService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        var addresses = await _addressService.GetAddressesByUserIdAsync(userId);
        return View(addresses);
    }

    public IActionResult Create()
    {
        return View(new CreateAddressDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateAddressDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        try
        {
            await _addressService.CreateAddressAsync(dto, GetUserId());
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(dto);
        }
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var address = await _addressService.GetAddressByIdAsync(id, GetUserId());

        var dto = new UpdateAddressDto
        {
            FullName = address.FullName,
            GooglePlaceId = address.GooglePlaceId
        };

        ViewBag.AddressId = id;
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateAddressDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.AddressId = id;
            return View(dto);
        }

        try
        {
            await _addressService.UpdateAddressAsync(id, dto, GetUserId());
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ViewBag.AddressId = id;
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(dto);
        }
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var address = await _addressService.GetAddressByIdAsync(id, GetUserId());
        return View(address);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        await _addressService.DeleteAddressAsync(id, GetUserId());
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> SearchGoogleAddress(string query)
    {
        var results = await _placeApiService.SearchAddressAsync(query);
        return Json(results);
    }

    private Guid GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
            throw new UnauthorizedAccessException();

        return Guid.Parse(userId);
    }
}