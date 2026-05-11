using Book_Exchange.Data;
using Book_Exchange.Models;
using Microsoft.EntityFrameworkCore;

namespace UI;

// This fixture ensures that the transaction used in UI-REVIEW-05 is in a clean state (no existing review by the test user)
// before the test runs, and it cleans up any review created by the test after it finishes. 
// This allows UI-REVIEW-05 to be reliably repeatable without manual database resets.
public class FreshTransactionFixture : IAsyncLifetime
{
    private static readonly Guid TestUserId = new("aaaa0001-0000-0000-0000-000000000001");
    private static readonly Guid OtherUserId = new("aaaa0002-0000-0000-0000-000000000002");

    public Guid FreshTransactionId { get; } = new("99999999-0003-0000-0000-000000000003");
    public Guid RevieweeId => OtherUserId;

    private ApplicationDbContext _db = null!;

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(GetConnectionString())
            .Options;

        _db = new ApplicationDbContext(options);

        var staleReview = await _db.Reviews
            .FirstOrDefaultAsync(r =>
                r.TransactionId == FreshTransactionId &&
                r.ReviewerId == TestUserId);

        if (staleReview != null)
        {
            _db.Reviews.Remove(staleReview);
            await _db.SaveChangesAsync();
        }
    }

    public async Task DisposeAsync()
    {
        var review = await _db.Reviews
            .FirstOrDefaultAsync(r =>
                r.TransactionId == FreshTransactionId &&
                r.ReviewerId == TestUserId);

        if (review != null)
        {
            _db.Reviews.Remove(review);
            await _db.SaveChangesAsync();
        }

        await _db.DisposeAsync();
    }

    private static string GetConnectionString()
    {
        return Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=BookExchangeDb;Username=postgres;Password=postgres;";
    }
}