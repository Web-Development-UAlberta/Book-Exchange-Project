namespace Book_Exchange.Models.DTOs;

// TODO: make sure nothing changes when the ORM is done. This is the DTO for updating a listing, so it should be separate from the Listing model.
public class UpdateListingDto
{
    public decimal Price { get; set; }
    public decimal WeightKg { get; set; }
    public BookCondition Condition { get; set; }
    public ListingType Type { get; set; }
}