namespace Book_Exchange.Models;

public class ListingGenre
{
    public Guid ListingId { get; set; }
    public Listing Listing { get; set; } = null!;

    public Guid GenreId { get; set; }
    public Genre Genre { get; set; } = null!;
}