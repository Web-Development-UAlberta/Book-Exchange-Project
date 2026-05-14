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

    public async Task<PlaceDistanceDto?> GetDistanceBetweenPlacesAsync(
    string originPlaceId,
    string destinationPlaceId)
    {
        if (string.IsNullOrWhiteSpace(originPlaceId) ||
            string.IsNullOrWhiteSpace(destinationPlaceId))
        {
            return null;
        }

        var apiKey = GetApiKey();

        var url = "https://routes.googleapis.com/distanceMatrix/v2:computeRouteMatrix";

        var requestBody = new
        {
            origins = new[]
            {
            new
            {
                waypoint = new
                {
                    placeId = originPlaceId
                }
            }
        },
            destinations = new[]
            {
            new
            {
                waypoint = new
                {
                    placeId = destinationPlaceId
                }
            }
        },
            travelMode = "DRIVE"
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, url);

        request.Headers.Add("X-Goog-Api-Key", apiKey);
        request.Headers.Add(
            "X-Goog-FieldMask",
            "originIndex,destinationIndex,distanceMeters,duration,status");

        request.Content = new StringContent(
            JsonSerializer.Serialize(requestBody),
            System.Text.Encoding.UTF8,
            "application/json");

        using var response = await _httpClient.SendAsync(request);

        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Google Routes API error: {response.StatusCode} - {json}");
        }

        using var doc = JsonDocument.Parse(json);

        var root = doc.RootElement;

        if (root.ValueKind != JsonValueKind.Array || root.GetArrayLength() == 0)
        {
            return null;
        }

        var route = root[0];

        if (!route.TryGetProperty("distanceMeters", out var distanceElement))
        {
            return null;
        }

        var distanceMeters = distanceElement.GetInt32();

        var duration = route.TryGetProperty("duration", out var durationElement)
            ? durationElement.GetString() ?? string.Empty
            : string.Empty;

        return new PlaceDistanceDto
        {
            DistanceMeters = distanceMeters,
            Duration = duration
        };
    }
}