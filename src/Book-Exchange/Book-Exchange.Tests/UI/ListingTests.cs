using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace UI;

public class ListingTests : PageTest
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
    /// UI-LIST-01: Listing index page loads
    /// Expected: Listing page loads, if a listing exists it is shown, otherwise no listings message is shown
    /// </summary>
    [Fact]
    public async Task UI_LIST_01_Index_Loads()
    {
        await LoginAsync("test@test.com", "Test1234!");

        await Page.GotoAsync($"{BaseUrl}/Listing");

        await Expect(Page).ToHaveTitleAsync("Listings - Book_Exchange");
        await Expect(Page.Locator("#create-listing-btn")).ToBeVisibleAsync();

        var hasListings = await Page.Locator("#listing-list .listing-card").CountAsync() > 0;

        if (hasListings)
            await Expect(Page.Locator("#listing-list")).ToBeVisibleAsync();
        else
            await Expect(Page.Locator("#no-listings-message")).ToBeVisibleAsync();
    }

    /// <summary>
    /// UI-LIST-02: Unauthenticated user cannot open create listing page
    /// Expected: User is redirected to login page
    /// </summary>
    [Fact]
    public async Task UI_LIST_02_Create_UnauthenticatedUser_RedirectsToLogin()
    {
        await Page.GotoAsync($"{BaseUrl}/Listing/Create");

        await Expect(Page).ToHaveURLAsync(new Regex(".*/Account/Login.*"));
    }

    /// <summary>
    /// UI-LIST-03: Authenticated user can open create listing page
    /// Expected: Create listing form is visible
    /// </summary>
    [Fact]
    public async Task UI_LIST_03_Create_AuthenticatedUser_LoadsForm()
    {
        await LoginAsync("test@test.com", "Test1234!");

        await Page.GotoAsync($"{BaseUrl}/Listing/Create");

        await Expect(Page).ToHaveTitleAsync("Create Listing - Book_Exchange");
        await Expect(Page.Locator("#create-listing-form")).ToBeVisibleAsync();
        await Expect(Page.Locator("#book-search-input")).ToBeVisibleAsync();
        await Expect(Page.Locator("#book-search-btn")).ToBeVisibleAsync();
        await Expect(Page.Locator("#listing-isbn-input")).ToBeVisibleAsync();
        await Expect(Page.Locator("#listing-condition-select")).ToBeVisibleAsync();
        await Expect(Page.Locator("#listing-price-input")).ToBeVisibleAsync();
        await Expect(Page.Locator("#listing-weight-input")).ToBeVisibleAsync();
        await Expect(Page.Locator("#create-listing-submit-btn")).ToBeVisibleAsync();
    }

    /// <summary>
    /// UI-LIST-04: Create listing validation works
    /// Expected: Required ISBN validation is shown when no book is selected
    /// </summary>
    [Fact]
    public async Task UI_LIST_04_CreateListingWithoutBook_ShowsValidation()
    {
        await LoginAsync("test@test.com", "Test1234!");

        await Page.GotoAsync($"{BaseUrl}/Listing/Create");

        await Page.SelectOptionAsync("#listing-condition-select", new[] { "Good" });
        await Page.FillAsync("#listing-price-input", "12.50");
        await Page.FillAsync("#listing-weight-input", "400");

        await Page.ClickAsync("#create-listing-submit-btn");

        await Expect(Page.Locator("[data-valmsg-for='Isbn']")).ToBeVisibleAsync();
    }
}