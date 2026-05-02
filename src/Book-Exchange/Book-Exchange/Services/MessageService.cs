using Book_Exchange.Models;
using Book_Exchange.Services.Interfaces;
using Book_Exchange.Areas.Message;
using Book_Exchange.Data;

namespace Book_Exchange.Services;

public class MessageService : IMessageService
{
    /// TODO: Once ORM is implemented make sure nothing changes. 

    private readonly ApplicationDbContext _context;

    // SendMessageAsync
    // - SenderId is taken from the logged-in user, not from a form
    // - ReceiverId must reference a valid existing user
    // - A user cannot send a message to themselves
    // - MessageText must not be null or whitespace
    // - ListingId, ExchangeRequestId, and TransactionId are all optional context references
    // - If provided, ListingId must reference a valid existing listing
    // - If provided, ExchangeRequestId must reference a valid existing exchange request
    // - If provided, TransactionId must reference a valid existing transaction
    // - Message is created with status MessageStatus.Sent
    public Task<Message> SendMessageAsync(SendMessageDto dto, Guid senderId)
    {
        throw new NotImplementedException();
    }

    // GetMessageByIdAsync
    // - Returns the message if it exists
    // - Throws KeyNotFoundException if the message does not exist
    public Task<Message> GetMessageByIdAsync(Guid messageId)
    {
        throw new NotImplementedException();
    }

    // GetConversationAsync
    // - Returns all messages between the two users, ordered by CreatedAt ascending
    // - Includes both directions (userId → otherUserId and otherUserId → userId)
    // - Returns an empty list if no messages exist between the two users
    public Task<IEnumerable<Message>> GetConversationAsync(Guid userId, Guid otherUserId)
    {
        throw new NotImplementedException();
    }

    // GetInboxAsync
    // - Returns a summary of distinct conversations for the given user
    // - Each summary includes the other participant, the latest message, and the unread count
    // - Ordered by most recent message descending
    // - Returns an empty list if the user has no messages
    public Task<IEnumerable<ConversationSummaryDto>> GetInboxAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    // MarkAsReadAsync
    // - Only the receiver of the message can mark it as read
    // - Throws UnauthorizedAccessException if userId is not the receiver
    // - Throws KeyNotFoundException if the message does not exist
    // - Sets Status to MessageStatus.Read
    public Task MarkAsReadAsync(Guid messageId, Guid userId)
    {
        throw new NotImplementedException();
    }

    // MarkConversationAsReadAsync
    // - Marks all messages in the conversation between userId and otherUserId as read
    // - No-op if there are no messages in the conversation
    public Task MarkConversationAsReadAsync(Guid userId, Guid otherUserId)
    {
        throw new NotImplementedException();
    }

    // GetUnreadCountAsync
    // - Returns the total count of messages where ReceiverId == userId and Status != Read
    // - Returns 0 if no unread messages exist
    public Task<int> GetUnreadCountAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

}