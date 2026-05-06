namespace Book_Exchange.Models.DTOs.Address;

public class GooglePlaceSearchResultDto
{
    public string PlaceId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}