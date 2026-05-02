using Xunit;
using Microsoft.EntityFrameworkCore;
using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Message;
using Book_Exchange.Services;
using Book_Exchange.Services.Interfaces;
using Book_Exchange.Data;

// Message Integration Tests
// Covers: IT-MSG-01 through IT-MSG-05

namespace Book_Exchange.Tests.Integration;

// TODO: Uncomment when MessageService is implemented
// public class MessageServiceIntegrationTests : IDisposable
// {
//     private readonly ApplicationDbContext _db;
//     private readonly IMessageService _service;
//
//     public MessageServiceIntegrationTests()
//     {
//         var options = new DbContextOptionsBuilder<ApplicationDbContext>()
//             .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
//             .Options;
//
//         _db = new ApplicationDbContext(options);
//         _service = new MessageService(_db);
//     }
//
//     public void Dispose() => _db.Dispose();
//
//     // Helper: seed two users into the in-memory database
//     private async Task<(ApplicationUser sender, ApplicationUser receiver)> SeedTwoUsersAsync()
//     {
//         var sender = new ApplicationUser { Id = Guid.NewGuid(), UserName = "sender_user" };
//         var receiver = new ApplicationUser { Id = Guid.NewGuid(), UserName = "receiver_user" };
//         _db.Users.AddRange(sender, receiver);
//         await _db.SaveChangesAsync();
//         return (sender, receiver);
//     }
//
//     // Helper: seed a message directly into the database
//     private async Task<Message> SeedMessageAsync(
//         Guid senderId,
//         Guid receiverId,
//         string text = "Hello",
//         bool isRead = false,
//         DateTime? createdAt = null)
//     {
//         var message = new Message
//         {
//             Id         = Guid.NewGuid(),
//             SenderId   = senderId,
//             ReceiverId = receiverId,
//             MessageText = text,
//             IsRead     = isRead,
//             CreatedAt  = createdAt ?? DateTime.UtcNow
//         };
//
//         _db.Messages.Add(message);
//         await _db.SaveChangesAsync();
//         return message;
//     }
//
//     /// <summary>
//     /// IT-MSG-01: Send a message between users.
//     /// Expected: Message is stored and visible in conversation thread.
//     /// </summary>
//     /// <returns>
//     /// Saved Message with IsRead = false retrievable from the database.
//     /// </returns>
//     [Fact]
//     public async Task IT_MSG_01_SendMessage_MessageIsSavedToDatabase()
//     {
//         var (sender, receiver) = await SeedTwoUsersAsync();
//
//         var dto = new SendMessageDto
//         {
//             ReceiverId  = receiver.Id,
//             MessageText = "Is this book still available?"
//         };
//
//         var result = await _service.SendMessageAsync(dto, sender.Id);
//
//         Assert.NotNull(result);
//         Assert.Equal(sender.Id, result.SenderId);
//         Assert.Equal(receiver.Id, result.ReceiverId);
//         Assert.Equal(dto.MessageText, result.MessageText);
//         Assert.False(result.IsRead);
//
//         var saved = await _db.Messages.FirstOrDefaultAsync(m => m.Id == result.Id);
//         Assert.NotNull(saved);
//         Assert.False(saved!.IsRead);
//     }
//
//     /// <summary>
//     /// IT-MSG-02: Conversation thread is returned in order
//     /// Expected: Messages between users is returned in ascending order
//     /// </summary>
//     /// <returns>
//     /// Ordered list of Message records covering both send directions, sorted by CreatedAt ascending.
//     /// </returns>
//     [Fact]
//     public async Task IT_MSG_02_GetConversation_ReturnsAllMessagesInOrder()
//     {
//         var (sender, receiver) = await SeedTwoUsersAsync();
//         var now = DateTime.UtcNow;
//
//         await SeedMessageAsync(sender.Id,   receiver.Id, "Hey!",        isRead: true,  createdAt: now.AddMinutes(-10));
//         await SeedMessageAsync(receiver.Id, sender.Id,   "Hi there!",   isRead: true,  createdAt: now.AddMinutes(-8));
//         await SeedMessageAsync(sender.Id,   receiver.Id, "Still there?",isRead: false, createdAt: now.AddMinutes(-2));
//
//         var result = (await _service.GetConversationAsync(sender.Id, receiver.Id)).ToList();
//
//         Assert.Equal(3, result.Count);
//         Assert.True(result[0].CreatedAt <= result[1].CreatedAt);
//         Assert.True(result[1].CreatedAt <= result[2].CreatedAt);
//     }
//
//     /// <summary>
//     /// IT-MSG-03: Receiver marks an unread message as read.
//     /// Expected: Operation completes successfully. 
//     /// </summary>
//     /// <returns>
//     /// Updated Message record with IsRead = true.
//     /// </returns>
//     [Fact]
//     public async Task IT_MSG_03_MarkAsRead_ByReceiver_UpdatesIsReadInDatabase()
//     {
//         var (sender, receiver) = await SeedTwoUsersAsync();
//         var message = await SeedMessageAsync(sender.Id, receiver.Id, isRead: false);
//
//         await _service.MarkAsReadAsync(message.Id, receiver.Id);
//
//         var updated = await _db.Messages.FirstOrDefaultAsync(m => m.Id == message.Id);
//         Assert.NotNull(updated);
//         Assert.True(updated!.IsRead);
//     }
//
//     /// <summary>
//     /// IT-MSG-03b: A user who is not the receiver attempts to mark a message as read.
//     /// Expected: UnauthorizedAccessException is thrown and the database record is unchanged.
//     /// </summary>
//     /// <returns>
//     /// UnauthorizedAccessException. Message.IsRead remains false.
//     /// </returns>
//     [Fact]
//     public async Task IT_MSG_03b_MarkAsRead_ByNonReceiver_ThrowsAndLeavesRecordUnchanged()
//     {
//         var (sender, receiver) = await SeedTwoUsersAsync();
//         var thirdParty = new ApplicationUser { Id = Guid.NewGuid(), UserName = "notme" };
//         _db.Users.Add(thirdParty);
//         await _db.SaveChangesAsync();
//
//         var message = await SeedMessageAsync(sender.Id, receiver.Id, isRead: false);
//
//         await Assert.ThrowsAsync<UnauthorizedAccessException>(
//             () => _service.MarkAsReadAsync(message.Id, thirdParty.Id));
//
//         var unchanged = await _db.Messages.FirstOrDefaultAsync(m => m.Id == message.Id);
//         Assert.NotNull(unchanged);
//         Assert.False(unchanged!.IsRead);
//     }
//
//     /// <summary>
//     /// IT-MSG-04: Mark conversation as read.
//     /// Expected: All messages in coversation are marked as read. 
//     /// </summary>
//     /// <returns>
//     /// All targeted Message records have IsRead = true in the database.
//     /// </returns>
//     [Fact]
//     public async Task IT_MSG_04_MarkConversationAsRead_UpdatesAllTargetedMessages()
//     {
//         var (sender, receiver) = await SeedTwoUsersAsync();
//         var now = DateTime.UtcNow;
//
//         // Two messages the receiver has NOT read yet
//         var msg1 = await SeedMessageAsync(sender.Id, receiver.Id, "First",  isRead: false, createdAt: now.AddMinutes(-5));
//         var msg2 = await SeedMessageAsync(sender.Id, receiver.Id, "Second", isRead: false, createdAt: now.AddMinutes(-3));
//
//         // One message the receiver sent — should remain unaffected
//         var msg3 = await SeedMessageAsync(receiver.Id, sender.Id, "Reply",  isRead: false, createdAt: now.AddMinutes(-1));
//
//         await _service.MarkConversationAsReadAsync(receiver.Id, sender.Id);
//
//         var updated1 = await _db.Messages.FirstOrDefaultAsync(m => m.Id == msg1.Id);
//         var updated2 = await _db.Messages.FirstOrDefaultAsync(m => m.Id == msg2.Id);
//         var untouched = await _db.Messages.FirstOrDefaultAsync(m => m.Id == msg3.Id);
//
//         Assert.True(updated1!.IsRead);
//         Assert.True(updated2!.IsRead);
//         Assert.False(untouched!.IsRead); // receiver's own outbound message unchanged
//     }
//
//     /// <summary>
//     /// IT-MSG-05: Get unread message count for a user.
//     /// Expected: Correct count of unread messages is received.
//     /// </summary>
//     /// <returns>
//     /// Integer count matching unread message records. Reduces to 0 after mark-as-read.
//     /// </returns>
//     [Fact]
//     public async Task IT_MSG_05_GetUnreadCount_ReflectsActualDatabaseState()
//     {
//         var (sender, receiver) = await SeedTwoUsersAsync();
//
//         await SeedMessageAsync(sender.Id, receiver.Id, "Msg 1", isRead: false);
//         await SeedMessageAsync(sender.Id, receiver.Id, "Msg 2", isRead: false);
//         await SeedMessageAsync(sender.Id, receiver.Id, "Msg 3", isRead: true);  // already read
//
//         var countBefore = await _service.GetUnreadCountAsync(receiver.Id);
//         Assert.Equal(2, countBefore);
//
//         await _service.MarkConversationAsReadAsync(receiver.Id, sender.Id);
//
//         var countAfter = await _service.GetUnreadCountAsync(receiver.Id);
//         Assert.Equal(0, countAfter);
//     }
// }