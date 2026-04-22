using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Exchange.Models;

[Table("transaction_listings")]
public class TransactionListing
{
    [Column("transaction_id")]
    public Guid TransactionId { get; set; }
    [Column("listing_id")]
    public Guid ListingId { get; set; }
    public Transaction Transaction { get; set; } = null!;
    public Listing Listing { get; set; } = null!;

}