using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace UI;

public class AddressTests : PageTest
{
    private const string BaseUrl = "http://localhost:5261";

    // Helper: logs in as a test user before each test that needs auth
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
    /// UI-ADDRESS-01: Address index page loads
    /// </summary>
    [Fact]
    public async Task AddressIndex_LoadsSuccessfully()
    {
        await LoginAsync("test@test.com", "Test1234!");

        await Page.GotoAsync($"{BaseUrl}/Addresses");

        await Expect(Page).ToHaveTitleAsync("My Addresses - Book_Exchange");

        await Expect(Page.Locator("[data-testid='address-index-page']"))
            .ToBeVisibleAsync();

        await Expect(Page.Locator("[data-testid='create-address-link']"))
            .ToBeVisibleAsync();
    }

    /// <summary>
    /// UI-ADDRESS-02: Unauthenticated user redirects to login
    /// </summary>
    [Fact]
    public async Task AddressIndex_UnauthenticatedUser_RedirectsToLogin()
    {
        await Page.GotoAsync($"{BaseUrl}/Addresses");

        await Expect(Page)
            .ToHaveURLAsync(new Regex(".*/Account/Login.*"));
    }

    /// <summary>
    /// UI-ADDRESS-03: Create page loads
    /// </summary>
    [Fact]
    public async Task CreateAddress_LoadsSuccessfully()
    {
        await LoginAsync("test@test.com", "Test1234!");

        await Page.GotoAsync($"{BaseUrl}/Addresses/Create");

        await Expect(Page.Locator("[data-testid='create-address-page']"))
            .ToBeVisibleAsync();

        await Expect(Page.Locator("[data-testid='address-search-input']"))
            .ToBeVisibleAsync();

        await Expect(Page.Locator("[data-testid='save-address-button']"))
            .ToBeVisibleAsync();
    }

    // FIXME: These are testing that the API is working properly and I don't think we need to test for that in UI tests
    // should be moved to unit tests if we really want to test that the API is working
    // /// <summary>
    // /// UI-ADDRESS-04: User can search addresses
    // /// </summary>
    // [Fact]
    // public async Task CreateAddress_SearchAddress_ShowsResults()
    // {
    //     await LoginAsync("test@test.com", "Test1234!");

    //     await Page.GotoAsync($"{BaseUrl}/Addresses/Create");

    //     await Page.FillAsync(
    //         "[data-testid='address-search-input']",
    //         "660 171 ST Edmonton"
    //     );

    //     await Page.ClickAsync("[data-testid='search-address-button']");

    //     await Expect(Page.Locator("[data-testid='address-search-results']"))
    //         .ToBeVisibleAsync();

    //     await Expect(Page.Locator("[data-testid='address-result-item']").First)
    //         .ToBeVisibleAsync();
    // }

    // /// <summary>
    // /// UI-ADDRESS-05: Selecting an address populates form
    // /// </summary>
    // [Fact]
    // public async Task CreateAddress_SelectAddress_PopulatesFields()
    // {
    //     await LoginAsync("test@test.com", "Test1234!");

    //     await Page.GotoAsync($"{BaseUrl}/Addresses/Create");

    //     await Page.FillAsync(
    //         "[data-testid='address-search-input']",
    //         "660 171 ST Edmonton"
    //     );

    //     await Page.ClickAsync("[data-testid='search-address-button']");

    //     var firstResult = Page
    //         .Locator("[data-testid='address-result-item']")
    //         .First;

    //     await firstResult.ClickAsync();

    //     var fullName = await Page
    //         .Locator("[data-testid='full-name-input']")
    //         .InputValueAsync();

    //     var placeId = await Page
    //         .Locator("[data-testid='google-place-id-input']")
    //         .InputValueAsync();

    //     Assert.False(string.IsNullOrWhiteSpace(fullName));
    //     Assert.False(string.IsNullOrWhiteSpace(placeId));
    // }

    /// <summary>
    /// UI-ADDRESS-06: Validation prevents empty create
    /// </summary>
    [Fact]
    public async Task CreateAddress_EmptyForm_ShowsValidation()
    {
        await LoginAsync("test@test.com", "Test1234!");

        await Page.GotoAsync($"{BaseUrl}/Addresses/Create");

        await Page.ClickAsync("[data-testid='save-address-button']");

        await Expect(Page.Locator("[data-valmsg-for='FullName']"))
        .Not.ToBeEmptyAsync();
    }

    /// <summary>
    /// UI-ADDRESS-07: User can navigate back to list
    /// </summary>
    [Fact]
    public async Task CreateAddress_BackButton_ReturnsToIndex()
    {
        await LoginAsync("test@test.com", "Test1234!");

        await Page.GotoAsync($"{BaseUrl}/Addresses/Create");

        await Page.ClickAsync("[data-testid='back-to-address-list']");

        await Expect(Page)
            .ToHaveURLAsync($"{BaseUrl}/Addresses");
    }

    /// <summary>
    /// UI-ADDRESS-08: Address table is visible when records exist
    /// </summary>
    [Fact]
    public async Task AddressIndex_AddressTable_Visible()
    {
        await LoginAsync("test@test.com", "Test1234!");

        await Page.GotoAsync($"{BaseUrl}/Addresses");

        var table = Page.Locator("[data-testid='address-table']");

        if (await table.CountAsync() > 0)
        {
            await Expect(table).ToBeVisibleAsync();
        }
    }
}