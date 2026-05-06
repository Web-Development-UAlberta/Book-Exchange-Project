namespace Book_Exchange.Models.DTOs.Address;

public class PlaceAddressDto
{
    public string PlaceId { get; set; } = string.Empty;
    public string FormattedAddress { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}