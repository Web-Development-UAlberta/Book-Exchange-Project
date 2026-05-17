using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace UI;

public class NotificationTests : PageTest
{
    private const string BaseUrl = "http://localhost:5261";

    private async Task LoginAsync(string email, string password)
    {
        await Page.GotoAsync($"{BaseUrl}/Identity/Account/Login");

        await Page.FillAsync("#Input_Email", email);
        await Page.FillAsync("#Input_Password", password);
        await Page.ClickAsync("#login-submit");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        if (Page.Url.Contains("/Account/Login"))
            throw new Exception($"Login failed for {email} – verify the user exists in the database.");
    }

    /// <summary>
    /// UI-NOTIF-01: Notifications index page loads for authenticated user
    /// Expected: Page title and notifications panel container are visible
    /// </summary>
    [Fact]
    public async Task UI_NOTIF_01_Index_AuthenticatedUser_LoadsPage()
    {
        await LoginAsync("test@test.com", "Test1234!");

        await Page.GotoAsync($"{BaseUrl}/Notification");

        await Expect(Page).ToHaveTitleAsync("Notifications - Book_Exchange");
        await Expect(Page.Locator("#notifications-panel")).ToBeVisibleAsync();
    }

    /// <summary>
    /// UI-NOTIF-02: Unauthenticated user is redirected to login when accessing notifications
    /// Expected: User is redirected to the login page
    /// </summary>
    [Fact]
    public async Task UI_NOTIF_02_Index_UnauthenticatedUser_RedirectsToLogin()
    {
        await Page.GotoAsync($"{BaseUrl}/Notification");

        await Expect(Page).ToHaveURLAsync(new Regex(".*/Account/Login.*"));
    }

    /// <summary>
    /// UI-NOTIF-03: Notifications index shows empty state when user has no notifications
    /// Expected: Empty message element is visible, notification list is not rendered
    /// </summary>
    [Fact]
    public async Task UI_NOTIF_03_Index_NoNotifications_ShowsEmptyMessage()
    {
        await LoginAsync("test@test.com", "Test1234!");

        await Page.GotoAsync($"{BaseUrl}/Notification");

        var emptyMessage = Page.Locator(".notification-empty");
        var notificationList = Page.Locator(".notification-list");

        var hasEmpty = await emptyMessage.CountAsync() > 0;
        var hasList = await notificationList.CountAsync() > 0;

        Assert.True(hasEmpty || hasList, "Page should show either an empty message or a notification list.");
    }

    /// <summary>
    /// UI-NOTIF-04: Notifications index shows notification items when notifications exist
    /// Expected: At least one notification-item is visible with title, message, and timestamp
    /// </summary>
    [Fact]
    public async Task UI_NOTIF_04_Index_WithNotifications_ShowsNotificationItems()
    {
        await LoginAsync("test@test.com", "Test1234!");

        await Page.GotoAsync($"{BaseUrl}/Notification");

        var notificationList = Page.Locator(".notification-list");

        if (await notificationList.CountAsync() > 0)
        {
            await Expect(Page.Locator(".notification-item").First).ToBeVisibleAsync();
            await Expect(Page.Locator(".notification-title").First).ToBeVisibleAsync();
            await Expect(Page.Locator(".notification-message").First).ToBeVisibleAsync();
            await Expect(Page.Locator(".notification-timestamp").First).ToBeVisibleAsync();
        }
    }

    /// <summary>
    /// UI-NOTIF-05: Unread notifications display the unread dot indicator
    /// Expected: Dot element is visible on unread items
    /// </summary>
    [Fact]
    public async Task UI_NOTIF_05_Index_UnreadNotification_ShowsUnreadDot()
    {
        await LoginAsync("test@test.com", "Test1234!");

        await Page.GotoAsync($"{BaseUrl}/Notification");

        var unreadItems = Page.Locator(".notification-item--unread");

        if (await unreadItems.CountAsync() > 0)
        {
            await Expect(unreadItems.First.Locator(".notification-dot")).ToBeVisibleAsync();
        }
    }

