using System.Security.Claims;
using Book_Exchange.Models.DTOs;
using Book_Exchange.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// TODO: Once ORM is implemented make sure nothing has changed
namespace Book_Exchange.Areas.ExchangeRequest;

[Authorize]
public class ExchangeRequestController : Controller
{
    private readonly IExchangeRequestService _exchangeRequestService;

    public ExchangeRequestController(IExchangeRequestService exchangeRequestService)
    {
        _exchangeRequestService = exchangeRequestService;
    }

    private Guid GetCurrentUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User not found.");
        return Guid.Parse(raw);
    }

    // Shows both sent and received exchange requests for the current user
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = GetCurrentUserId();
        var sent = await _exchangeRequestService.GetSentExchangeRequestsAsync(userId);
        var received = await _exchangeRequestService.GetReceivedExchangeRequestsAsync(userId);

        // TODO: Replace with a proper ViewModel once ORM is finalized
        ViewBag.Sent = sent;
        ViewBag.Received = received;

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var request = await _exchangeRequestService.GetExchangeRequestByIdAsync(id);
        return View(request);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateExchangeRequestDto dto)
    {
        await _exchangeRequestService.CreateExchangeRequestAsync(dto, GetCurrentUserId());
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Accept(Guid id)
    {
        await _exchangeRequestService.AcceptExchangeRequestAsync(id, GetCurrentUserId());
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Reject(Guid id)
    {
        await _exchangeRequestService.RejectExchangeRequestAsync(id, GetCurrentUserId());
        return RedirectToAction(nameof(Index));
    }
}