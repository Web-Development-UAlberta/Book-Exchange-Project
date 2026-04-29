namespace Book_Exchange.Models;

public class Genre
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<ListingGenre> ListingGenres { get; set; } = new List<ListingGenre>();
}