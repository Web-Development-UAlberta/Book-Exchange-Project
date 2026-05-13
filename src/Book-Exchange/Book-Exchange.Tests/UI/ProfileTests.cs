using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace UI;

public class ProfileTests : PageTest
{
    private const string BaseUrl = "http://localhost:5261";
    private const string TestEmail = "test@test.com";
    private const string TestPassword = "Test1234!";

    private const string OtherUserId = "aaaa0002-0000-0000-0000-000000000002";

    // Login helper
    private async Task LoginAsync(string email = TestEmail, string password = TestPassword)
    {
        await Page.GotoAsync($"{BaseUrl}/Identity/Account/Login");
        await Page.FillAsync("#Input_Email", email);
        await Page.FillAsync("#Input_Password", password);
        await Page.ClickAsync("#login-submit");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        if (Page.Url.Contains("/Account/Login"))
            throw new Exception($"Login failed for {email} – check the user exists in the database.");
    }

    /// <summary>
    /// UI-PROF-01: Own profile loads for an authenticated user.
    /// Expected: Profile page is visible and title contains the username.
    /// </summary>
    [Fact]
    public async Task UI_PROF_01_OwnProfile_LoadsSuccessfully()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/Profile");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page.Locator("#profile-page")).ToBeVisibleAsync();

