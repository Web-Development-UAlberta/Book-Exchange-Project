using Book_Exchange.Models;

namespace Book_Exchange.Areas.Listing;

// TODO: make sure nothing changes when the ORM is done. This is the DTO for creating a listing, so it should be separate from the Listing model.
public class CreateListingDto
{
    public string Isbn { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal WeightGrams { get; set; }
    public BookCondition Condition { get; set; }
}