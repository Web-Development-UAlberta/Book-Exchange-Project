using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Reflection;

namespace Book_Exchange.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public ICollection<Listing> Listings { get; set; } = new List<Listing>();
    public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
    public ICollection<ExchangeRequest> ExchangeRequests { get; set; } = new List<ExchangeRequest>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<Message> SentMessages { get; set; } = new List<Message>();
    public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
    public ICollection<TransactionStatusHistory> TransactionStatusUpdatedByUser { get; set; } = new List<TransactionStatusHistory>();

}
