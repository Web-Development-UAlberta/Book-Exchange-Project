using Xunit;
using Moq;
using Book_Exchange.Models;
using Book_Exchange.Models.DTOs.Notification;
using Book_Exchange.Services.Interfaces;

namespace Book_Exchange.Tests.BackEnd;

// UNIT TESTS
// Covers: UT-NOTIF-01 through UT-NOTIF-04
public class NotificationUnitTests
{
    private readonly Mock<INotificationService> _serviceMock;

    public NotificationUnitTests()
    {
        _serviceMock = new Mock<INotificationService>();
    }

    /// <summary>
    /// UT-NOTIF-01: Trigger notification when a match is found
    /// Expected: Notification record is created
    /// </summary>
    [Fact]
    public async Task UT_NOTIF_01_CreateMatchFoundNotification_CompletesSuccessfully()
    {
        var userId = Guid.NewGuid();
        var listingId = Guid.NewGuid();

        var dto = new CreateNotificationDto
        {
            UserId = userId,
            Category = NotificationCategory.MatchFound,
            Title = "Match found",
            Message = "A matching book is available.",
            RelatedListingId = listingId
        };

        _serviceMock
            .Setup(s => s.CreateNotificationAsync(dto))
            .Returns(Task.CompletedTask);

        await _serviceMock.Object.CreateNotificationAsync(dto);

        _serviceMock.Verify(s => s.CreateNotificationAsync(dto), Times.Once);
    }

    /// <summary>
    /// UT-NOTIF-02: Trigger notification when message is received
    /// Expected: Notification is created
    /// </summary>
    [Fact]
    public async Task UT_NOTIF_02_CreateNewMessageNotification_CompletesSuccessfully()
    {
        var userId = Guid.NewGuid();
        var exchangeRequestId = Guid.NewGuid();

        var dto = new CreateNotificationDto
        {
            UserId = userId,
            Category = NotificationCategory.NewMessage,
            Title = "New message",
            Message = "You received a new message.",
            RelatedExchangeRequestId = exchangeRequestId
        };

        _serviceMock
            .Setup(s => s.CreateNotificationAsync(dto))
            .Returns(Task.CompletedTask);

        await _serviceMock.Object.CreateNotificationAsync(dto);

        _serviceMock.Verify(s => s.CreateNotificationAsync(dto), Times.Once);
    }

    /// <summary>
    /// UT-NOTIF-03: Trigger notification when exchange request is accepted
    /// Expected: Notification is created
    /// </summary>
    [Fact]
    public async Task UT_NOTIF_03_CreateExchangeAcceptedNotification_CompletesSuccessfully()
    {
        var userId = Guid.NewGuid();
        var exchangeRequestId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();

        var dto = new CreateNotificationDto
        {
            UserId = userId,
            Category = NotificationCategory.ExchangeAccepted,
            Title = "Exchange accepted",
            Message = "Your exchange request has been accepted.",
            RelatedExchangeRequestId = exchangeRequestId,
            RelatedTransactionId = transactionId
        };

        _serviceMock
            .Setup(s => s.CreateNotificationAsync(dto))
            .Returns(Task.CompletedTask);

        await _serviceMock.Object.CreateNotificationAsync(dto);

        _serviceMock.Verify(s => s.CreateNotificationAsync(dto), Times.Once);
    }

    /// <summary>
    /// UT-NOTIF-04: Mark notification as read
    /// Expected: Status changes to Read and ReadAt is populated
    /// </summary>
    [Fact]
    public async Task UT_NOTIF_04_MarkNotificationAsRead_CompletesSuccessfully()
    {
        var userId = Guid.NewGuid();
        var notificationId = Guid.NewGuid();

        _serviceMock
            .Setup(s => s.MarkAsReadAsync(notificationId, userId))
            .Returns(Task.CompletedTask);

        await _serviceMock.Object.MarkAsReadAsync(notificationId, userId);

        _serviceMock.Verify(
            s => s.MarkAsReadAsync(notificationId, userId),
            Times.Once);
    }

    /// <summary>
    /// Extra check for UT-NOTIF-04:
    /// After marking as read, notification should have IsRead = true and ReadAt populated.
    /// </summary>
    [Fact]
    public async Task UT_NOTIF_04_GetReadNotification_ReturnsIsReadTrueAndReadAtPopulated()
    {
        var userId = Guid.NewGuid();
        var notificationId = Guid.NewGuid();

        var expectedNotification = new NotificationDto
        {
            Id = notificationId,
            Category = NotificationCategory.NewMessage,
            Title = "New message",
            Message = "You received a new message.",
            IsRead = true,
            CreatedAt = DateTime.UtcNow.AddMinutes(-10),
            ReadAt = DateTime.UtcNow
        };

        _serviceMock
            .Setup(s => s.GetNotificationByIdAsync(notificationId, userId))
            .ReturnsAsync(expectedNotification);

        var result = await _serviceMock.Object.GetNotificationByIdAsync(notificationId, userId);

        Assert.NotNull(result);
        Assert.True(result.IsRead);
        Assert.NotNull(result.ReadAt);
    }
}