using System.ComponentModel.DataAnnotations;

namespace Book_Exchange.Models.DTOs.Address;

public class UpdateAddressDto
{
    [MaxLength(200)]
    public string? FullName { get; set; }

    // External reference from Google Places API — not an internal FK
    [MaxLength(500)]
    public string? GooglePlaceId { get; set; }
}