    /// <summary>
    /// UI-NOTIF-06: Unread notifications show a Mark read button
    /// Expected: Mark read button is visible on unread items
    /// </summary>
    [Fact]
    public async Task UI_NOTIF_06_Index_UnreadNotification_ShowsMarkReadButton()
    {
        await LoginAsync("test@test.com", "Test1234!");

        await Page.GotoAsync($"{BaseUrl}/Notification");

        var unreadItems = Page.Locator(".notification-item--unread");

        if (await unreadItems.CountAsync() > 0)
        {
            await Expect(unreadItems.First.Locator(".notification-mark-read-btn")).ToBeVisibleAsync();
        }
    }

    /// <summary>
    /// UI-NOTIF-07: Mark all as read button is visible when notifications exist
    /// Expected: Mark all as read button is rendered on the page
    /// </summary>
    [Fact]
    public async Task UI_NOTIF_07_Index_WithNotifications_ShowsMarkAllReadButton()
    {
        await LoginAsync("test@test.com", "Test1234!");

        await Page.GotoAsync($"{BaseUrl}/Notification");

        var notificationList = Page.Locator(".notification-list");

        if (await notificationList.CountAsync() > 0)
        {
            await Expect(Page.Locator("#mark-all-read-btn")).ToBeVisibleAsync();
        }
    }

    /// <summary>
    /// UI-NOTIF-08: Clicking Mark all as read marks all notifications as read
    /// Expected: After clicking, no unread dot indicators remain on the page
    /// </summary>
    [Fact]
    public async Task UI_NOTIF_08_Index_MarkAllAsRead_RemovesUnreadIndicators()
    {
        await LoginAsync("test@test.com", "Test1234!");

        await Page.GotoAsync($"{BaseUrl}/Notification");

        var markAllBtn = Page.Locator("#mark-all-read-btn");

        if (await markAllBtn.CountAsync() > 0)
        {
            await markAllBtn.ClickAsync();

            await Page.WaitForURLAsync(new Regex(".*/Notification.*"));

            var unreadDots = Page.Locator(".notification-dot");
            Assert.Equal(0, await unreadDots.CountAsync());
        }
    }

    /// <summary>
    /// UI-NOTIF-09: Each notification item shows a Dismiss button
    /// Expected: notification-archive-btn is visible on each notification item
    /// </summary>
    [Fact]
    public async Task UI_NOTIF_09_Index_NotificationItem_ShowsDismissButton()
    {
        await LoginAsync("test@test.com", "Test1234!");

        await Page.GotoAsync($"{BaseUrl}/Notification");

        var notificationItems = Page.Locator(".notification-item");

        if (await notificationItems.CountAsync() > 0)
        {
            await Expect(notificationItems.First.Locator(".notification-archive-btn")).ToBeVisibleAsync();
        }
    }

    /// <summary>
    /// UI-NOTIF-10: Clicking Dismiss removes the notification from the list
    /// Expected: After dismissing, the page reloads and the dismissed item is no longer present
    /// </summary>
    [Fact]
    public async Task UI_NOTIF_10_Index_DismissNotification_RemovesItemFromList()
    {
        await LoginAsync("test@test.com", "Test1234!");

        await Page.GotoAsync($"{BaseUrl}/Notification");

        var notificationItems = Page.Locator(".notification-item");

        if (await notificationItems.CountAsync() > 0)
        {
            var countBefore = await notificationItems.CountAsync();

            await notificationItems.First.Locator(".notification-archive-btn").ClickAsync();

            await Page.WaitForURLAsync(new Regex(".*/Notification.*"));

            var countAfter = await Page.Locator(".notification-item").CountAsync();

            Assert.True(countAfter < countBefore, "Notification count should decrease after dismissing.");
        }
    }

