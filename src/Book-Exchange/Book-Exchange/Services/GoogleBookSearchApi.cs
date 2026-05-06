using System.Text.Json;
using Book_Exchange.Models.DTOs.Book;
using Book_Exchange.Services.Interfaces;

namespace Book_Exchange.Services;

public class GoogleBookSearchApi : IBookSearchApi
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public GoogleBookSearchApi(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<BookInfoDto?> GetBookByIsbnAsync(string isbn)
    {
        if (string.IsNullOrWhiteSpace(isbn))
            return null;

        isbn = isbn.Trim().Replace("-", "");

        var apiKey = _configuration["GoogleBooks:ApiKey"];

        var url =
            $"volumes?q=isbn:{Uri.EscapeDataString(isbn)}" +
            $"&maxResults=1" +
            $"&key={apiKey}";

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();

            Console.WriteLine(error);

            return null;
        }

        var json = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<GoogleBooksResponse>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        var book = result?.Items?.FirstOrDefault();

        return book == null ? null : MapToDto(book);
    }

    public async Task<List<BookInfoDto>> SearchBooksAsync(
        string searchText,
        int maxResults = 10)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return new List<BookInfoDto>();

        maxResults = Math.Clamp(maxResults, 1, 40);

        var apiKey = _configuration["GoogleBooks:ApiKey"];

        var url =
            $"volumes?q={Uri.EscapeDataString(searchText.Trim())}" +
            $"&maxResults={maxResults}" +
            $"&key={apiKey}";

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();

            Console.WriteLine(error);

            return new List<BookInfoDto>();
        }

        var json = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<GoogleBooksResponse>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return result?.Items?
            .Select(MapToDto)
            .ToList()
            ?? new List<BookInfoDto>();
    }

    private static BookInfoDto MapToDto(GoogleBookItem item)
    {
        var volume = item.VolumeInfo;

        var isbn10 = volume?.IndustryIdentifiers?
            .FirstOrDefault(x => x.Type == "ISBN_10")
            ?.Identifier;

        var isbn13 = volume?.IndustryIdentifiers?
            .FirstOrDefault(x => x.Type == "ISBN_13")
            ?.Identifier;

        return new BookInfoDto
        {
            GoogleBookId = item.Id,
            Title = volume?.Title,
            Authors = volume?.Authors ?? new List<string>(),
            Genres = volume?.Categories ?? new List<string>(),
            Publisher = volume?.Publisher,
            PublishedYear = ExtractYear(volume?.PublishedDate),
            Description = volume?.Description,
            Isbn10 = isbn10,
            Isbn13 = isbn13,
            PageCount = volume?.PageCount,
            ThumbnailUrl = volume?.ImageLinks?.Thumbnail,
            PreviewLink = volume?.PreviewLink
        };
    }

    private static int? ExtractYear(string? publishedDate)
    {
        if (string.IsNullOrWhiteSpace(publishedDate))
            return null;

        var yearPart = publishedDate.Length >= 4
            ? publishedDate[..4]
            : publishedDate;

        return int.TryParse(yearPart, out var year)
            ? year
            : null;
    }
}