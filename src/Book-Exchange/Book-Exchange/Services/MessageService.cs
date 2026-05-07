using Book_Exchange.Data;
using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Message;
using Book_Exchange.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Book_Exchange.Services;

public class MessageService : IMessageService
{
    private readonly ApplicationDbContext _context;

    public MessageService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// SendMessageAsync
    /// - SenderId is taken from the logged-in user, not from a form
    /// - ReceiverId must reference a valid existing user
    /// - A user cannot send a message to themselves
    /// - MessageText must not be null or whitespace
    /// - ListingId, ExchangeRequestId, and TransactionId are all optional context references
    /// - If provided, ListingId must reference a valid existing listing
    /// - If provided, ExchangeRequestId must reference a valid existing exchange request
    /// - If provided, TransactionId must reference a valid existing transaction
    /// - Message is created with IsRead = false and CreatedAt = DateTime.UtcNow
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="senderId"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="KeyNotFoundException"></exception>
    public async Task<Message> SendMessageAsync(SendMessageDto dto, Guid senderId)
    {
        if (senderId == dto.ReceiverId)
        {
            throw new ArgumentException("Cannot send a message to yourself.");
        }

        if (string.IsNullOrWhiteSpace(dto.MessageText))
        {
            throw new ArgumentException("Message text cannot be empty.");
        }

        var receiverExists = await _context.Users.AnyAsync(u => u.Id == dto.ReceiverId);
        if (!receiverExists)
        {
            throw new ArgumentException("That user does not exist.");
        }

        if (dto.ListingId.HasValue)
        {
            var listingExists = await _context.Listings.AnyAsync(l => l.Id == dto.ListingId.Value);
            if (!listingExists)
            {
                throw new KeyNotFoundException($"Listing {dto.ListingId} does not exist.");
            }
        }

        if (dto.ExchangeRequestId.HasValue)
        {
            var erExists = await _context.ExchangeRequests.AnyAsync(e => e.Id == dto.ExchangeRequestId.Value);
            if (!erExists)
            {
                throw new KeyNotFoundException($"Exchange request {dto.ExchangeRequestId} does not exist.");
            }
        }

        if (dto.TransactionId.HasValue)
        {
            var txExists = await _context.Transactions.AnyAsync(t => t.Id == dto.TransactionId.Value);
            if (!txExists)
            {
                throw new KeyNotFoundException($"Transaction {dto.TransactionId} does not exist.");
            }
        }

        var message = new Message
        {
            Id = Guid.NewGuid(),
            SenderId = senderId,
            ReceiverId = dto.ReceiverId,
            MessageText = dto.MessageText.Trim(),
            IsRead = false,
            ListingId = dto.ListingId,
            ExchangeRequestId = dto.ExchangeRequestId,
            TransactionId = dto.TransactionId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
        return message;
    }

    /// <summary>
    /// GetMessageByIdAsync
    /// - Returns the message if it exists
    /// - Throws KeyNotFoundException if the message does not exist
    /// </summary>
    /// <param name="messageId"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public async Task<Message> GetMessageByIdAsync(Guid messageId)
    {
        return await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .FirstOrDefaultAsync(m => m.Id == messageId) ?? throw new KeyNotFoundException($"Message {messageId} does not exist.");
    }

    /// <summary>
    /// GetConversationAsync
    /// - Returns all messages between the two users ordered by CreatedAt ascending
    /// - Covers both directions
    /// - Returns empty list if no messages exist
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="otherUserId"></param>
    public async Task<IEnumerable<Message>> GetConversationAsync(Guid userId, Guid otherUserId)
    {
        return await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Include(m => m.Listing)
            .Include(m => m.ExchangeRequest)
            .Include(m => m.Transaction)
            .Where(m =>
                (m.SenderId == userId && m.ReceiverId == otherUserId) ||
                (m.SenderId == otherUserId && m.ReceiverId == userId))
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// GetInboxAsync
    /// - Returns a summary of each distinct conversation for the given user
    /// - Includes: other participant, latest message text, latest message time, unread count
    /// - Ordered by most recent message descending
    /// </summary>
    /// <param name="userId"></param>
    public async Task<IEnumerable<ConversationSummaryDto>> GetInboxAsync(Guid userId)
    {
        var messages = await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Where(m => m.SenderId == userId || m.ReceiverId == userId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

        var grouped = messages
            .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
            .Select(g =>
            {
                var latest = g.First();
                var otherUser = latest.SenderId == userId ? latest.Receiver : latest.Sender;
                var unreadCount = g.Count(m => m.ReceiverId == userId && !m.IsRead);

                return new ConversationSummaryDto
                {
                    OtherUserId = otherUser.Id,
                    OtherUserName = otherUser.UserName ?? string.Empty,
                    LatestMessageText = latest.MessageText ?? string.Empty,
                    LatestMessageAt = latest.CreatedAt,
                    UnreadCount = unreadCount
                };
            })
            .OrderByDescending(s => s.LatestMessageAt)
            .ToList();

        return grouped;
    }

    /// <summary>
    /// MarkAsReadAsync
    /// - Only the receiver of the message can mark it as read
    /// - Throws UnauthorizedAccessException if userId is not the receiver
    /// - Throws KeyNotFoundException if the message does not exist
    /// </summary>
    /// <param name="messageId"></param>
    /// <param name="userId"></param>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="KeyNotFoundException"></exception>
    public async Task MarkAsReadAsync(Guid messageId, Guid userId)
    {
        var message = await _context.Messages.FindAsync(messageId)
            ?? throw new KeyNotFoundException($"Message {messageId} not found.");

        if (message.ReceiverId != userId)
            throw new UnauthorizedAccessException("Only the receiver can mark a message as read.");

        if (!message.IsRead)
        {
            message.IsRead = true;
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// MarkConversationAsReadAsync
    /// - Marks all unread messages sent to userId from otherUserId as read
    /// - No-op if there are no matching unread messages
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="otherUserId"></param>
    public async Task MarkConversationAsReadAsync(Guid userId, Guid otherUserId)
    {
        var unread = await _context.Messages
            .Where(m => m.ReceiverId == userId && m.SenderId == otherUserId && !m.IsRead)
            .ToListAsync();

        if (unread.Count == 0) return;

        foreach (var m in unread)
            m.IsRead = true;

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// GetUnreadCountAsync
    /// - Returns count of messages where ReceiverId == userId and IsRead == false
    /// - Returns 0 if none
    /// </summary>
    /// <param name="userId"></param>
    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        return await _context.Messages
            .CountAsync(m => m.ReceiverId == userId && !m.IsRead);
    }
}