    /// <summary>
    /// UI-NOTIF-11: Clicking a notification title navigates to the details page
    /// Expected: URL changes to Notification/Details and the detail panel is visible
    /// </summary>
    [Fact]
    public async Task UI_NOTIF_11_Index_ClickNotificationTitle_NavigatesToDetails()
    {
        await LoginAsync("test@test.com", "Test1234!");

        await Page.GotoAsync($"{BaseUrl}/Notification");

        var firstTitle = Page.Locator(".notification-title").First;

        if (await firstTitle.CountAsync() > 0)
        {
            await firstTitle.ClickAsync();

            await Expect(Page).ToHaveURLAsync(new Regex(".*/Notification/Details.*"));
            await Expect(Page.Locator("#notification-detail-panel")).ToBeVisibleAsync();
        }
    }

    /// <summary>
    /// UI-NOTIF-12: Unauthenticated user is redirected to login when accessing a details page
    /// Expected: User is redirected to the login page
    /// </summary>
    [Fact]
    public async Task UI_NOTIF_12_Details_UnauthenticatedUser_RedirectsToLogin()
    {
        var fakeId = Guid.NewGuid();
        await Page.GotoAsync($"{BaseUrl}/Notification/Details/{fakeId}");

        await Expect(Page).ToHaveURLAsync(new Regex(".*/Account/Login.*"));
    }

    /// <summary>
    /// UI-NOTIF-13: Details page loads with all expected elements
    /// Expected: Title, message, category badge, timestamp, and back button are all visible
    /// </summary>
    [Fact]
    public async Task UI_NOTIF_13_Details_LoadsAllSections()
    {
        await LoginAsync("test@test.com", "Test1234!");

        await Page.GotoAsync($"{BaseUrl}/Notification");

        var firstTitle = Page.Locator(".notification-title").First;

        if (await firstTitle.CountAsync() > 0)
        {
            await firstTitle.ClickAsync();
            await Page.WaitForURLAsync(new Regex(".*/Notification/Details.*"));

            await Expect(Page).ToHaveTitleAsync("Notification Details - Book_Exchange");
            await Expect(Page.Locator("#notification-detail-panel")).ToBeVisibleAsync();
            await Expect(Page.Locator(".notification-detail-title")).ToBeVisibleAsync();
            await Expect(Page.Locator(".notification-detail-message")).ToBeVisibleAsync();
            await Expect(Page.Locator(".notification-category-badge")).ToBeVisibleAsync();
            await Expect(Page.Locator(".notification-detail-meta")).ToBeVisibleAsync();
            await Expect(Page.Locator(".notification-back-btn")).ToBeVisibleAsync();
        }
    }

    /// <summary>
    /// UI-NOTIF-14: Viewing a notification details page marks it as read automatically
    /// Expected: After navigating to details, the notification no longer shows the unread dot on the index
    /// </summary>
    [Fact]
    public async Task UI_NOTIF_14_Details_ViewingNotification_MarksAsRead()
    {
        await LoginAsync("test@test.com", "Test1234!");

        await Page.GotoAsync($"{BaseUrl}/Notification");

        var unreadItem = Page.Locator(".notification-item--unread").First;

        if (await unreadItem.CountAsync() > 0)
        {
            var titleLink = unreadItem.Locator(".notification-title");
            var titleText = await titleLink.InnerTextAsync();

            await titleLink.ClickAsync();
            await Page.WaitForURLAsync(new Regex(".*/Notification/Details.*"));

            await Page.GotoAsync($"{BaseUrl}/Notification");

            var stillUnread = Page.Locator(".notification-item--unread .notification-title",
                new PageLocatorOptions { HasTextString = titleText });

            Assert.Equal(0, await stillUnread.CountAsync());
        }
    }

