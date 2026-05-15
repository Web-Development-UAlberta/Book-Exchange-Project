using Book_Exchange.Data;
using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Book;
using Book_Exchange.Models.DTOs.ExchangeRequest;
using Book_Exchange.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Book_Exchange.Controllers;

[Authorize]
public class ExchangeRequestController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IExchangeRequestService _exchangeRequestService;
    private readonly IListingService _listingService;
    private readonly IBookSearchApi _bookSearchApi;

    public ExchangeRequestController(
        ApplicationDbContext context,
        IExchangeRequestService exchangeRequestService,
        IListingService listingService,
        IBookSearchApi bookSearchApi)
    {
        _context = context;
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

        if (!await IsListingAvailableAsync(listingId))
        {
            TempData["ErrorMessage"] = "This listing is no longer available for exchange.";
            return RedirectToAction("Details", "Listing", new { id = listingId });
        }

        var myListings = await _listingService.GetListingsByUserIdAsync(userId);

        var availableMyListings = new List<Listing>();

        foreach (var listing in myListings)
        {
            if (await IsListingAvailableAsync(listing.Id))
            {
                availableMyListings.Add(listing);
            }
        }

        ViewBag.TargetListing = targetListing;
        ViewBag.TargetBook = await _bookSearchApi.GetBookByIsbnAsync(targetListing.Isbn);
        ViewBag.MyListings = availableMyListings;

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

        if (!await IsListingAvailableAsync(dto.TargetListingId))
        {
            TempData["ErrorMessage"] = "This listing is no longer available for exchange.";
            return RedirectToAction("Details", "Listing", new { id = dto.TargetListingId });
        }

        foreach (var offeredListingId in dto.OfferedListingIds)
        {
            if (!await IsListingAvailableAsync(offeredListingId))
            {
                TempData["ErrorMessage"] = "One of your offered books is no longer available.";
                return RedirectToAction(nameof(Create), new { listingId = dto.TargetListingId });
            }
        }

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

    /// <summary>
    /// A listing is considered NOT available if it appears in any active
    /// exchange request either:
    ///
    /// 1. As the target listing of the exchange request
    ///    ExchangeRequest.TargetListingId
    ///
    /// OR
    ///
    /// 2. As an offered listing inside ExchangeRequestItems
    ///    ExchangeRequestItem.OfferedListingId
    ///
    /// Active statuses:
    /// - Requested
    /// - Accepted
    /// - Completed
    ///
    /// Listings used only in Rejected or Cancelled requests
    /// are still considered available.
    /// </summary>
    private async Task<bool> IsListingAvailableAsync(Guid listingId)
    {
        return !await _context.ExchangeRequests
            .AnyAsync(er =>
                (
                    er.Status == ExchangeStatus.Requested ||
                    er.Status == ExchangeStatus.Accepted ||
                    er.Status == ExchangeStatus.Completed
                )
                &&
                (
                    er.TargetListingId == listingId ||
                    er.ExchangeRequestItems.Any(item => item.OfferedListingId == listingId)
                ));
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