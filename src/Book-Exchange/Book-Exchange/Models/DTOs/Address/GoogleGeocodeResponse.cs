namespace Book_Exchange.Models.DTOs.Address;

public class GoogleGeocodeResponse
{
    public List<GoogleGeocodeResult>? Results { get; set; }
}

public class GoogleGeocodeResult
{
    public string? PlaceId { get; set; }
    public string? FormattedAddress { get; set; }
    public GoogleGeometry? Geometry { get; set; }
}

public class GoogleGeometry
{
    public GoogleLatLng? Location { get; set; }
}

public class GoogleLatLng
{
    public double Lat { get; set; }
    public double Lng { get; set; }
}

public class GoogleTextSearchResponse
{
    public List<GooglePlaceResult>? Places { get; set; }
}

public class GooglePlaceResult
{
    public string? Id { get; set; }
    public string? FormattedAddress { get; set; }
    public GooglePlaceLocation? Location { get; set; }
}

public class GooglePlaceLocation
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
