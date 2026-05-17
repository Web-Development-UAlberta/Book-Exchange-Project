namespace Book_Exchange.Services.Interfaces;

using Book_Exchange.Models.DTOs.Book;
public interface IBookSearchApi
{
    Task<BookInfoDto?> GetBookByIsbnAsync(string isbn);
    Task<List<BookInfoDto>> SearchBooksAsync(string searchText, int maxResults = 10);
}