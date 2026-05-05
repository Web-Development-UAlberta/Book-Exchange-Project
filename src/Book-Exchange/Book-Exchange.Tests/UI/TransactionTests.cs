using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace UI;

public class TransactionTests : PageTest
{
    private const string BaseUrl = "http://localhost:5261";

    // Helper: logs in as a test user before each test that needs auth
    private async Task LoginAsync(string email, string password)
    {
        await Page.GotoAsync($"{BaseUrl}/Identity/Account/Login");
        await Page.FillAsync("#Input_Email", email);
        await Page.FillAsync("#Input_Password", password);
        await Page.ClickAsync("#login-submit");
        await Page.WaitForURLAsync($"{BaseUrl}/");
    }

    /// <summary>
    /// UI-TRANS-01: Transaction index page loads with tabs
    /// </summary>
    [Fact]
    public async Task TransactionIndex_LoadsWithActiveTabs()
    {
        await LoginAsync("test@test.com", "Test1234!");
        await Page.GotoAsync($"{BaseUrl}/Transaction");

        await Expect(Page).ToHaveTitleAsync("Transactions - Book_Exchange");
        await Expect(Page.Locator("#transactions-tabs")).ToBeVisibleAsync();
        await Expect(Page.Locator("#active-tab")).ToBeVisibleAsync();
        await Expect(Page.Locator("#history-tab")).ToBeVisibleAsync();
    }

    /// <summary>
    /// UI-TRANS-02: Unauthenticated user is redirected to login
    /// </summary>
    [Fact]
    public async Task TransactionIndex_UnauthenticatedUser_RedirectsToLogin()
    {
        await Page.GotoAsync($"{BaseUrl}/Transaction");
        await Expect(Page).ToHaveURLAsync(new Regex(".*/Account/Login.*"));
    }

    /// <summary>
    /// UI-TRANS-03: History tab shows completed transactions
    /// </summary>
    [Fact]
    public async Task TransactionIndex_HistoryTab_ShowsCompletedTransactions()
    {
        await LoginAsync("test@test.com", "Test1234!");
        await Page.GotoAsync($"{BaseUrl}/Transaction");

        await Page.ClickAsync("#history-tab");
        await Expect(Page.Locator("#history-panel")).ToBeVisibleAsync();
    }
}