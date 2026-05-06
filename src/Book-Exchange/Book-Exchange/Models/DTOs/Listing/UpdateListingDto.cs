using System.ComponentModel.DataAnnotations;
using Book_Exchange.Models;

namespace Book_Exchange.Models.DTOs.Listing;

public class UpdateListingDto
{
    [Required]
    public BookCondition Condition { get; set; }

    [Range(0, 10000)]
    public decimal Price { get; set; }

    [Range(1, 100000)]
    public int WeightGrams { get; set; }
}