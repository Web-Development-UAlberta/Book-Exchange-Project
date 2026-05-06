using Book_Exchange.Data;
using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Listing;
using Book_Exchange.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Book_Exchange.Services;

public class ListingService : IListingService
{
    private readonly ApplicationDbContext _context;
    private readonly IMatchingService _matchingService;

    public ListingService(
        ApplicationDbContext context,
        IMatchingService matchingService)
    {
        _context = context;
        _matchingService = matchingService;
    }

    public async Task<Listing> CreateListingAsync(CreateListingDto dto, Guid userId)
    {
        var isbn = dto.Isbn.Trim().Replace("-", "").ToUpper();

        if (!IsValidIsbn(isbn))
        {
            throw new ArgumentException("ISBN must be a 10 or 13 digit number.");
        }

        if (dto.Price < 0)
        {
            throw new ArgumentException("Price must be greater than or equal to 0.");
        }

        if (dto.WeightGrams <= 0)
        {
            throw new ArgumentException("Weight must be greater than 0.");
        }

        var listing = new Listing
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Isbn = isbn,
            Condition = dto.Condition,
            Price = dto.Price,
            WeightGrams = dto.WeightGrams,
            CreatedAt = DateTime.UtcNow
        };

        _context.Listings.Add(listing);
        await _context.SaveChangesAsync();

        await _matchingService.CreateMatchNotificationsAsync(listing);

        return listing;
    }

    private static bool IsValidIsbn(string isbn)
    {
        return Regex.IsMatch(isbn, @"^([0-9]{13}|[0-9X]{10})$");
    }

    public async Task<Listing> GetListingByIdAsync(Guid listingId)
    {
        return await _context.Listings
            .Include(l => l.User)
            .FirstOrDefaultAsync(l => l.Id == listingId)
            ?? throw new KeyNotFoundException("Listing not found.");
    }

    public async Task<IEnumerable<Listing>> GetListingsByUserIdAsync(Guid userId)
    {
        return await _context.Listings
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task UpdateListingAsync(Guid id, UpdateListingDto dto, Guid userId)
    {
        var listing = await _context.Listings
            .FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId)
            ?? throw new UnauthorizedAccessException("Listing not found or not owned by user.");

        listing.Condition = dto.Condition;
        listing.Price = dto.Price;
        listing.WeightGrams = dto.WeightGrams;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteListingAsync(Guid id, Guid userId)
    {
        var listing = await _context.Listings
            .FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId)
            ?? throw new UnauthorizedAccessException("Listing not found or not owned by user.");

        _context.Listings.Remove(listing);
        await _context.SaveChangesAsync();
    }
}