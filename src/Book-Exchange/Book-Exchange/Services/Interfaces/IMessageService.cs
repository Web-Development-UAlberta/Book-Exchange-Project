using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Message;

namespace Book_Exchange.Services.Interfaces;
// TODO: Once ORM is implemented make sure nothing changes. 
public interface IMessageService
{
    Task<Message> SendMessageAsync(SendMessageDto dto, Guid senderId);

    Task<Message> GetMessageByIdAsync(Guid messageId);

    Task<IEnumerable<Message>> GetConversationAsync(Guid userId, Guid otherUserId);

    Task<IEnumerable<ConversationSummaryDto>> GetInboxAsync(Guid userId);

    Task MarkAsReadAsync(Guid messageId, Guid userId);

    Task MarkConversationAsReadAsync(Guid userId, Guid otherUserId);

    Task<int> GetUnreadCountAsync(Guid userId);
}