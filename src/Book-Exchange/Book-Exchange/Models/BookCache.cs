namespace Book_Exchange.Models;

public class BookCache
{
    public int Id { get; set; }
    public string Isbn { get; set; } = null!;
    public string? GoogleBookId { get; set; }
    public string? Title { get; set; }
    public string? Authors { get; set; }
    public string? Genres { get; set; }
    public string? Publisher { get; set; }
    public int? PublishedYear { get; set; }
    public string? Description { get; set; }
    public string? Isbn10 { get; set; }
    public int? PageCount { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? ThumbnailPath { get; set; }
    public string? PreviewLink { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}