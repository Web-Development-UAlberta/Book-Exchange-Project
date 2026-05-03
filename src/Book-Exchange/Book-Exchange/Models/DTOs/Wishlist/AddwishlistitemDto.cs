using System.ComponentModel.DataAnnotations;

namespace Book_Exchange.Models.DTOs.Wishlist;

public class AddWishlistItemDto
{
    [Required(ErrorMessage = "ISBN is required.")]
    [RegularExpression(@"^\d{10}(\d{3})?$", ErrorMessage = "ISBN must be a 10 or 13 digit number.")]
    public string Isbn { get; set; } = null!;
}