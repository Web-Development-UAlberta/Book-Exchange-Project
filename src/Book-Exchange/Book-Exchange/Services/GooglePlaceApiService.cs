using System.Text.Json;
using Book_Exchange.Models.DTOs.Address;
using Book_Exchange.Services.Interfaces;

namespace Book_Exchange.Services;

public class GooglePlaceApiService : IPlaceApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public GooglePlaceApiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<IEnumerable<PlaceAddressDto>> SearchAddressAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return new List<PlaceAddressDto>();

        var apiKey = GetApiKey();

        var url =
            "https://maps.googleapis.com/maps/api/place/autocomplete/json" +
            $"?input={Uri.EscapeDataString(query)}" +
            "&types=address" +
            "&components=country:ca" +
            $"&key={apiKey}";

        var json = await _httpClient.GetStringAsync(url);

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var status = root.GetProperty("status").GetString();

        if (status != "OK" && status != "ZERO_RESULTS")
            throw new Exception($"Google API error: {status}");

        var results = new List<PlaceAddressDto>();

        if (status == "ZERO_RESULTS")
            return results;

        foreach (var item in root.GetProperty("predictions").EnumerateArray())
        {
            results.Add(new PlaceAddressDto
            {
                PlaceId = item.GetProperty("place_id").GetString() ?? "",
                FormattedAddress = item.GetProperty("description").GetString() ?? ""
            });
        }

        return results;
    }

    public async Task<PlaceAddressDto?> GetPlaceIdByAddressAsync(string address)
    {
        var results = await SearchAddressAsync(address);
        return results.FirstOrDefault();
    }

    public async Task<PlaceAddressDto?> GetAddressByPlaceIdAsync(string placeId)
    {
        if (string.IsNullOrWhiteSpace(placeId))
            return null;

        var apiKey = GetApiKey();

        var url =
            "https://maps.googleapis.com/maps/api/place/details/json" +
            $"?place_id={Uri.EscapeDataString(placeId)}" +
            "&fields=place_id,formatted_address" +
            $"&key={apiKey}";

        var json = await _httpClient.GetStringAsync(url);

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var status = root.GetProperty("status").GetString();

        if (status != "OK")
            throw new Exception($"Google API error: {status}");

        var result = root.GetProperty("result");

        return new PlaceAddressDto
        {
            PlaceId = result.GetProperty("place_id").GetString() ?? "",
            FormattedAddress = result.GetProperty("formatted_address").GetString() ?? ""
        };
    }

    private string GetApiKey()
    {
        var apiKey = _configuration["GoogleMaps:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new Exception("Google Maps API key is missing.");

        return apiKey;
    }
}