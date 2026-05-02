using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Listing;

namespace Book_Exchange.Services.Interfaces;
// TODO: Once ORM is implemented make sure nothing changes. 
public interface IListingService
{
    Task<Listing> CreateListingAsync(CreateListingDto dto, Guid userId);
    Task<Listing> GetListingByIdAsync(Guid listingId);
    Task<IEnumerable<Listing>> GetListingsByUserIdAsync(Guid userId);
    Task UpdateListingAsync(Guid id, UpdateListingDto dto, Guid userId);
    Task DeleteListingAsync(Guid id, Guid userId);
}