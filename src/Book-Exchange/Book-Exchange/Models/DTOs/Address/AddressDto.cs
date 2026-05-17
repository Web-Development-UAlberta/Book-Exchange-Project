namespace Book_Exchange.Models.DTOs.Address;

public class AddressDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string GooglePlaceId { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}
