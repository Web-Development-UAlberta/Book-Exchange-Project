using Book_Exchange.Models;
using Book_Exchange.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// TODO: Once ORM is implemented make sure nothing has changed
namespace Book_Exchange.Controllers;

[Authorize]
public class ShippingController : Controller
{
    private readonly IShippingService _shippingService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ShippingController(
        IShippingService shippingService,
        UserManager<ApplicationUser> userManager)
    {
        _shippingService = shippingService;
        _userManager = userManager;
    }

    // GET /Shipping
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User)!;
        var shipments = await _shippingService.GetShipmentsForUserAsync(userId);
        return View(shipments);
    }

    // GET /Shipping/Details/{transactionId}
    [HttpGet]
    public async Task<IActionResult> Details(Guid transactionId)
    {
        var shipment = await _shippingService.GetShipmentByTransactionAsync(transactionId);

        if (shipment == null)
            return NotFound();

        return View(shipment);
    }

    // GET /Shipping/Quote/{transactionId}
    [HttpGet]
    public async Task<IActionResult> Quote(Guid transactionId)
    {
        // TODO: Once ORM is done resolve senderAddressId, receiverAddressId, and packageWeightGrams
        throw new NotImplementedException();
    }

    // POST /Shipping/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Guid transactionId, Guid senderAddressId, Guid receiverAddressId, Guid carrierId, int packageWeightGrams)
    {
        try
        {
            await _shippingService.CreateShipmentAsync(transactionId, senderAddressId, receiverAddressId, carrierId, packageWeightGrams);

            TempData["Success"] = "Shipment created successfully.";
            return RedirectToAction(nameof(Details), new { transactionId });
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return RedirectToAction(nameof(Quote), new { transactionId });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return RedirectToAction(nameof(Quote), new { transactionId });
        }
    }

    // POST /Shipping/UpdateStatus/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(Guid id, ShipmentStatus newStatus)
    {
        try
        {
            var shipment = await _shippingService.UpdateShipmentStatusAsync(id, newStatus);
            TempData["Success"] = $"Shipment status updated to {newStatus}.";
            return RedirectToAction(nameof(Details), new { transactionId = shipment.TransactionId });
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    // POST /Shipping/Cancel/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid id)
    {
        try
        {
            var shipment = await _shippingService.CancelShipmentAsync(id);
            TempData["Success"] = "Shipment cancelled.";
            return RedirectToAction(nameof(Details), new { transactionId = shipment.TransactionId });
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}