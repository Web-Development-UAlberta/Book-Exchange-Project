using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Reflection;

namespace Book_Exchange.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public ICollection<Listing> Listings { get; set; } = new List<Listing>();
}
