using Book_Exchange.Models;
using Book_Exchange.Models.DTOs;

namespace Book_Exchange.Services.Interfaces;

public interface IListingService
{
    Task<Listing> CreateListingAsync(CreateListingDto dto, Guid userId);
    Task<Listing> GetListingByIdAsync(Guid listingId);
    Task<IEnumerable<Listing>> GetListingsByUserIdAsync(Guid userId);
    Task UpdateListingAsync(Guid id, UpdateListingDto dto, Guid userId);
    Task DeleteListingAsync(Guid id, Guid userId);
}