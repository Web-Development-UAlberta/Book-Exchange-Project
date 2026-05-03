using Xunit;
using Moq;
using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Message;
using Book_Exchange.Services.Interfaces;

// Message Tests
// Covers: UT-MSG-01 through UT-MSG-10 (Unit Tests)
namespace Book_Exchange.Tests.BackEnd;

// UNIT TESTS
// TODO: Once ORM is revised run and confirm tests pass
// public class MessageServiceUnitTests
// {
//     private readonly Mock<IMessageService> _serviceMock;

//     public MessageServiceUnitTests()
//     {
//         _serviceMock = new Mock<IMessageService>();
//     }

//     /// <summary>
//     /// UT-MSG-01: Send message with valid data
//     /// Expected: Message is created successfully.
//     /// </summary>
//     /// <returns>
//     /// Message with SenderId, ReceiverId, MessageText populated and IsRead = false.
//     /// </returns>
//     [Fact]
//     public async Task UT_MSG_01_SendValidMessage_ReturnsMessageWithIsReadFalse()
//     {
//         var senderId = Guid.NewGuid();
//         var receiverId = Guid.NewGuid();

//         var dto = new SendMessageDto
//         {
//             ReceiverId = receiverId,
//             MessageText = "Hello, is this book still available?"
//         };

//         var expectedMessage = new Message
//         {
//             Id = Guid.NewGuid(),
//             SenderId = senderId,
//             ReceiverId = receiverId,
//             MessageText = dto.MessageText,
//             IsRead = false,
//             CreatedAt = DateTime.UtcNow
//         };

//         _serviceMock
//             .Setup(s => s.SendMessageAsync(dto, senderId))
//             .ReturnsAsync(expectedMessage);

//         var result = await _serviceMock.Object.SendMessageAsync(dto, senderId);

//         Assert.NotNull(result);
//         Assert.Equal(senderId, result.SenderId);
//         Assert.Equal(receiverId, result.ReceiverId);
//         Assert.Equal(dto.MessageText, result.MessageText);
//         Assert.False(result.IsRead);
//     }

//     /// <summary>
//     /// UT-MSG-02: Send message to self.
//     /// Expected: Validation fails
//     /// </summary>
//     /// <returns>
//     /// InvalidOperationException with message "A user cannot send a message to themselves."
//     /// </returns>
//     [Fact]
//     public async Task UT_MSG_02_SendMessageToSelf_ThrowsInvalidOperationException()
//     {
//         var userId = Guid.NewGuid();

//         var dto = new SendMessageDto
//         {
//             ReceiverId = userId,
//             MessageText = "Talking to myself."
//         };

//         _serviceMock
//             .Setup(s => s.SendMessageAsync(dto, userId))
//             .ThrowsAsync(new InvalidOperationException("A user cannot send a message to themselves."));

//         await Assert.ThrowsAsync<InvalidOperationException>(
//             () => _serviceMock.Object.SendMessageAsync(dto, userId));
//     }

//     /// <summary>
//     /// UT-MSG-03: Send message with empty text.
//     /// Expected: Validation fails.
//     /// </summary>
//     /// <returns>
//     /// ArgumentException with message "Message text cannot be empty."
//     /// </returns>
//     [Fact]
//     public async Task UT_MSG_03_SendMessageWithEmptyText_ThrowsArgumentException()
//     {
//         var senderId = Guid.NewGuid();

//         var dto = new SendMessageDto
//         {
//             ReceiverId = Guid.NewGuid(),
//             MessageText = "   "
//         };

//         _serviceMock
//             .Setup(s => s.SendMessageAsync(dto, senderId))
//             .ThrowsAsync(new ArgumentException("Message text cannot be empty."));

//         await Assert.ThrowsAsync<ArgumentException>(
//             () => _serviceMock.Object.SendMessageAsync(dto, senderId));
//     }

