using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Shipping;
using Book_Exchange.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Book_Exchange.Controllers;

[Authorize]
public class ShippingController : Controller
{
    private readonly IShippingService _shippingService;
    private readonly IAddressService _addressService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ShippingController(
        IShippingService shippingService,
        IAddressService addressService,
        UserManager<ApplicationUser> userManager)
    {
        _shippingService = shippingService;
        _addressService = addressService;
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

    // GET /Shipping/Details/{id}
    // 'id' is the Shipment ID (not the transaction ID).
    // Each user's index links to their own shipment row directly, so there is no
    // ambiguity when a transaction has two shipments (one per swap direction).
    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var userId = Guid.Parse(_userManager.GetUserId(User)!);
        var shipment = await _shippingService.GetShipmentAsync(id);

        if (shipment == null)
            return NotFound();

        // Only parties to the shipment may view it
        bool isSender = shipment.SenderAddress.UserId == userId;
        bool isReceiver = shipment.ReceiverAddress.UserId == userId;

        if (!isSender && !isReceiver)
            return Forbid();

        ViewBag.IsSender = isSender;
        ViewBag.ShipmentReference = $"SHP-{shipment.Id.ToString("N")[..4].ToUpper()}";

        return View(shipment);
    }

    // GET /Shipping/Quote/{transactionId}
    [HttpGet]
    public async Task<IActionResult> Quote(Guid transactionId, Guid senderAddressId, Guid receiverAddressId, int packageWeightGrams)
    {
        try
        {
            var quotes = await _shippingService.GetQuotesAsync(transactionId, senderAddressId, receiverAddressId, packageWeightGrams);

            ViewBag.TransactionId = transactionId;
            ViewBag.SenderAddressId = senderAddressId;
            ViewBag.ReceiverAddressId = receiverAddressId;
            ViewBag.PackageWeightGrams = packageWeightGrams;

            return View(quotes);
        }
        catch (ArgumentException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    // POST /Shipping/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Guid transactionId, Guid senderAddressId, Guid receiverAddressId, Guid carrierId, int packageWeightGrams)
    {
        try
        {
            var shipment = await _shippingService.CreateShipmentAsync(transactionId, senderAddressId, receiverAddressId, carrierId, packageWeightGrams);

            TempData["Success"] = "Shipment created successfully.";
            return RedirectToAction(nameof(Details), new { id = shipment.Id });
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return RedirectToAction(nameof(Quote), new { transactionId, senderAddressId, receiverAddressId, packageWeightGrams });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return RedirectToAction(nameof(Quote), new { transactionId, senderAddressId, receiverAddressId, packageWeightGrams });
        }
    }

    // POST /Shipping/UpdateStatus/{id}
    // Only the sender may advance their shipment's status.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(Guid id, ShipmentStatus newStatus)
    {
        var userId = Guid.Parse(_userManager.GetUserId(User)!);

        var shipment = await _shippingService.GetShipmentAsync(id);
        if (shipment == null)
            return NotFound();

        if (shipment.SenderAddress.UserId != userId)
        {
            TempData["Error"] = "You can only update the status of shipments you are sending.";
            return RedirectToAction(nameof(Details), new { id = shipment.Id });
        }

        try
        {
            var updated = await _shippingService.UpdateShipmentStatusAsync(id, newStatus);
            TempData["Success"] = $"Shipment status updated to {newStatus}.";
            return RedirectToAction(nameof(Details), new { id = updated.Id });
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
    // Only the sender may cancel their own shipment.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var userId = Guid.Parse(_userManager.GetUserId(User)!);

        var shipment = await _shippingService.GetShipmentAsync(id);
        if (shipment == null)
            return NotFound();

        if (shipment.SenderAddress.UserId != userId)
        {
            TempData["Error"] = "You can only cancel shipments you are sending.";
            return RedirectToAction(nameof(Details), new { id = shipment.Id });
        }

        try
        {
            var cancelled = await _shippingService.CancelShipmentAsync(id);
            TempData["Success"] = "Shipment cancelled.";
            return RedirectToAction(nameof(Details), new { id = cancelled.Id });
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