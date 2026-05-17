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

    /// <summary>
    /// UI-ADDRESS-04: Validation prevents empty create
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
    /// UI-ADDRESS-05: User can navigate back to list
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
    /// UI-ADDRESS-06: Address table is visible when records exist
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