using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Review;
using Book_Exchange.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// TODO: Once ORM is implemented make sure nothing changes. 
namespace Book_Exchange.Controllers;

[Authorize]
public class ReviewController : Controller
{
    private readonly IReviewService _reviewService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReviewController(IReviewService reviewService, UserManager<ApplicationUser> userManager)
    {
        _reviewService = reviewService;
        _userManager = userManager;
    }

    // GET /Review/Create/{transactionId}... {revieweeId}
    [HttpGet]
    public IActionResult Create(Guid transactionId, Guid revieweeId)
    {
        var dto = new CreateReviewDto
        {
            TransactionId = transactionId,
            RevieweeId = revieweeId
        };
        return View(dto);
    }

    // POST /Review/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateReviewDto dto)
    {
        var userId = Guid.Parse(_userManager.GetUserId(User)!);

        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        try
        {
            await _reviewService.CreateReviewAsync(dto, userId);
            TempData["Success"] = "Review submitted successfully.";
            return RedirectToAction("Index", "Transaction");
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(dto);
        }
    }

    // GET /Review/User/{userId}
    [HttpGet]
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