        var title = await Page.TitleAsync();
        Assert.Contains("Profile", title);
    }

    /// <summary>
    /// UI-PROF-02: Unauthenticated user accessing /Profile is redirected to login.
    /// Expected: URL contains /Account/Login.
    /// </summary>
    [Fact]
    public async Task UI_PROF_02_OwnProfile_UnauthenticatedUser_RedirectsToLogin()
    {
        await Page.GotoAsync($"{BaseUrl}/Profile");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page).ToHaveURLAsync(new Regex(".*/Account/Login.*"));
    }

    /// <summary>
    /// UI-PROF-03: Profile header shows username, member-since year and avatar.
    /// Expected: Profile username, member since and avatar are visible and non-empty.
    /// </summary>
    [Fact]
    public async Task UI_PROF_03_ProfileHeader_ShowsUsernameAndMemberSince()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/Profile");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var username = Page.Locator("#profile-username");
        await Expect(username).ToBeVisibleAsync();
        Assert.False(string.IsNullOrWhiteSpace(await username.InnerTextAsync()));

        var memberSince = Page.Locator("#profile-member-since");
        await Expect(memberSince).ToBeVisibleAsync();
        Assert.Contains("MEMBER SINCE", await memberSince.InnerTextAsync());

        await Expect(Page.Locator("#profile-avatar")).ToBeVisibleAsync();
    }

    /// <summary>
    /// UI-PROF-04: "Edit Profile" button is visible on own profile.
    /// Expected: Profile edit button is visible when IsOwnProfile = true.
    /// </summary>
    [Fact]
    public async Task UI_PROF_04_OwnProfile_ShowsEditProfileBtn()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/Profile");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page.Locator("#profile-edit-btn")).ToBeVisibleAsync();
    }

    /// <summary>
    /// UI-PROF-05: "Edit Profile" button is NOT shown when viewing another user's profile.
    /// Expected: Profile edit button is absent when an authenticated user views someone else's profile.
    /// </summary>
    [Fact]
    public async Task UI_PROF_05_PublicProfile_DoesNotShowEditBtn()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/Profile/View/{OtherUserId}");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        if (await Page.Locator("#profile-page").CountAsync() == 0)
            return;

        Assert.Equal(0, await Page.Locator("#profile-edit-btn").CountAsync());
    }

    /// <summary>
    /// UI-PROF-06: Profile stats section shows Rating, Trades, and Listings cards.
    /// Expected: Profile stats section is visible with rating, trade count,
    ///           and listing count each containing a stat value.
    /// </summary>
    [Fact]
    public async Task UI_PROF_06_ProfileStats_ShowsAllThreeCards()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/Profile");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page.Locator("#profile-stats")).ToBeVisibleAsync();
        await Expect(Page.Locator("#profile-rating .stat-value")).ToBeVisibleAsync();
        await Expect(Page.Locator("#profile-trade-count .stat-value")).ToBeVisibleAsync();
        await Expect(Page.Locator("#profile-listing-count .stat-value")).ToBeVisibleAsync();
    }

    /// <summary>
    /// UI-PROF-07: Rating stat value is a decimal number in "0.0" format.
    /// Expected: Profile rating stat value text parses as a double.
    /// </summary>
    [Fact]
    public async Task UI_PROF_07_ProfileStats_RatingValue_IsNumeric()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/Profile");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var text = (await Page.Locator("#profile-rating .stat-value").InnerTextAsync()).Trim();
        Assert.True(double.TryParse(text, out _), $"Expected rating to be numeric, got '{text}'.");
    }

    /// <summary>
    /// UI-PROF-08: Trade count and listing count stat values are integers.
    /// Expected: Both trade count and listing count stat values parse as integers.
    /// </summary>
    [Fact]
    public async Task UI_PROF_08_ProfileStats_TradeAndListingCounts_AreIntegers()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/Profile");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var tradeText = (await Page.Locator("#profile-trade-count .stat-value").InnerTextAsync()).Trim();
        var listingText = (await Page.Locator("#profile-listing-count .stat-value").InnerTextAsync()).Trim();

        Assert.True(int.TryParse(tradeText, out _), $"Expected trade count to be an integer, got '{tradeText}'.");
        Assert.True(int.TryParse(listingText, out _), $"Expected listing count to be an integer, got '{listingText}'.");
    }

    /// <summary>
    /// UI-PROF-09: Reviews section is visible.
    /// Expected: Reviews section is visible with a heading.
    /// </summary>
    [Fact]
    public async Task UI_PROF_09_ReviewsPanel_IsVisible()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/Profile");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page.Locator("#profile-review-list")).ToBeVisibleAsync();
    }

    /// <summary>
    /// UI-PROF-10: Review cards show reviewer name and star rating when reviews exist.
    /// Expected: At least one profile reviews comment with reviewer name and star rating is visible.
    /// </summary>
    [Fact]
    public async Task UI_PROF_10_ReviewsPanel_ShowsReviewerNameAndStars_WhenReviewsExist()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/Profile");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var reviews = Page.Locator(".profile-reviews-comment");
        if (await reviews.CountAsync() == 0)
            return; // Skip if no reviews

        await Expect(reviews.First.Locator(".reviewer-name")).ToBeVisibleAsync();
        await Expect(reviews.First.Locator(".star-rating")).ToBeVisibleAsync();
    }

    /// <summary>
    /// UI-PROF-11: Reviews panel shows "No reviews yet." when there are none.
    /// Expected: The fallback text is visible when the reviews list is empty.
    /// </summary>
    [Fact]
    public async Task UI_PROF_11_ReviewsPanel_ShowsEmptyState_WhenNoReviews()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/Profile");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var reviews = Page.Locator(".profile-reviews-comment");
        if (await reviews.CountAsync() > 0)
            return;

        await Expect(Page.Locator("#profile-review-list .text-muted")).ToBeVisibleAsync();
    }

    /// <summary>
    /// UI-PROF-12: Public listings section is visible.
    /// Expected: Profile listings list is visible with a heading.
    /// </summary>
    [Fact]
    public async Task UI_PROF_12_ListingsPanel_IsVisible()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/Profile");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page.Locator("#profile-listings-list")).ToBeVisibleAsync();
    }

    /// <summary>
    /// UI-PROF-13: Listing rows show a book title link and a condition badge.
    /// Expected: At least one Profile listing row with a listing title anchor
    ///           and a condition type badge is visible.
    /// </summary>
    [Fact]
    public async Task UI_PROF_13_ListingsPanel_ShowsTitleAndConditionBadge_WhenListingsExist()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/Profile");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var rows = Page.Locator(".profile-listing-row");
        if (await rows.CountAsync() == 0)
            return; // Skip if no listings

        await Expect(rows.First.Locator(".listing-title a")).ToBeVisibleAsync();
        await Expect(rows.First.Locator(".profile-type-badge")).ToBeVisibleAsync();
    }

    /// <summary>
    /// UI-PROF-14: Clicking a listing title link navigates to the Listing details page.
    /// Expected: URL contains /Listing/Details after click.
    /// </summary>
    [Fact]
    public async Task UI_PROF_14_ListingsPanel_TitleLink_NavigatesToListingDetails()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/Profile");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var firstLink = Page.Locator(".profile-listing-row .listing-title a").First;
        if (await firstLink.CountAsync() == 0)
            return; // Skip if no listings

        await firstLink.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page).ToHaveURLAsync(new Regex(".*/Listing/Details/.*"));
    }

    /// <summary>
    /// UI-PROF-15: Listings panel shows "No listings yet." when there are none.
    /// Expected: The fallback text is visible when no .profile-listing-row elements exist.
    /// </summary>
    [Fact]
    public async Task UI_PROF_15_ListingsPanel_ShowsEmptyState_WhenNoListings()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/Profile");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var rows = Page.Locator(".profile-listing-row");
        if (await rows.CountAsync() > 0)
            return; // Skips if listings exist

        await Expect(Page.Locator("#profile-listings-list .text-muted")).ToBeVisibleAsync();
    }
}