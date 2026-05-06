using Book_Exchange.Models.DTOs.Book;
using Book_Exchange.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Book_Exchange.Controllers;

public class BookSearchController : Controller
{
    private readonly IBookSearchApi _bookSearchApi;

    public BookSearchController(IBookSearchApi bookSearchApi)
    {
        _bookSearchApi = bookSearchApi;
    }

    [HttpGet]
    public IActionResult Index()
    {
        ViewBag.SearchText = "";
        return View(new List<BookInfoDto>());
    }

    [HttpGet]
    public async Task<IActionResult> Search(string? searchText)
    {
        ViewBag.SearchText = searchText;

        if (string.IsNullOrWhiteSpace(searchText))
        {
            return View("Index", new List<BookInfoDto>());
        }

        var books = await _bookSearchApi.SearchBooksAsync(searchText, 10)
                    ?? new List<BookInfoDto>();

        return View("Index", books);
    }
}