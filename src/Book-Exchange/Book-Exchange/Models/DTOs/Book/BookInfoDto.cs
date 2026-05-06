namespace Book_Exchange.Models.DTOs.Book;

public class BookInfoDto
{
    public string? GoogleBookId { get; set; }

    public string? Title { get; set; }

    public List<string> Authors { get; set; } = new();

    public List<string> Genres { get; set; } = new();

    public string? Publisher { get; set; }

    public int? PublishedYear { get; set; }

    public string? Description { get; set; }

    public string? Isbn10 { get; set; }

    public string? Isbn13 { get; set; }

    public int? PageCount { get; set; }

    public string? ThumbnailUrl { get; set; }

    public string? PreviewLink { get; set; }
}