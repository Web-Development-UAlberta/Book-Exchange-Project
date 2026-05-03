using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Address;
using Book_Exchange.Services.Interfaces;

namespace Book_Exchange.Services;

public class AddressService : IAddressService
{
    // TODO: Implement once ORM is set up and database context is available.
    // private readonly ApplicationDbContext _context;
    // private readonly IGooglePlacesService _googlePlacesService;

    // public AddressService(ApplicationDbContext context, IGooglePlacesService googlePlacesService)
    // {
    //     _context = context;
    //     _googlePlacesService = googlePlacesService;
    // }

    // GetAddressByIdAsync
    // - Returns the address if it exists
    // - Throws KeyNotFoundException if the address does not exist
    // - UserId must match the address's UserId (users cannot access each other's addresses)
    public Task<Address> GetAddressByIdAsync(Guid addressId, Guid userId)
    {
        throw new NotImplementedException();
    }

    // GetAddressesByUserIdAsync
    // - Returns all addresses belonging to the user
    // - Returns empty list if the user has no saved addresses
    // - GooglePlaceId values are returned here — the controller/view layer is to 
    //    resolve these to human-readable addresses via the Google Places API before displaying them to the user
    public Task<IEnumerable<Address>> GetAddressesByUserIdAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    // CreateAddressAsync
    // - FullName must not be null or whitespace
    // - GooglePlaceId must not be null or whitespace
    // - BEFORE saving, validate the GooglePlaceId against the Google Places API via IGooglePlacesService. 
    //   Throw ArgumentException if the Place ID cannot be resolved (invalid, expired, or not found)
    // - Sets CreatedAt to UTC now
    // - Associates the address with the given userId
    public Task<Address> CreateAddressAsync(CreateAddressDto dto, Guid userId)
    {
        throw new NotImplementedException();
    }

    // UpdateAddressAsync
    // - Throws KeyNotFoundException if the address does not exist or does not belong to the user
    // - Only updates fields that are supplied in the DTO (partial update)
    // - If GooglePlaceId is being updated, validate the new value against the Google Places API
    //   via IGooglePlacesService BEFORE saving. Throw ArgumentException if the Place ID cannot
    //   be resolved (invalid, expired, or not found)
    // - If FullName is being updated, it must not be null or whitespace
    public Task<Address> UpdateAddressAsync(Guid addressId, UpdateAddressDto dto, Guid userId)
    {
        throw new NotImplementedException();
    }

    // DeleteAddressAsync
    // - Throws KeyNotFoundException if the address does not exist or does not belong to the user
    // - Throws InvalidOperationException if the address is currently referenced by an active shipment
    // - Hard deletes the address record
    // - No API call needed for deletion
    public Task DeleteAddressAsync(Guid addressId, Guid userId)
    {
        throw new NotImplementedException();
    }
}