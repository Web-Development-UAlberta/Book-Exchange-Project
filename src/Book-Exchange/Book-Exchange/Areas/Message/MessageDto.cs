using System.ComponentModel.DataAnnotations;

namespace Book_Exchange.Areas.Message;
// TODO: make sure nothing changes when the ORM is done. These are the DTOs for messaging, so they should be separate from the Message model.
public class SendMessageDto
{
    [Required]
    public Guid ReceiverId { get; set; }

    [Required]
    [StringLength(2000, MinimumLength = 1)]
    public string MessageText { get; set; } = string.Empty;

    // Optional context — at most one should be provided
    public Guid? ListingId { get; set; }
    public Guid? ExchangeRequestId { get; set; }
    public Guid? TransactionId { get; set; }
}

public class ConversationSummaryDto
{
    public Guid OtherUserId { get; set; }
    public string OtherUserName { get; set; } = string.Empty;
    public string LatestMessageText { get; set; } = string.Empty;
    public DateTime LatestMessageAt { get; set; }
    public int UnreadCount { get; set; }
}