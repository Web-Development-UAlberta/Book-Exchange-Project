using Xunit;
using Microsoft.EntityFrameworkCore;
using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Notification;
using Book_Exchange.Services;
using Book_Exchange.Services.Interfaces;
using Book_Exchange.Data;

namespace Book_Exchange.Tests.Integration;

// Notification Integration Tests
// Covers: IT-NOTIF-01 through IT-NOTIF-04

// TODO: Uncomment when NotificationService is implemented
// public class NotificationServiceIntegrationTests : IDisposable
// {
//     private readonly ApplicationDbContext _db;
//     private readonly INotificationService _service;
//
//     public NotificationServiceIntegrationTests()
//     {
//         var options = new DbContextOptionsBuilder<ApplicationDbContext>()
//             .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
//             .Options;
//
//         _db = new ApplicationDbContext(options);
//
//         _service = new NotificationService(_db);
//         // _service = null!;
//     }
//
//     public void Dispose()
//     {
//         _db.Dispose();
//     }
//
//     /// <summary>
//     /// IT-NOTIF-01: Notification is created for a user
//     /// Expected: Notification is saved in the database as unread
//     /// </summary>
//     [Fact]
//     public async Task IT_NOTIF_01_CreateNotification_NotificationIsSavedAsUnread()
//     {
//         var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "user1" };
//         _db.Users.Add(user);
//         await _db.SaveChangesAsync();
//
//         var dto = new CreateNotificationDto
//         {
//             UserId = user.Id,
//             Category = NotificationCategory.MatchFound,
//             Title = "Match found",
//             Message = "A matching book is available."
//         };
//
//         await _service.CreateNotificationAsync(dto);
//
//         var saved = await _db.Notifications.FirstOrDefaultAsync(n => n.UserId == user.Id);
//
//         Assert.NotNull(saved);
//         Assert.Equal(NotificationCategory.MatchFound, saved!.Category);
//         Assert.Equal("Match found", saved.Title);
//         Assert.Equal("A matching book is available.", saved.Message);
//         Assert.False(saved.IsRead);
//         Assert.Null(saved.ReadAt);
//     }
//
//     /// <summary>
//     /// IT-NOTIF-02: User receives notification list
//     /// Expected: Notifications for the user are returned
//     /// </summary>
//     [Fact]
//     public async Task IT_NOTIF_02_GetNotificationsForUser_ReturnsUserNotifications()
//     {
//         var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "user2" };
//         var otherUser = new ApplicationUser { Id = Guid.NewGuid(), UserName = "other" };
//
//         _db.Users.AddRange(user, otherUser);
//         _db.Notifications.AddRange(
//             new Notification
//             {
//                 Id = Guid.NewGuid(),
//                 UserId = user.Id,
//                 Category = NotificationCategory.NewMessage,
//                 Title = "New message",
//                 Message = "You received a new message.",
//                 IsRead = false,
//                 CreatedAt = DateTime.UtcNow
//             },
//             new Notification
//             {
//                 Id = Guid.NewGuid(),
//                 UserId = otherUser.Id,
//                 Category = NotificationCategory.ExchangeAccepted,
//                 Title = "Other notification",
//                 Message = "This belongs to another user.",
//                 IsRead = false,
//                 CreatedAt = DateTime.UtcNow
//             }
//         );
//         await _db.SaveChangesAsync();
//
//         var result = await _service.GetNotificationsForUserAsync(user.Id);
//
//         Assert.NotNull(result);
//         Assert.Single(result);
//         Assert.Equal("New message", result.First().Title);
//     }
//
//     /// <summary>
//     /// IT-NOTIF-03: User gets unread notifications
//     /// Expected: Only unread notifications are returned
//     /// </summary>
//     [Fact]
//     public async Task IT_NOTIF_03_GetUnreadNotifications_ReturnsOnlyUnreadNotifications()
//     {
//         var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "user3" };
//
//         _db.Users.Add(user);
//         _db.Notifications.AddRange(
//             new Notification
//             {
//                 Id = Guid.NewGuid(),
//                 UserId = user.Id,
//                 Category = NotificationCategory.NewMessage,
//                 Title = "Unread notification",
//                 Message = "This is unread.",
//                 IsRead = false,
//                 CreatedAt = DateTime.UtcNow
//             },
//             new Notification
//             {
//                 Id = Guid.NewGuid(),
//                 UserId = user.Id,
//                 Category = NotificationCategory.ExchangeAccepted,
//                 Title = "Read notification",
//                 Message = "This is already read.",
//                 IsRead = true,
//                 ReadAt = DateTime.UtcNow,
//                 CreatedAt = DateTime.UtcNow.AddMinutes(-10)
//             }
//         );
//         await _db.SaveChangesAsync();
//
//         var result = await _service.GetUnreadNotificationsAsync(user.Id);
//
//         Assert.NotNull(result);
//         Assert.Single(result);
//         Assert.Equal("Unread notification", result.First().Title);
//
//         var unreadCount = await _service.GetUnreadCountAsync(user.Id);
//         Assert.Equal(1, unreadCount);
//     }
//
//     /// <summary>
//     /// IT-NOTIF-04: Mark notification as read
//     /// Expected: IsRead becomes true and ReadAt is populated
//     /// </summary>
//     [Fact]
//     public async Task IT_NOTIF_04_MarkNotificationAsRead_StatusChangesToRead()
//     {
//         var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "user4" };
//
//         var notification = new Notification
//         {
//             Id = Guid.NewGuid(),
//             UserId = user.Id,
//             Category = NotificationCategory.TransactionUpdate,
//             Title = "Transaction update",
//             Message = "Your transaction status changed.",
//             IsRead = false,
//             CreatedAt = DateTime.UtcNow
//         };
//
//         _db.Users.Add(user);
//         _db.Notifications.Add(notification);
//         await _db.SaveChangesAsync();
//
//         await _service.MarkAsReadAsync(notification.Id, user.Id);
//
//         var saved = await _db.Notifications.FindAsync(notification.Id);
//
//         Assert.NotNull(saved);
//         Assert.True(saved!.IsRead);
//         Assert.NotNull(saved.ReadAt);
//     }
//
//     /// <summary>
//     /// IT-NOTIF-05: User tries to mark another user's notification as read
//     /// Expected: Operation is blocked
//     /// </summary>
//     [Fact]
//     public async Task IT_NOTIF_05_MarkAnotherUsersNotificationAsRead_IsBlocked()
//     {
//         var owner = new ApplicationUser { Id = Guid.NewGuid(), UserName = "owner" };
//         var other = new ApplicationUser { Id = Guid.NewGuid(), UserName = "other" };
//
//         var notification = new Notification
//         {
//             Id = Guid.NewGuid(),
//             UserId = owner.Id,
//             Category = NotificationCategory.NewMessage,
//             Title = "Private notification",
//             Message = "Only the owner can read this.",
//             IsRead = false,
//             CreatedAt = DateTime.UtcNow
//         };
//
//         _db.Users.AddRange(owner, other);
//         _db.Notifications.Add(notification);
//         await _db.SaveChangesAsync();
//
//         await Assert.ThrowsAsync<KeyNotFoundException>(
//             () => _service.MarkAsReadAsync(notification.Id, other.Id));
//
//         var saved = await _db.Notifications.FindAsync(notification.Id);
//
//         Assert.NotNull(saved);
//         Assert.False(saved!.IsRead);
//         Assert.Null(saved.ReadAt);
//     }
// }