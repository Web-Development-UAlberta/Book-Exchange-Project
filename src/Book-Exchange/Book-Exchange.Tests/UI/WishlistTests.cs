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

        await Page.WaitForURLAsync($"{BaseUrl}/");
    }

    /// <summary>
    /// UI-WISH-01: Wishlist page loads
    /// Expected: Wishlist page and Add Book button are visible
    /// </summary>
    [Fact]
    public async Task UI_WISH_01_Index_Loads()
    {
        await LoginAsync("test@test.com", "Test1234!");

        await Page.GotoAsync($"{BaseUrl}/Wishlist");

        await Expect(Page).ToHaveTitleAsync("My Wishlist - Book_Exchange");
        await Expect(Page.Locator("#wishlist-page")).ToBeVisibleAsync();
        await Expect(Page.Locator("#wishlist-add-btn")).ToBeVisibleAsync();
        await Expect(Page.Locator("#wishlist-items-list")).ToBeVisibleAsync();
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

    // FIXME: These are testing that the API is working properly and I don't think we need to test for that in UI tests
    // should be moved to unit tests if we really want to test that the API is working
    // /// <summary>
    // /// UI-WISH-04: User searches and selects a book
    // /// Expected: Add Selected Book button becomes enabled
    // /// </summary>
    // [Fact]
    // public async Task UI_WISH_04_UserCanSearchAndSelectBook()
    // {
    //     await LoginAsync("test@test.com", "Test1234!");

    //     await Page.GotoAsync($"{BaseUrl}/Wishlist");

    //     await Page.ClickAsync("#wishlist-add-btn");

    //     await Page.FillAsync("#wishlist-book-search-input", "harry");
    //     await Page.ClickAsync("#wishlist-book-search-btn");

    //     await Page.WaitForSelectorAsync(".wishlist-book-result-item");

    //     await Page.Locator(".wishlist-book-result-item").First.ClickAsync();

    //     await Expect(Page.Locator("#wishlist-confirm-add-btn")).ToBeEnabledAsync();
    // }

    // FIXME: These are testing that the API is working properly and I don't think we need to test for that in UI tests
    // should be moved to unit tests if we really want to test that the API is working
    // /// <summary>
    // /// UI-WISH-05: User can add selected book to wishlist
    // /// Expected: User returns to wishlist page and item list is visible
    // /// </summary>
    // [Fact]
    // public async Task UI_WISH_05_UserCanAddBookToWishlist()
    // {
    //     await LoginAsync("test@test.com", "Test1234!");

    //     await Page.GotoAsync($"{BaseUrl}/Wishlist");

    //     await Page.ClickAsync("#wishlist-add-btn");

    //     await Page.FillAsync("#wishlist-book-search-input", "harry");
    //     await Page.ClickAsync("#wishlist-book-search-btn");

    //     await Page.WaitForSelectorAsync(".wishlist-book-result-item");
    //     await Page.Locator(".wishlist-book-result-item").First.ClickAsync();

    //     await Page.ClickAsync("#wishlist-confirm-add-btn");

    //     await Expect(Page).ToHaveURLAsync(new Regex(".*/Wishlist.*"));
    //     await Expect(Page.Locator("#wishlist-items-list")).ToBeVisibleAsync();
    //     await Expect(Page.Locator(".wishlist-item").First).ToBeVisibleAsync();
    // }

    /// <summary>
    /// UI-WISH-06: Available only filter is visible and clickable
    /// Expected: Available-only filter reloads wishlist page
    /// </summary>
    [Fact]
    public async Task UI_WISH_06_AvailableOnlyFilter_Works()
    {
        await LoginAsync("test@test.com", "Test1234!");

        await Page.GotoAsync($"{BaseUrl}/Wishlist");

        await Expect(Page.Locator("#wishlist-filter-available-btn")).ToBeVisibleAsync();

        await Page.ClickAsync("#wishlist-filter-available-btn");

        await Expect(Page).ToHaveURLAsync(new Regex(".*/Wishlist.*availableOnly=True.*|.*/Wishlist.*availableOnly=true.*"));
    }

    // FIXME: These are testing that the API is working properly and I don't think we need to test for that in UI tests
    // should be moved to unit tests if we really want to test that the API is working
    // /// <summary>
    // /// UI-WISH-07: Wishlist search input is visible
    // /// Expected: User can type in wishlist search box
    // /// </summary>
    // [Fact]
    // public async Task UI_WISH_07_SearchInput_IsVisible()
    // {
    //     await LoginAsync("test@test.com", "Test1234!");

    //     await Page.GotoAsync($"{BaseUrl}/Wishlist");

    //     await Expect(Page.Locator("#wishlist-search-input")).ToBeVisibleAsync();

    //     await Page.FillAsync("#wishlist-search-input", "harry");

    //     await Expect(Page.Locator("#wishlist-search-input")).ToHaveValueAsync("harry");
    // }

    /// <summary>
    /// UI-WISH-08: User can remove wishlist item if one exists
    /// Expected: Remove button can be clicked
    /// </summary>
    [Fact]
    public async Task UI_WISH_08_UserCanRemoveWishlistItem()
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
    /// UI-WISH-09: Matching listing button is visible when match exists
    /// Expected: View Listings button opens matching listings page
    /// </summary>
    [Fact]
    public async Task UI_WISH_09_ViewMatchingListings_WhenAvailable()
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