using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Book;
using Book_Exchange.Models.DTOs.ExchangeRequest;
using Book_Exchange.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Book_Exchange.Controllers;

[Authorize]
public class ExchangeRequestController : Controller
{
    private readonly IExchangeRequestService _exchangeRequestService;
    private readonly IListingService _listingService;
    private readonly IBookSearchApi _bookSearchApi;

    public ExchangeRequestController(
        IExchangeRequestService exchangeRequestService,
        IListingService listingService,
        IBookSearchApi bookSearchApi)
    {
        _exchangeRequestService = exchangeRequestService;
        _listingService = listingService;
        _bookSearchApi = bookSearchApi;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string tab = "received")
    {
        var userId = GetCurrentUserId();

        var received = await BuildViewDtos(
            await _exchangeRequestService.GetReceivedExchangeRequestsAsync(userId));

        var sent = await BuildViewDtos(
            await _exchangeRequestService.GetSentExchangeRequestsAsync(userId));

        ViewBag.Tab = tab;

        return View(new ExchangeRequestIndexViewModel
        {
            Received = received,
            Sent = sent,
            History = received.Concat(sent)
                .Where(x => x.Status != ExchangeStatus.Requested)
                .OrderByDescending(x => x.CreatedAt)
                .ToList()
        });
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid listingId)
    {
        var userId = GetCurrentUserId();
        var targetListing = await _listingService.GetListingByIdAsync(listingId);

        if (targetListing.UserId == userId)
        {
            return RedirectToAction("Details", "Listing", new { id = listingId });
        }

        var myListings = await _listingService.GetListingsByUserIdAsync(userId);

        ViewBag.TargetListing = targetListing;
        ViewBag.TargetBook = await _bookSearchApi.GetBookByIsbnAsync(targetListing.Isbn);
        ViewBag.MyListings = myListings;

        return View(new CreateExchangeRequestDto
        {
            TargetListingId = listingId
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateExchangeRequestDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var userId = GetCurrentUserId();

        await _exchangeRequestService.CreateExchangeRequestAsync(dto, userId);

        return RedirectToAction(nameof(Index), new { tab = "sent" });
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var request = await _exchangeRequestService.GetExchangeRequestByIdAsync(id);
        var userId = GetCurrentUserId();

        if (request.RequesterId != userId && request.TargetListing.UserId != userId)
        {
            return Forbid();
        }

        var dto = await BuildViewDto(request);

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Accept(Guid id)
    {
        await _exchangeRequestService.AcceptExchangeRequestAsync(id, GetCurrentUserId());
        return RedirectToAction(nameof(Index), new { tab = "received" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(Guid id)
    {
        await _exchangeRequestService.RejectExchangeRequestAsync(id, GetCurrentUserId());
        return RedirectToAction(nameof(Index), new { tab = "received" });
    }

    private Guid GetCurrentUserId()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    private async Task<List<ExchangeRequestViewDto>> BuildViewDtos(IEnumerable<ExchangeRequest> requests)
    {
        var list = new List<ExchangeRequestViewDto>();

        foreach (var request in requests)
        {
            list.Add(await BuildViewDto(request));
        }

        return list;
    }

    private async Task<ExchangeRequestViewDto> BuildViewDto(ExchangeRequest request)
    {
        var targetBook = await _bookSearchApi.GetBookByIsbnAsync(request.TargetListing.Isbn);

        var offeredBooks = new List<BookInfoDto>();
        var offeredIsbns = new List<string>();

        foreach (var item in request.ExchangeRequestItems)
        {
            var isbn = item.OfferedListing.Isbn;

            offeredIsbns.Add(isbn);

            var book = await _bookSearchApi.GetBookByIsbnAsync(isbn);

            if (book != null)
            {
                offeredBooks.Add(book);
            }
        }

        return new ExchangeRequestViewDto
        {
            Id = request.Id,
            TargetListingId = request.TargetListingId,
            TargetIsbn = request.TargetListing.Isbn,
            TargetBook = targetBook,

            RequesterId = request.RequesterId,
            RequesterName = request.Requester.UserName ?? "Unknown",

            OwnerId = request.TargetListing.UserId,
            OwnerName = request.TargetListing.User.UserName ?? "Unknown",

            Status = request.Status,
            Price = request.Price,
            Message = request.Message,
            CreatedAt = request.CreatedAt,
            AcceptedAt = request.AcceptedAt,
            CancelledAt = request.CancelledAt,

            OfferedIsbns = offeredIsbns,
            OfferedBooks = offeredBooks
        };
    }
}

public class ExchangeRequestIndexViewModel
{
    public List<ExchangeRequestViewDto> Received { get; set; } = new();
    public List<ExchangeRequestViewDto> Sent { get; set; } = new();
    public List<ExchangeRequestViewDto> History { get; set; } = new();
}