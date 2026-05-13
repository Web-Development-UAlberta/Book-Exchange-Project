using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace UI;

public class HomeTests : PageTest
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
            throw new Exception($"Login failed for {email} – check the user exists in the database.");
    }

    /// <summary>
    /// UI-HOME-01: Home page loads without error for an unauthenticated user.
    /// Expected: Hero section is visible and the page title is correct.
    /// </summary>
    [Fact]
    public async Task UI_HOME_01_Index_LoadsSuccessfully_Unauthenticated()
    {
        await Page.GotoAsync($"{BaseUrl}/");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page).ToHaveTitleAsync("Exchange, Buy & Sell Books - Book_Exchange");
        await Expect(Page.Locator("#hero-section")).ToBeVisibleAsync();
    }

    /// <summary>
    /// UI-HOME-02: Authenticated user visiting / is redirected to the Dashboard.
    /// Expected: URL contains /Home/Dashboard.
    /// </summary>
    [Fact]
    public async Task UI_HOME_02_Index_AuthenticatedUser_RedirectsToDashboard()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page).ToHaveURLAsync(new Regex(".*/Home/Dashboard.*"));
    }

    /// <summary>
    /// UI-HOME-03: Hero section renders the search form with input and button.
    /// Expected: Home search form, search input, and search button are visible.
    /// </summary>
    [Fact]
    public async Task UI_HOME_03_HeroSection_ShowsSearchForm()
    {
        await Page.GotoAsync($"{BaseUrl}/");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page.Locator("#home-search-form")).ToBeVisibleAsync();
        await Expect(Page.Locator("#search-input")).ToBeVisibleAsync();
        await Expect(Page.Locator("#search-btn")).ToBeVisibleAsync();
    }

    /// <summary>
    /// UI-HOME-04: Submitting the search form navigates to the BookSearch results page.
    /// Expected: URL contains /BookSearch/Search and the query string matches the input.
    /// </summary>
    [Fact]
    public async Task UI_HOME_04_HeroSearch_SubmitForm_NavigatesToSearchResults()
    {
        await Page.GotoAsync($"{BaseUrl}/");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Page.FillAsync("#search-input", "harry potter");
        await Page.ClickAsync("#search-btn");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page).ToHaveURLAsync(new Regex(".*/BookSearch/Search.*"));
    }

    /// <summary>
    /// UI-HOME-05: Featured listings section is present on the page.
    /// Expected: Featured listings section is visible.
    /// </summary>
    [Fact]
    public async Task UI_HOME_05_FeaturedListings_SectionIsVisible()
    {
        await Page.GotoAsync($"{BaseUrl}/");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page.Locator("#featured-listings")).ToBeVisibleAsync();
    }

    /// <summary>
    /// UI-HOME-06: Featured listings grid renders listing cards when listings exist.
    /// Expected: Featured listings grid is visible and contains at least one
    ///           listing card with a title.
    /// Requires at least one seeded listing in the database.
    /// </summary>
    [Fact]
    public async Task UI_HOME_06_FeaturedListings_ShowsCards_WhenListingsExist()
    {
        await Page.GotoAsync($"{BaseUrl}/");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page.Locator("#featured-listings-grid")).ToBeVisibleAsync();
        await Expect(Page.Locator(".listings-card").First).ToBeVisibleAsync();
        await Expect(Page.Locator(".listing-card-title").First).ToBeVisibleAsync();
    }

    /// <summary>
    /// UI-HOME-07: Each listing card shows condition and a Sell or Swap badge.
    /// Expected: The first card has a visible listing card condition and either
    ///           a Sell or Swap badge.
    /// </summary>
    [Fact]
    public async Task UI_HOME_07_ListingCard_ShowsConditionAndTypeBadge()
    {
        await Page.GotoAsync($"{BaseUrl}/");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var firstCard = Page.Locator(".listings-card").First;
        await Expect(firstCard).ToBeVisibleAsync();

        await Expect(firstCard.Locator(".listing-card-condition")).ToBeVisibleAsync();

        var hasSell = await firstCard.Locator(".badge-sell").CountAsync() > 0;
        var hasSwap = await firstCard.Locator(".badge-swap").CountAsync() > 0;
        Assert.True(hasSell || hasSwap, "Expected each listing card to have either a Sell or Swap badge.");
    }

    /// <summary>
    /// UI-HOME-08: Sell listing card shows a price alongside the Sell badge.
    /// Expected: A card with a Sell badge also has a visible price.
    /// Requires at least one seeded listing with Price > 0.
    /// </summary>
    [Fact]
    public async Task UI_HOME_08_SellListingCard_ShowsPrice()
    {
        await Page.GotoAsync($"{BaseUrl}/");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var sellCard = Page.Locator(".listings-card", new() { Has = Page.Locator(".badge-sell") }).First;

        if (await sellCard.CountAsync() == 0)
        {
            // Skip if no listings available
            return;
        }

        await Expect(sellCard.Locator(".listings-card-price")).ToBeVisibleAsync();
    }

    /// <summary>
    /// UI-HOME-09: "View all listings" button is visible and links to the Listing index.
    /// Expected: View all listings button is visible and it redirects to the Listing index which requires authentication.
    /// </summary>
    [Fact]
    public async Task UI_HOME_09_FeaturedListings_ViewAllBtn_IsVisible()
    {
        await Page.GotoAsync($"{BaseUrl}/");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var btn = Page.Locator("#view-all-listings-btn");
        await Expect(btn).ToBeVisibleAsync();

        var href = await btn.GetAttributeAsync("href");
        Assert.Contains("/Listing", href);
    }

    /// <summary>
    /// UI-HOME-10: "How it works" section is visible with all three steps.
    /// Expected: How it works, Step1, Step2, and Step3 are all visible.
    /// </summary>
    [Fact]
    public async Task UI_HOME_10_HowItWorks_ShowsAllThreeSteps()
    {
        await Page.GotoAsync($"{BaseUrl}/");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page.Locator("#how-it-works")).ToBeVisibleAsync();
        await Expect(Page.Locator("#step1")).ToBeVisibleAsync();
        await Expect(Page.Locator("#step2")).ToBeVisibleAsync();
        await Expect(Page.Locator("#step3")).ToBeVisibleAsync();
    }
}