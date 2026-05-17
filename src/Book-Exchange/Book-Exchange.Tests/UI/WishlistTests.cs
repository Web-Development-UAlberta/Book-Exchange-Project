using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace UI;

public class WishlistTests : PageTest
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
    /// UI-WISH-01: Wishlist page loads
    /// Expected: Wishlist page loads, if items exist they are shown, otherwise no items message is shown
    /// </summary>
    [Fact]
    public async Task UI_WISH_01_Index_Loads()
    {
        await LoginAsync("test@test.com", "Test1234!");

        await Page.GotoAsync($"{BaseUrl}/Wishlist");

        await Expect(Page).ToHaveTitleAsync("My Wishlist - Book_Exchange");
        await Expect(Page.Locator("#wishlist-add-btn")).ToBeVisibleAsync();

        var hasItems = await Page.Locator("#wishlist-items-list .wishlist-item").CountAsync() > 0;

        if (hasItems)
            await Expect(Page.Locator("#wishlist-items-list")).ToBeVisibleAsync();
        else
            await Expect(Page.Locator("#no-wishlist-message")).ToBeVisibleAsync();
    }

    /// <summary>
    /// UI-WISH-02: Unauthenticated user is redirected to login
    /// Expected: User cannot access wishlist without login
    /// </summary>
    [Fact]
    public async Task UI_WISH_02_UnauthenticatedUser_RedirectsToLogin()
    {
        await Page.GotoAsync($"{BaseUrl}/Wishlist");

        await Expect(Page).ToHaveURLAsync(new Regex(".*/Account/Login.*"));
    }

    /// <summary>
    /// UI-WISH-03: Add Wishlist modal opens
    /// Expected: Book search modal is visible
    /// </summary>
    [Fact]
    public async Task UI_WISH_03_AddBookModal_Opens()
    {
        await LoginAsync("test@test.com", "Test1234!");

        await Page.GotoAsync($"{BaseUrl}/Wishlist");

        await Page.ClickAsync("#wishlist-add-btn");

        await Expect(Page.Locator("#addWishlistModal")).ToBeVisibleAsync();
        await Expect(Page.Locator("#wishlist-book-search-input")).ToBeVisibleAsync();
        await Expect(Page.Locator("#wishlist-book-search-btn")).ToBeVisibleAsync();
    }

    /// <summary>
    /// UI-WISH-04: Available only filter is visible and clickable
    /// Expected: Available-only filter reloads wishlist page
    /// </summary>
    [Fact]
    public async Task UI_WISH_04_AvailableOnlyFilter_Works()
    {
        await LoginAsync("test@test.com", "Test1234!");

        await Page.GotoAsync($"{BaseUrl}/Wishlist");

        await Expect(Page.Locator("#wishlist-filter-available-btn")).ToBeVisibleAsync();

        await Page.ClickAsync("#wishlist-filter-available-btn");

        await Expect(Page).ToHaveURLAsync(new Regex(".*/Wishlist.*availableOnly=True.*|.*/Wishlist.*availableOnly=true.*"));
    }

    /// <summary>
    /// UI-WISH-05: User can remove wishlist item if one exists
    /// Expected: Remove button can be clicked
    /// </summary>
    [Fact]
    public async Task UI_WISH_05_UserCanRemoveWishlistItem()
    {
        await LoginAsync("test@test.com", "Test1234!");

        await Page.GotoAsync($"{BaseUrl}/Wishlist");

        var removeButtons = Page.Locator(".wishlist-remove-btn");

        if (await removeButtons.CountAsync() > 0)
        {
            await removeButtons.First.ClickAsync();

            await Expect(Page).ToHaveURLAsync(new Regex(".*/Wishlist.*"));
        }
    }

    /// <summary>
    /// UI-WISH-06: Matching listing button is visible when match exists
    /// Expected: View Listings button opens matching listings page
    /// </summary>
    [Fact]
    public async Task UI_WISH_06_ViewMatchingListings_WhenAvailable()
    {
        await LoginAsync("test@test.com", "Test1234!");

        await Page.GotoAsync($"{BaseUrl}/Wishlist");

        var viewListingsButtons = Page.Locator(".wishlist-view-listings-btn");

        if (await viewListingsButtons.CountAsync() > 0)
        {
            await viewListingsButtons.First.ClickAsync();

            await Expect(Page).ToHaveURLAsync(new Regex(".*/Wishlist/Matches.*"));
        }
    }
}