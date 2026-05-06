namespace Book_Exchange.Services.Interfaces;

using Book_Exchange.Models.DTOs.Address;

public interface IPlaceApiService
{
    Task<PlaceAddressDto?> GetAddressByPlaceIdAsync(string placeId);
    Task<PlaceAddressDto?> GetPlaceIdByAddressAsync(string address);
    Task<IEnumerable<PlaceAddressDto>> SearchAddressAsync(string query);
}