using System.ComponentModel.DataAnnotations;

namespace Book_Exchange.Models.DTOs.Address;

public class CreateAddressDto
{
    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = null!;

    // External reference from Google Places API — not an internal FK
    [Required]
    [MaxLength(500)]
    public string GooglePlaceId { get; set; } = null!;
}