//     /// <summary>
//     /// UT-MSG-04: Send message to non-existent receiver.
//     /// Expected: Validation fails.
//     /// </summary>
//     /// <returns>
//     /// KeyNotFoundException indicating the receiver was not found.
//     /// </returns>
//     [Fact]
//     public async Task UT_MSG_04_SendMessageToNonExistentReceiver_ThrowsKeyNotFoundException()
//     {
//         var senderId = Guid.NewGuid();

//         var dto = new SendMessageDto
//         {
//             ReceiverId = Guid.NewGuid(),
//             MessageText = "Hello?"
//         };

//         _serviceMock
//             .Setup(s => s.SendMessageAsync(dto, senderId))
//             .ThrowsAsync(new KeyNotFoundException("Receiver user not found."));

//         await Assert.ThrowsAsync<KeyNotFoundException>(
//             () => _serviceMock.Object.SendMessageAsync(dto, senderId));
//     }

//     /// <summary>
//     /// UT-MSG-05: Retrieve a message by its ID.
//     /// Expected: The correct message is returned.
//     /// </summary>
//     /// <returns>
//     /// Message with the matching Id.
//     /// </returns>
//     [Fact]
//     public async Task UT_MSG_05_GetMessageById_ExistingId_ReturnsMessage()
//     {
//         var messageId = Guid.NewGuid();

//         var expectedMessage = new Message
//         {
//             Id = messageId,
//             SenderId = Guid.NewGuid(),
//             ReceiverId = Guid.NewGuid(),
//             MessageText = "Hey there.",
//             IsRead = false,
//             CreatedAt = DateTime.UtcNow
//         };

//         _serviceMock
//             .Setup(s => s.GetMessageByIdAsync(messageId))
//             .ReturnsAsync(expectedMessage);

//         var result = await _serviceMock.Object.GetMessageByIdAsync(messageId);

//         Assert.NotNull(result);
//         Assert.Equal(messageId, result.Id);
//     }

//     /// <summary>
//     /// UT-MSG-06: Retrieve a message using an ID that does not exist.
//     /// Expected: Validation fails.
//     /// </summary>
//     /// <returns>
//     /// KeyNotFoundException indicating the message was not found.
//     /// </returns>
//     [Fact]
//     public async Task UT_MSG_06_GetMessageById_NonExistentId_ThrowsKeyNotFoundException()
//     {
//         var messageId = Guid.NewGuid();

//         _serviceMock
//             .Setup(s => s.GetMessageByIdAsync(messageId))
//             .ThrowsAsync(new KeyNotFoundException("Message not found."));

//         await Assert.ThrowsAsync<KeyNotFoundException>(
//             () => _serviceMock.Object.GetMessageByIdAsync(messageId));
//     }

//     /// <summary>
//     /// UT-MSG-07: Mark message as read by the receiver.
//     /// Expected: Operation completes successfully. IsRead is set to true.
//     /// </summary>
//     /// <returns>
//     /// Task completes without exception. MarkAsReadAsync is called once with correct parameters.
//     /// </returns>
//     [Fact]
//     public async Task UT_MSG_07_MarkAsRead_ByReceiver_CompletesSuccessfully()
//     {
//         var messageId = Guid.NewGuid();
//         var receiverId = Guid.NewGuid();

//         _serviceMock
//             .Setup(s => s.MarkAsReadAsync(messageId, receiverId))
//             .Returns(Task.CompletedTask);

//         await _serviceMock.Object.MarkAsReadAsync(messageId, receiverId);

//         _serviceMock.Verify(s => s.MarkAsReadAsync(messageId, receiverId), Times.Once);
//     }

//     /// <summary>
//     /// UT-MSG-07b: A user who is not the receiver attempts to mark a message as read.
//     /// Expected: UnauthorizedAccessException is thrown.
//     /// </summary>
//     /// <returns>
//     /// UnauthorizedAccessException with message "Only the receiver can mark this message as read."
//     /// </returns>
//     [Fact]
//     public async Task UT_MSG_07b_MarkAsRead_ByNonReceiver_ThrowsUnauthorizedAccessException()
//     {
//         var messageId = Guid.NewGuid();
//         var nonReceiverId = Guid.NewGuid();