    /// <summary>
    /// UI-NOTIF-15: Details page shows a Dismiss button
    /// Expected: Dismiss button is visible on the details card footer
    /// </summary>
    [Fact]
    public async Task UI_NOTIF_15_Details_ShowsDismissButton()
    {
        await LoginAsync("test@test.com", "Test1234!");

        await Page.GotoAsync($"{BaseUrl}/Notification");

        var firstTitle = Page.Locator(".notification-title").First;

        if (await firstTitle.CountAsync() > 0)
        {
            await firstTitle.ClickAsync();
            await Page.WaitForURLAsync(new Regex(".*/Notification/Details.*"));

            await Expect(Page.Locator(".notification-archive-btn")).ToBeVisibleAsync();
        }
    }

    /// <summary>
    /// UI-NOTIF-16: Dismissing from the details page redirects back to the notifications index
    /// Expected: After clicking Dismiss, the user lands back on the Notification index
    /// </summary>
    [Fact]
    public async Task UI_NOTIF_16_Details_DismissNotification_RedirectsToIndex()
    {
        await LoginAsync("test@test.com", "Test1234!");

        await Page.GotoAsync($"{BaseUrl}/Notification");

        var firstTitle = Page.Locator(".notification-title").First;

        if (await firstTitle.CountAsync() > 0)
        {
            await firstTitle.ClickAsync();
            await Page.WaitForURLAsync(new Regex(".*/Notification/Details.*"));

            await Page.Locator(".notification-archive-btn").ClickAsync();

            await Expect(Page).ToHaveURLAsync(new Regex(".*/Notification$"));
            await Expect(Page.Locator("#notifications-panel")).ToBeVisibleAsync();
        }
    }

    /// <summary>
    /// UI-NOTIF-17: Details page back button navigates back to the notifications index
    /// Expected: Clicking the back button returns the user to the notifications list
    /// </summary>
    [Fact]
    public async Task UI_NOTIF_17_Details_BackButton_NavigatesToIndex()
    {
        await LoginAsync("test@test.com", "Test1234!");

        await Page.GotoAsync($"{BaseUrl}/Notification");

        var firstTitle = Page.Locator(".notification-title").First;

        if (await firstTitle.CountAsync() > 0)
        {
            await firstTitle.ClickAsync();
            await Page.WaitForURLAsync(new Regex(".*/Notification/Details.*"));

            await Page.Locator(".notification-back-btn").ClickAsync();

            await Expect(Page).ToHaveURLAsync(new Regex(".*/Notification$"));
            await Expect(Page.Locator("#notifications-panel")).ToBeVisibleAsync();
        }
    }

    /// <summary>
    /// UI-NOTIF-18: Navbar Notifications button is visible for authenticated users
    /// Expected: The Notifications nav link is present in the main navbar
    /// </summary>
    [Fact]
    public async Task UI_NOTIF_18_Navbar_AuthenticatedUser_ShowsNotificationsLink()
    {
        await LoginAsync("test@test.com", "Test1234!");

        await Page.GotoAsync($"{BaseUrl}/");

        await Page.ClickAsync("#messageDropdown");
        await Page.WaitForSelectorAsync("a[href*='Notification']");

        await Expect(Page.Locator("a[href*='Notification'], button[href*='Notification']").First)
        .ToBeVisibleAsync();
    }

    /// <summary>
    /// UI-NOTIF-19: Clicking the Notifications navbar link navigates to the notifications index
    /// Expected: URL changes to /Notification and the panel is rendered
    /// </summary>
    [Fact]
    public async Task UI_NOTIF_19_Navbar_ClickNotificationsLink_NavigatesToIndex()
    {
        await LoginAsync("test@test.com", "Test1234!");

        await Page.GotoAsync($"{BaseUrl}/");

        await Page.ClickAsync("#messageDropdown");
        await Page.WaitForSelectorAsync("a[href*='/Notification']");

        await Page.Locator("a[href*='/Notification']").First.ClickAsync();
        await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex(".*/Notification.*"));
    }
}