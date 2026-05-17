namespace Book_Exchange.Models;

public class WishlistItem
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public string Isbn { get; set; } = null!;

    public bool IsActive { get; set; } = true;
}