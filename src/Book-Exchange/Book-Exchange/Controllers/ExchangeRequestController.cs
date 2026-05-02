using Book_Exchange.Models;
using Book_Exchange.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// TODO: Once ORM is implemented make sure nothing has changed
namespace Book_Exchange.Controllers;

[Authorize]
public class ExchangeRequestController : Controller
{
    private readonly IExchangeRequestService _exchangeRequestService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ExchangeRequestController(
        IExchangeRequestService exchangeRequestService,
        UserManager<ApplicationUser> userManager)
    {
        _exchangeRequestService = exchangeRequestService;
        _userManager = userManager;
    }

    // Shows both sent and received exchange requests for the current user
    // GET /ExchangeRequest
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = Guid.Parse(_userManager.GetUserId(User)!);
        var sent = await _exchangeRequestService.GetSentExchangeRequestsAsync(userId);
        var received = await _exchangeRequestService.GetReceivedExchangeRequestsAsync(userId);

        // TODO: Replace with a proper ViewModel once ORM is finalized
        ViewBag.Sent = sent;
        ViewBag.Received = received;

        return View();
    }

    // GET /ExchangeRequest/Details/{id}
    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var request = await _exchangeRequestService.GetExchangeRequestByIdAsync(id);
            return View(request);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // GET /ExchangeRequest/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // POST /ExchangeRequest/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateExchangeRequestDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var userId = Guid.Parse(_userManager.GetUserId(User)!);

        try
        {
            await _exchangeRequestService.CreateExchangeRequestAsync(dto, userId);
            TempData["Success"] = "Exchange request submitted.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(dto);
        }
    }

    // POST /ExchangeRequest/Accept/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Accept(Guid id)
    {
        var userId = Guid.Parse(_userManager.GetUserId(User)!);

        try
        {
            await _exchangeRequestService.AcceptExchangeRequestAsync(id, userId);
            TempData["Success"] = "Exchange request accepted.";
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

    // POST /ExchangeRequest/Reject/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(Guid id)
    {
        var userId = Guid.Parse(_userManager.GetUserId(User)!);

        try
        {
            await _exchangeRequestService.RejectExchangeRequestAsync(id, userId);
            TempData["Success"] = "Exchange request rejected.";
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