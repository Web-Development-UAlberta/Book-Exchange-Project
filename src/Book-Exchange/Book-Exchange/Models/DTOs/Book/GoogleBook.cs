namespace Book_Exchange.Models.DTOs.Book;

using System.Text.Json.Serialization;

public class GoogleBooksResponse
{
    [JsonPropertyName("items")]
    public List<GoogleBookItem>? Items { get; set; }
}

public class GoogleBookItem
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("volumeInfo")]
    public GoogleVolumeInfo? VolumeInfo { get; set; }
}

public class GoogleVolumeInfo
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("authors")]
    public List<string>? Authors { get; set; }

    [JsonPropertyName("categories")]
    public List<string>? Categories { get; set; }

    [JsonPropertyName("publisher")]
    public string? Publisher { get; set; }

    [JsonPropertyName("publishedDate")]
    public string? PublishedDate { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("industryIdentifiers")]
    public List<GoogleIndustryIdentifier>? IndustryIdentifiers { get; set; }

    [JsonPropertyName("pageCount")]
    public int? PageCount { get; set; }

    [JsonPropertyName("imageLinks")]
    public GoogleImageLinks? ImageLinks { get; set; }

    [JsonPropertyName("previewLink")]
    public string? PreviewLink { get; set; }
}

public class GoogleIndustryIdentifier
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("identifier")]
    public string? Identifier { get; set; }
}

public class GoogleImageLinks
{
    public string? SmallThumbnail { get; set; }
    [JsonPropertyName("thumbnail")]
    public string? Thumbnail { get; set; }
}