//         _serviceMock
//             .Setup(s => s.MarkAsReadAsync(messageId, nonReceiverId))
//             .ThrowsAsync(new UnauthorizedAccessException("Only the receiver can mark this message as read."));

//         await Assert.ThrowsAsync<UnauthorizedAccessException>(
//             () => _serviceMock.Object.MarkAsReadAsync(messageId, nonReceiverId));
//     }

//     /// <summary>
//     /// UT-MSG-08: Get unread message count.
//     /// Expected: Correct count of unread messages is received.
//     /// </summary>
//     /// <returns>
//     /// Integer count of unread messages for that user.
//     /// </returns>
//     [Fact]
//     public async Task UT_MSG_08_GetUnreadCount_ReturnsCorrectCount()
//     {
//         var userId = Guid.NewGuid();
//         int expectedCount = 4;

//         _serviceMock
//             .Setup(s => s.GetUnreadCountAsync(userId))
//             .ReturnsAsync(expectedCount);

//         var result = await _serviceMock.Object.GetUnreadCountAsync(userId);

//         Assert.Equal(expectedCount, result);
//     }

//     /// <summary>
//     /// UT-MSG-08b: Retrieve unread count when user has no unread messages.
//     /// Expected: Returns 0.
//     /// </summary>
//     /// <returns>
//     /// 0.
//     /// </returns>
//     [Fact]
//     public async Task UT_MSG_08b_GetUnreadCount_NoUnreadMessages_ReturnsZero()
//     {
//         var userId = Guid.NewGuid();

//         _serviceMock
//             .Setup(s => s.GetUnreadCountAsync(userId))
//             .ReturnsAsync(0);

//         var result = await _serviceMock.Object.GetUnreadCountAsync(userId);

//         Assert.Equal(0, result);
//     }

//     /// <summary>
//     /// UT-MSG-09: Get conversation between two users.
//     /// Expected: Messages are returned in ascending order
//     /// </summary>
//     /// <returns>
//     /// IEnumerable of Message ordered by CreatedAt ascending.
//     /// </returns>
//     [Fact]
//     public async Task UT_MSG_09_GetConversation_ReturnsMessagesInOrder()
//     {
//         var userId = Guid.NewGuid();
//         var otherUserId = Guid.NewGuid();
//         var now = DateTime.UtcNow;

//         var expectedMessages = new List<Message>
//         {
//             new Message { Id = Guid.NewGuid(), SenderId = userId,      ReceiverId = otherUserId, MessageText = "Hey",        IsRead = true,  CreatedAt = now.AddMinutes(-10) },
//             new Message { Id = Guid.NewGuid(), SenderId = otherUserId, ReceiverId = userId,      MessageText = "Hi!",        IsRead = true,  CreatedAt = now.AddMinutes(-8)  },
//             new Message { Id = Guid.NewGuid(), SenderId = userId,      ReceiverId = otherUserId, MessageText = "Still there?",IsRead = false, CreatedAt = now.AddMinutes(-2)  }
//         };

//         _serviceMock
//             .Setup(s => s.GetConversationAsync(userId, otherUserId))
//             .ReturnsAsync(expectedMessages);

//         var result = (await _serviceMock.Object.GetConversationAsync(userId, otherUserId)).ToList();

//         Assert.Equal(3, result.Count);
//         Assert.True(result[0].CreatedAt <= result[1].CreatedAt);
//         Assert.True(result[1].CreatedAt <= result[2].CreatedAt);
//     }

