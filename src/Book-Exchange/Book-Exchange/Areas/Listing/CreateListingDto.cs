using Book_Exchange.Models;
using System.ComponentModel.DataAnnotations;

namespace Book_Exchange.Areas.Listing;

// TODO: make sure nothing changes when the ORM is done. This is the DTO for creating a listing, so it should be separate from the Listing model.
public class CreateListingDto
{
    [Required(ErrorMessage = "ISBN is required.")]
    [RegularExpression(@"^\d{10}(\d{3})?$", ErrorMessage = "ISBN must be a 10 or 13 digit number.")]
    public string Isbn { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Weight is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Weight must be greater than 0.")]
    public int WeightGrams { get; set; }

    [Required(ErrorMessage = "Condition is required.")]
    public BookCondition Condition { get; set; }
}