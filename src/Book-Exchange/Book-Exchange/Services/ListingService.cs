using Book_Exchange.Data;
using Book_Exchange.Models;
using Book_Exchange.Models.DTOs;
using Book_Exchange.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Book_Exchange.Services;

public class ListingService : IListingService
{
    // TODO: Implement the methods defined in IListingService once ORM is set up and database context is available.
    // Book condition must be specified (Like New, Very Good, Good, Acceptable, Poor)
    // Price must be non-negative
    // Weight must be greater than zero
    // ISBN must be a valid format
    // Each listing has a transaction status lifecycle: Active, Pending, Completed, Cancelled
    // A listing's transaction status must follow valid transitions only (no skipping states)
    // A listing must have a type BuySell, BookSwap, BookSwapWithCash
    // A user cannot create an exchange request against their own listing
}
