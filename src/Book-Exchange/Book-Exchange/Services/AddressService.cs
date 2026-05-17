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
            .OrderByDescending(a => a.IsDefault)
            .ThenByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<Address?> GetDefaultAddressAsync(Guid userId)
    {
        return await _context.Addresses
            .FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefault);
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

        var userHasAnyAddress = await _context.Addresses
            .AnyAsync(a => a.UserId == userId);

        var shouldBeDefault = !userHasAnyAddress || dto.IsDefault;

        if (shouldBeDefault)
        {
            await ClearDefaultAddressAsync(userId);
        }

        var address = new Address
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FullName = googleAddress.FormattedAddress,
            GooglePlaceId = googleAddress.PlaceId,
            IsDefault = shouldBeDefault,
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

        var duplicateExists = await _context.Addresses.AnyAsync(a =>
            a.UserId == userId &&
            a.Id != addressId &&
            a.GooglePlaceId == dto.GooglePlaceId);

        if (duplicateExists)
            throw new InvalidOperationException("This address already exists.");

        if (dto.IsDefault && !address.IsDefault)
        {
            var currentDefaultAddresses = await _context.Addresses
                .Where(a => a.UserId == userId && a.IsDefault && a.Id != addressId)
                .ToListAsync();

            foreach (var defaultAddress in currentDefaultAddresses)
            {
                defaultAddress.IsDefault = false;
            }

            await _context.SaveChangesAsync();

            address.IsDefault = true;
        }

        address.FullName = googleAddress.FormattedAddress;
        address.GooglePlaceId = googleAddress.PlaceId;

        await _context.SaveChangesAsync();

        return address;
    }

    public async Task SetDefaultAddressAsync(Guid addressId, Guid userId)
    {
        var address = await GetAddressByIdAsync(addressId, userId);

        var currentDefaultAddresses = await _context.Addresses
            .Where(a => a.UserId == userId && a.IsDefault && a.Id != addressId)
            .ToListAsync();

        foreach (var defaultAddress in currentDefaultAddresses)
        {
            defaultAddress.IsDefault = false;
        }

        await _context.SaveChangesAsync();

        address.IsDefault = true;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAddressAsync(Guid addressId, Guid userId)
    {
        var address = await GetAddressByIdAsync(addressId, userId);

        var isUsedInShipment = await _context.Shipments.AnyAsync(s =>
            s.SenderAddressId == addressId ||
            s.ReceiverAddressId == addressId);

        if (isUsedInShipment)
        {
            throw new InvalidOperationException(
                "This address cannot be deleted because it is already used in a shipment.");
        }

        var wasDefault = address.IsDefault;

        _context.Addresses.Remove(address);
        await _context.SaveChangesAsync();

        if (wasDefault)
        {
            var nextAddress = await _context.Addresses
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .FirstOrDefaultAsync();

            if (nextAddress != null)
            {
                nextAddress.IsDefault = true;
                await _context.SaveChangesAsync();
            }
        }
    }

    private async Task ClearDefaultAddressAsync(Guid userId)
    {
        var defaultAddresses = await _context.Addresses
            .Where(a => a.UserId == userId && a.IsDefault)
            .ToListAsync();

        foreach (var address in defaultAddresses)
        {
            address.IsDefault = false;
        }
    }
}