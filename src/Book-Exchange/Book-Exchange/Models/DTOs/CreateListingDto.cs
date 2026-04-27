namespace Book_Exchange.Models.DTOs;

// TODO: make sure nothing changes when the ORM is done. This is the DTO for creating a listing, so it should be separate from the Listing model.
public class CreateListingDto
{
    public Guid BookId { get; set; }
    public decimal Price { get; set; }
    public decimal WeightKg { get; set; }
    public BookCondition Condition { get; set; }
    public ListingType Type { get; set; }
}