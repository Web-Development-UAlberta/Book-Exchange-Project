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

        await Page.WaitForURLAsync($"{BaseUrl}/");
    }

    /// <summary>
    /// UI-LIST-01: Listing index page loads
    /// Expected: Listing page and listing list are visible
    /// </summary>
    [Fact]
    public async Task UI_LIST_01_Index_Loads()
    {
        await Page.GotoAsync($"{BaseUrl}/Listing");

        await Expect(Page).ToHaveTitleAsync("Listings - Book_Exchange");
        await Expect(Page.Locator("#listing-list")).ToBeVisibleAsync();
        await Expect(Page.Locator("#create-listing-btn")).ToBeVisibleAsync();
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
    /// UI-LIST-04: User searches and selects a book from Google Books result
    /// Expected: Selected book card is shown and ISBN is filled
    /// </summary>
    [Fact]
    public async Task UI_LIST_04_UserCanSearchAndSelectBook()
    {
        await LoginAsync("test@test.com", "Test1234!");

        await Page.GotoAsync($"{BaseUrl}/Listing/Create");

        await Page.FillAsync("#book-search-input", "harry");
        await Page.ClickAsync("#book-search-btn");

        await Page.WaitForSelectorAsync(".book-result-item");

        await Page.Locator(".book-result-item").First.ClickAsync();

        await Expect(Page.Locator("#selected-book-card")).ToBeVisibleAsync();
        await Expect(Page.Locator("#selected-book-title")).ToBeVisibleAsync();
        await Expect(Page.Locator("#listing-isbn-input")).Not.ToHaveValueAsync("");
    }

    /// <summary>
    /// UI-LIST-05: Authenticated user creates listing
    /// Expected: User is redirected to listing details page
    /// </summary>
    [Fact]
    public async Task UI_LIST_05_UserCanCreateListing()
    {
        await LoginAsync("test@test.com", "Test1234!");

        await Page.GotoAsync($"{BaseUrl}/Listing/Create");

        await Page.FillAsync("#book-search-input", "harry");
        await Page.ClickAsync("#book-search-btn");

        await Page.WaitForSelectorAsync(".book-result-item");
        await Page.Locator(".book-result-item").First.ClickAsync();

        await Page.SelectOptionAsync("#listing-condition-select", new[] { "Good" });
        await Page.FillAsync("#listing-price-input", "12.50");
        await Page.FillAsync("#listing-weight-input", "400");

        await Page.ClickAsync("#create-listing-submit-btn");

        await Expect(Page).ToHaveURLAsync(new Regex(".*/Listing/Details.*"));
        await Expect(Page.Locator("#listing-detail-page")).ToBeVisibleAsync();
        await Expect(Page.Locator("#listing-book-title")).ToBeVisibleAsync();
        await Expect(Page.Locator("#listing-price")).ToContainTextAsync("12.50");
    }

    /// <summary>
    /// UI-LIST-06: Create listing validation works
    /// Expected: Required ISBN validation is shown when no book is selected
    /// </summary>
    [Fact]
    public async Task UI_LIST_06_CreateListingWithoutBook_ShowsValidation()
    {
        await LoginAsync("test@test.com", "Test1234!");

        await Page.GotoAsync($"{BaseUrl}/Listing/Create");

        await Page.SelectOptionAsync("#listing-condition-select", new[] { "Good" });
        await Page.FillAsync("#listing-price-input", "12.50");
        await Page.FillAsync("#listing-weight-input", "400");

        await Page.ClickAsync("#create-listing-submit-btn");

        await Expect(Page.Locator("[data-valmsg-for='Isbn']")).ToBeVisibleAsync();
    }

    /// <summary>
    /// UI-LIST-07: Listing detail page contains wireframe sections
    /// Expected: Main listing detail elements are visible
    /// </summary>
    [Fact]
    public async Task UI_LIST_07_DetailsPage_ShowsMainSections()
    {
        await Page.GotoAsync($"{BaseUrl}/Listing");

        await Page.Locator(".listing-view-btn").First.ClickAsync();

        await Expect(Page.Locator("#listing-detail-page")).ToBeVisibleAsync();
        await Expect(Page.Locator("#listing-book-title")).ToBeVisibleAsync();
        await Expect(Page.Locator("#listing-author-name")).ToBeVisibleAsync();
        await Expect(Page.Locator("#listing-price")).ToBeVisibleAsync();
        await Expect(Page.Locator("#listing-condition-badge")).ToBeVisibleAsync();
        await Expect(Page.Locator("#condition-guide")).ToBeVisibleAsync();
        await Expect(Page.Locator("#listing-seller-card")).ToBeVisibleAsync();
        await Expect(Page.Locator("#propose-swap-btn")).ToBeVisibleAsync();
        await Expect(Page.Locator("#send-offer-btn")).ToBeVisibleAsync();
        await Expect(Page.Locator("#message-seller-btn")).ToBeVisibleAsync();
    }
}