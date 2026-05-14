using Book_Exchange.Data;
using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Book;
using Book_Exchange.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Book_Exchange.Services;

public class GoogleBookSearchApi : IBookSearchApi
{
    private readonly HttpClient _httpClient;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public GoogleBookSearchApi(
        ApplicationDbContext context,
        HttpClient httpClient,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        _context = context;
        _httpClient = httpClient;
        _configuration = configuration;
        _environment = environment;
    }

    public async Task<BookInfoDto?> GetBookByIsbnAsync(string isbn)
    {
        if (string.IsNullOrWhiteSpace(isbn))
            return null;

        isbn = NormalizeIsbn(isbn);

        var cachedBook = await _context.BookCaches
            .FirstOrDefaultAsync(x =>
                x.Isbn == isbn ||
                x.Isbn10 == isbn);

        if (cachedBook != null)
        {
            return MapCacheToDto(cachedBook);
        }

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

        if (book == null)
            return null;

        var dto = MapToDto(book);

        var cacheKeyIsbn = dto.Isbn13 ?? dto.Isbn10 ?? isbn;

        var localThumbnailPath = await DownloadThumbnailAsync(
            cacheKeyIsbn,
            dto.ThumbnailUrl);

        var cache = new BookCache
        {
            GoogleBookId = dto.GoogleBookId,
            Title = dto.Title,
            Authors = string.Join(", ", dto.Authors),
            Genres = string.Join(", ", dto.Genres),
            Publisher = dto.Publisher,
            PublishedYear = dto.PublishedYear,
            Description = dto.Description,
            Isbn10 = dto.Isbn10,
            Isbn = dto.Isbn13 ?? isbn,
            PageCount = dto.PageCount,
            ThumbnailUrl = dto.ThumbnailUrl,
            ThumbnailPath = localThumbnailPath,
            PreviewLink = dto.PreviewLink,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.BookCaches.Add(cache);
        await _context.SaveChangesAsync();

        dto.ThumbnailUrl = localThumbnailPath ?? dto.ThumbnailUrl;

        return dto;
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

    private async Task<string?> DownloadThumbnailAsync(
        string isbn,
        string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return null;

        imageUrl = imageUrl.Replace("http://", "https://");

        var folderPath = Path.Combine(
            _environment.WebRootPath,
            "book-covers");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var fileName = $"{NormalizeIsbn(isbn)}.jpg";
        var filePath = Path.Combine(folderPath, fileName);

        if (File.Exists(filePath))
        {
            return $"/book-covers/{fileName}";
        }

        try
        {
            var imageBytes = await _httpClient.GetByteArrayAsync(imageUrl);
            await File.WriteAllBytesAsync(filePath, imageBytes);

            return $"/book-covers/{fileName}";
        }
        catch
        {
            return null;
        }
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

        var thumbnailUrl =
            volume?.ImageLinks?.Thumbnail ??
            volume?.ImageLinks?.SmallThumbnail;

        if (!string.IsNullOrWhiteSpace(thumbnailUrl))
        {
            thumbnailUrl = thumbnailUrl.Replace("http://", "https://");
        }

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
            ThumbnailUrl = thumbnailUrl,
            PreviewLink = volume?.PreviewLink
        };
    }

    private static BookInfoDto MapCacheToDto(BookCache cache)
    {
        return new BookInfoDto
        {
            GoogleBookId = cache.GoogleBookId,
            Title = cache.Title,
            Authors = SplitString(cache.Authors),
            Genres = SplitString(cache.Genres),
            Publisher = cache.Publisher,
            PublishedYear = cache.PublishedYear,
            Description = cache.Description,
            Isbn10 = cache.Isbn10,
            Isbn13 = cache.Isbn,
            PageCount = cache.PageCount,
            ThumbnailUrl = cache.ThumbnailPath ?? cache.ThumbnailUrl,
            PreviewLink = cache.PreviewLink
        };
    }

    private static List<string> SplitString(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? new List<string>()
            : value.Split(", ", StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    private static string NormalizeIsbn(string isbn)
    {
        return isbn
            .Replace("-", "")
            .Replace(" ", "")
            .Trim();
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