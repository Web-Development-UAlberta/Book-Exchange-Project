using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Review;
using Book_Exchange.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Book_Exchange.Controllers;

[Authorize]
public class ReviewController : Controller
{
    private readonly IReviewService _reviewService;
    private readonly ITransactionService _transactionService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReviewController(
        IReviewService reviewService,
        ITransactionService transactionService,
        UserManager<ApplicationUser> userManager)
    {
        _reviewService = reviewService;
        _transactionService = transactionService;
        _userManager = userManager;
    }

    // GET /Review/Create/{transactionId}?revieweeId={revieweeId}
    [HttpGet]
    public async Task<IActionResult> Create(Guid transactionId, Guid revieweeId)
    {
        try
        {
            var userId = Guid.Parse(_userManager.GetUserId(User)!);
            var vm = await _transactionService.GetTransactionByIdAsync(transactionId, userId);

            // Derive reviewee name: if the current user is the requester,
            // the reviewee is the listing owner (WithUserName already holds the other party)
            ViewBag.TransactionCode = transactionId.ToString()[..8].ToUpper();
            ViewBag.CompletedDate = vm.CreatedAt.ToString("MMMM d, yyyy");
            ViewBag.RevieweeName = vm.WithUserName;
            ViewBag.BookTitle = vm.Description;
            ViewBag.BookSnap = string.Empty;
        }
        catch (KeyNotFoundException)
        {

        }

        var dto = new CreateReviewDto
        {
            TransactionId = transactionId,
            RevieweeId = revieweeId
        };

        return View(dto);
    }

    // GET /Review/User/{userId}
    [HttpGet]
    [Route("Review/GetUser/{userId}")]
    public async Task<IActionResult> GetUser(Guid userId)
    {
        try
        {
            var reviews = await _reviewService.GetReviewsByUserIdAsync(userId);
            var averageRating = await _reviewService.GetAverageRatingForUserAsync(userId);

            ViewBag.AverageRating = averageRating;
            ViewBag.RevieweeId = userId;

            return View(reviews);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // GET /Review/Details/{id}
    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var review = await _reviewService.GetReviewByIdAsync(id);
            return View(review);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
