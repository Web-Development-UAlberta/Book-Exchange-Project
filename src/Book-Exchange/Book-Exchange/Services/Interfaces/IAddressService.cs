using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Address;

namespace Book_Exchange.Services.Interfaces;

// TODO: Once ORM is implemented make sure nothing has changed.
public interface IAddressService
{
    Task<Address> GetAddressByIdAsync(Guid addressId, Guid userId);
    Task<IEnumerable<Address>> GetAddressesByUserIdAsync(Guid userId);
    Task<Address> CreateAddressAsync(CreateAddressDto dto, Guid userId);
    Task<Address> UpdateAddressAsync(Guid addressId, UpdateAddressDto dto, Guid userId);
    Task DeleteAddressAsync(Guid addressId, Guid userId);
}