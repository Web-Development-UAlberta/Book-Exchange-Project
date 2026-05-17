using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace UI;

public class DashboardTests : PageTest
{
    private const string BaseUrl = "http://localhost:5261";
    private const string TestEmail = "test@test.com";
    private const string TestPassword = "Test1234!";

    // Login helper

    private async Task LoginAsync(string email = TestEmail, string password = TestPassword)
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
    /// UI-DASH-01: Dashboard loads for an authenticated user.
    /// Expected: Title is "Dashboard" and Dashboard page is visible.
    /// </summary>
    [Fact]
    public async Task UI_DASH_01_Dashboard_LoadsSuccessfully()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/Home/Dashboard");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page).ToHaveTitleAsync("Dashboard - Book_Exchange");
        await Expect(Page.Locator("#dashboard-page")).ToBeVisibleAsync();
    }

    /// <summary>
    /// UI-DASH-02: Unauthenticated user is redirected to login.
    /// Expected: URL contains /Account/Login.
    /// </summary>
    [Fact]
    public async Task UI_DASH_02_Dashboard_UnauthenticatedUser_RedirectsToLogin()
    {
        await Page.GotoAsync($"{BaseUrl}/Home/Dashboard");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page).ToHaveURLAsync(new Regex(".*/Account/Login.*"));
    }

    /// <summary>
    /// UI-DASH-03: Sidebar is visible with all navigation links.
    /// Expected: #dashboard-sidebar is visible and contains all expected nav links.
    /// </summary>
    [Fact]
    public async Task UI_DASH_03_Sidebar_ShowsAllNavLinks()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/Home/Dashboard");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var sidebar = Page.Locator("#dashboard-sidebar");
        await Expect(sidebar).ToBeVisibleAsync();

        var links = sidebar.Locator(".sidebar-nav-link");
        var count = await links.CountAsync();
        Assert.True(count >= 9, $"Expected at least 9 sidebar nav links, found {count}.");
    }

    /// <summary>
    /// UI-DASH-04: Sidebar displays the logged in users usernaeme initial in avatar. 
    /// Expected: Avatar is visible and not empty. 
    /// </summary>
    [Fact]
    public async Task UI_DASH_04_Sidebar_ShowsUserAvatar()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/Home/Dashboard");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page.Locator(".bx-avatar")).ToBeVisibleAsync();

        var username = Page.Locator(".bx-sidebar-username");
        await Expect(username).ToBeVisibleAsync();
        var text = await username.InnerTextAsync();
        Assert.False(string.IsNullOrWhiteSpace(text), "Expected sidebar username to be non-empty.");
    }

    /// <summary>
    /// UI-DASH-05: Sidebar reputation line shows number of trades. 
    /// Expected: Reputation line is visible and displays the correct number of trades.
    /// </summary>
    [Fact]
    public async Task UI_DASH_05_Sidebar_ShowsTradeCount()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/Home/Dashboard");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var rep = Page.Locator(".bx-sidebar-rep");
        await Expect(rep).ToBeVisibleAsync();

        var text = await rep.InnerTextAsync();
        Assert.Contains("trades", text);
    }

    /// <summary>
    /// UI-DASH-06: Stat cards section is visible with all four cards.
    /// Expected: Dashboard stats are visible and contains four .stat-card elements.
    /// </summary>
    [Fact]
    public async Task UI_DASH_06_StatCards_AllFourAreVisible()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/Home/Dashboard");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var stats = Page.Locator("#dashboard-stats");
        await Expect(stats).ToBeVisibleAsync();

        var cards = stats.Locator(".stat-card");
        Assert.Equal(4, await cards.CountAsync());
    }

    /// <summary>
    /// UI-DASH-07: Stat cards each display a numeric value.
    /// Expected: Every Stat card value contains text that parses as an integer.
    /// </summary>
    [Fact]
    public async Task UI_DASH_07_StatCards_ShowNumericValues()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/Home/Dashboard");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var values = Page.Locator(".stat-card-value");
        var count = await values.CountAsync();
        Assert.Equal(4, count);

        for (var i = 0; i < count; i++)
        {
            var text = (await values.Nth(i).InnerTextAsync()).Trim();
            Assert.True(int.TryParse(text, out _), $"Expected stat card {i} to contain an integer, got '{text}'.");
        }
    }

    /// <summary>
    /// UI-DASH-08: Active transactions panel is visible.
    /// Expected: Active transactions is visible with a card header and footer.
    /// </summary>
    [Fact]
    public async Task UI_DASH_08_ActiveTransactionsPanel_IsVisible()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/Home/Dashboard");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page.Locator("#active-transactions")).ToBeVisibleAsync();
        await Expect(Page.Locator("#active-transactions .card-header")).ToBeVisibleAsync();
        await Expect(Page.Locator("#active-transactions .card-footer")).ToBeVisibleAsync();
    }

    /// <summary>
    /// UI-DASH-09: Active transactions panel "View all" link points to Transaction index.
    /// Expected: The footer link href contains /Transaction.
    /// </summary>
    [Fact]
    public async Task UI_DASH_09_ActiveTransactionsPanel_ViewAllLink_PointsToTransactions()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/Home/Dashboard");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var link = Page.Locator("#active-transactions .card-footer a");
        var href = await link.GetAttributeAsync("href");
        Assert.Contains("/Transaction", href);
    }

    /// <summary>
    /// UI-DASH-10: Wishlist panel is visible.
    /// Expected: Wishlist matches panel is visible.
    /// </summary>
    [Fact]
    public async Task UI_DASH_10_WishlistPanel_IsVisible()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/Home/Dashboard");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page.Locator("#wishlist-matches-panel")).ToBeVisibleAsync();
    }

    /// <summary>
    /// UI-DASH-11: Wishlist panel "View all" link points to the Wishlist index.
    /// Expected: The footer link href contains /Wishlist.
    /// </summary>
    [Fact]
    public async Task UI_DASH_11_WishlistPanel_ViewAllLink_PointsToWishlist()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/Home/Dashboard");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var link = Page.Locator("#wishlist-matches-panel .card-footer a");
        var href = await link.GetAttributeAsync("href");
        Assert.Contains("/Wishlist", href);
    }

    /// <summary>
    /// UI-DASH-12: My active listings panel is visible.
    /// Expected: My active listings panel is visible with a header and footer.
    /// </summary>
    [Fact]
    public async Task UI_DASH_12_MyListingsPanel_IsVisible()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/Home/Dashboard");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page.Locator("#my-listings-panel")).ToBeVisibleAsync();
        await Expect(Page.Locator("#my-listings-panel .card-header")).ToBeVisibleAsync();
        await Expect(Page.Locator("#my-listings-panel .card-footer")).ToBeVisibleAsync();
    }

    /// <summary>
    /// UI-DASH-13: "Add Listing" button is visible and links to the Listing create page.
    /// Expected: Dashboard create listing button is visible and href contains /Listing/Create.
    /// </summary>
    [Fact]
    public async Task UI_DASH_13_MyListingsPanel_AddListingBtn_IsVisible()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/Home/Dashboard");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var btn = Page.Locator("#dashboard-create-listing-btn");
        await Expect(btn).ToBeVisibleAsync();

        var href = await btn.GetAttributeAsync("href");
        Assert.Contains("/Listing/Create", href);
    }

    /// <summary>
    /// UI-DASH-14: Notifications panel is visible.
    /// Expected: Notifications panel is visible.
    /// </summary>
    [Fact]
    public async Task UI_DASH_14_NotificationsPanel_IsVisible()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/Home/Dashboard");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page.Locator("#notifications-panel")).ToBeVisibleAsync();
    }

    /// <summary>
    /// UI-DASH-15: Notifications panel "View all" link points to the Notification index.
    /// Expected: The footer link href contains /Notification.
    /// </summary>
    [Fact]
    public async Task UI_DASH_15_NotificationsPanel_ViewAllLink_PointsToNotifications()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/Home/Dashboard");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var link = Page.Locator("#notifications-panel .card-footer a");
        var href = await link.GetAttributeAsync("href");
        Assert.Contains("/Notification", href);
    }

}