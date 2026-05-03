using Book_Exchange.Models;
using System.ComponentModel.DataAnnotations;

namespace Book_Exchange.Models.DTOs.Listing;

// TODO: make sure nothing changes when the ORM is done. This is the DTO for updating a listing, so it should be separate from the Listing model.
public class UpdateListingDto
{
    [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Weight is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Weight must be greater than 0.")]
    public int WeightGrams { get; set; }

    [Required(ErrorMessage = "Condition is required.")]
    public BookCondition Condition { get; set; }

}