using Book_Exchange.Data;
using Book_Exchange.Services.Interfaces;
using Book_Exchange.Models;
using Microsoft.EntityFrameworkCore;
using Book_Exchange.Models.DTOs.Listing;

namespace Book_Exchange.Services;

public class ListingService : IListingService
{
    // TODO: Implement the methods defined in IListingService once ORM is set up and database context is available.

    // private readonly ApplicationDbContext _context;

    // CreateListingAsync
    // Isbn must be valid ISBN-10 or ISBN-13 format
    // Condition must be specified (Like New, Very Good, Good, Acceptable, Poor)
    // Price must be non-negative
    // WeightGrams must be greater than zero
    // Status is set to Active on creation
    // UserId is take from the logged in user, not from a form
    public Task<Listing> CreateListingAsync(CreateListingDto dto, Guid userId)
    {
        throw new NotImplementedException();
    }

    // GetListingByIdAsync
    // Returns the listing if it exists
    // Throws an exception if the listing does not exist
    public Task<Listing> GetListingByIdAsync(Guid listingId)
    {
        throw new NotImplementedException();
    }

    // GetListingsByUserIdAsync
    // Returns all listings created by the specified user
    // Returns an empty list if the user has no listings
    public Task<IEnumerable<Listing>> GetListingsByUserIdAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    // UpdateListingAsync
    // Only the user who created the listing can update it (authorization check needed)
    // Price must be non-negative
    // WeightGrams must be greater than zero
    // Status transitions must be valid (Active -> Pending -> Completed or Cancelled)
    // Cannot update a listing that is already Completed or Cancelled
    public Task UpdateListingAsync(Guid id, UpdateListingDto dto, Guid userId)
    {
        throw new NotImplementedException();
    }

    // DeleteListingAsync
    // Only the user who created the listing can delete it (authorization check needed)
    // Cannot delete a listing that is pending, completed, or cancelled
    public Task DeleteListingAsync(Guid id, Guid userId)
    {
        throw new NotImplementedException();
    }
}