//     /// <summary>
//     /// UT-MSG-09b: Retrieve conversation when no messages exist between the two users.
//     /// Expected: Empty list is returned without throwing an exception.
//     /// </summary>
//     /// <returns>
//     /// Empty IEnumerable of Message.
//     /// </returns>
//     [Fact]
//     public async Task UT_MSG_09b_GetConversation_NoMessages_ReturnsEmptyList()
//     {
//         var userId = Guid.NewGuid();
//         var otherUserId = Guid.NewGuid();

//         _serviceMock
//             .Setup(s => s.GetConversationAsync(userId, otherUserId))
//             .ReturnsAsync(new List<Message>());

//         var result = await _serviceMock.Object.GetConversationAsync(userId, otherUserId);

//         Assert.NotNull(result);
//         Assert.Empty(result);
//     }

//     /// <summary>
//     /// UT-MSG-10: Get inbox for user.
//     /// Expected: Conversations are listed by latest messages in descending order. 
//     /// </summary>
//     /// <returns>
//     /// IEnumerable of ConversationSummaryDto.
//     /// </returns>
//     [Fact]
//     public async Task UT_MSG_10_GetInbox_WithConversations_ReturnsSummaries()
//     {
//         var userId = Guid.NewGuid();
//         var now = DateTime.UtcNow;

//         var expectedSummaries = new List<ConversationSummaryDto>
//         {
//             new ConversationSummaryDto
//             {
//                 OtherUserId   = Guid.NewGuid(),
//                 OtherUserName = "Alice",
//                 LatestMessageText = "Sounds good!",
//                 LatestMessageAt   = now.AddMinutes(-5),
//                 UnreadCount       = 2
//             },

//             new ConversationSummaryDto
//             {
//                 OtherUserId   = Guid.NewGuid(),
//                 OtherUserName = "Bob",
//                 LatestMessageText = "Is it available?",
//                 LatestMessageAt   = now.AddHours(-2),
//                 UnreadCount       = 0
//             }
//         };

//         _serviceMock
//             .Setup(s => s.GetInboxAsync(userId))
//             .ReturnsAsync(expectedSummaries);

//         var result = (await _serviceMock.Object.GetInboxAsync(userId)).ToList();

//         Assert.Equal(2, result.Count);
//         Assert.True(result[0].LatestMessageAt >= result[1].LatestMessageAt);
//         Assert.Equal(2, result[0].UnreadCount);
//     }

//     /// <summary>
//     /// UT-MSG-11: Mark conversation as read.
//     /// Expected: All messages in the conversation are marked as read.
//     /// </summary>
//     /// <returns>
//     /// Operation completes successfully. MarkConversationAsReadAsync is called once.
//     /// </returns>
//     [Fact]
//     public async Task UT_MSG_11_MarkConversationAsRead_CompletesSuccessfully()
//     {
//         var userId = Guid.NewGuid();
//         var otherUserId = Guid.NewGuid();

//         _serviceMock
//             .Setup(s => s.MarkConversationAsReadAsync(userId, otherUserId))
//             .Returns(Task.CompletedTask);

//         await _serviceMock.Object.MarkConversationAsReadAsync(userId, otherUserId);

//         _serviceMock.Verify(s => s.MarkConversationAsReadAsync(userId, otherUserId), Times.Once);
//     }

//     /// <summary>
//     /// UT-MSG-11b: Mark conversation as read when no messages exist between the two users.
//     /// Expected: No-op — completes without throwing.
//     /// </summary>
//     /// <returns>
//     /// Task completes without exception.
//     /// </returns>
//     [Fact]
//     public async Task UT_MSG_11b_MarkConversationAsRead_NoMessages_IsNoOp()
//     {
//         var userId = Guid.NewGuid();
//         var otherUserId = Guid.NewGuid();

//         _serviceMock
//             .Setup(s => s.MarkConversationAsReadAsync(userId, otherUserId))
//             .Returns(Task.CompletedTask);

//         var exception = await Record.ExceptionAsync(
//             () => _serviceMock.Object.MarkConversationAsReadAsync(userId, otherUserId));

//         Assert.Null(exception);
//     }
// }