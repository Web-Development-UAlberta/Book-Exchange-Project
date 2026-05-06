using Book_Exchange.Data;
using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Address;
using Book_Exchange.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Book_Exchange.Services;

public class AddressService : IAddressService
{
    private readonly ApplicationDbContext _context;
    private readonly IPlaceApiService _placeApiService;

    public AddressService(ApplicationDbContext context, IPlaceApiService placeApiService)
    {
        _context = context;
        _placeApiService = placeApiService;
    }

    public async Task<Address> GetAddressByIdAsync(Guid addressId, Guid userId)
    {
        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);

        if (address == null)
            throw new KeyNotFoundException("Address not found.");

        return address;
    }

    public async Task<IEnumerable<Address>> GetAddressesByUserIdAsync(Guid userId)
    {
        return await _context.Addresses
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<Address> CreateAddressAsync(CreateAddressDto dto, Guid userId)
    {
        var googleAddress = await _placeApiService.GetAddressByPlaceIdAsync(dto.GooglePlaceId);

        if (googleAddress == null)
            throw new InvalidOperationException("Invalid Google Place Id.");

        var alreadyExists = await _context.Addresses.AnyAsync(a =>
            a.UserId == userId &&
            a.GooglePlaceId == dto.GooglePlaceId);

        if (alreadyExists)
            throw new InvalidOperationException("This address already exists.");

        var address = new Address
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FullName = googleAddress.FormattedAddress,
            GooglePlaceId = googleAddress.PlaceId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Addresses.Add(address);
        await _context.SaveChangesAsync();

        return address;
    }

    public async Task<Address> UpdateAddressAsync(Guid addressId, UpdateAddressDto dto, Guid userId)
    {
        var address = await GetAddressByIdAsync(addressId, userId);

        var googleAddress = await _placeApiService.GetAddressByPlaceIdAsync(dto.GooglePlaceId);

        if (googleAddress == null)
            throw new InvalidOperationException("Invalid Google Place Id.");

        address.FullName = googleAddress.FormattedAddress;
        address.GooglePlaceId = googleAddress.PlaceId;

        await _context.SaveChangesAsync();

        return address;
    }

    public async Task DeleteAddressAsync(Guid addressId, Guid userId)
    {
        var address = await GetAddressByIdAsync(addressId, userId);

        _context.Addresses.Remove(address);
        await _context.SaveChangesAsync();
    }
}