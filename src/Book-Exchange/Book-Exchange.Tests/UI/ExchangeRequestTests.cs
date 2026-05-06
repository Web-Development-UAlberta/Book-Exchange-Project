using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace UI;

public class ExchangeRequestTests : PageTest
{
    //private const string BaseUrl = "http://localhost:5261";

    //private async Task LoginAsync(string email, string password)
    //{
    //    await Page.GotoAsync($"{BaseUrl}/Identity/Account/Login");
    //    await Page.FillAsync("#Input_Email", email);
    //    await Page.FillAsync("#Input_Password", password);
    //    await Page.ClickAsync("#login-submit");
    //    await Page.WaitForURLAsync($"{BaseUrl}/");
    //}

    ///// <summary>
    ///// UI-EXCH-01: Exchange request index loads with tabs
    ///// Expected: Received, Sent, and History tabs are visible
    ///// </summary>
    //[Fact]
    //public async Task UI_EXCH_01_Index_LoadsWithTabs()
    //{
    //    await LoginAsync("test@test.com", "Test1234!");

    //    await Page.GotoAsync($"{BaseUrl}/ExchangeRequest");

    //    await Expect(Page).ToHaveTitleAsync("Exchange Requests - Book_Exchange");
    //    await Expect(Page.Locator("#exchange-index-page")).ToBeVisibleAsync();
    //    await Expect(Page.Locator("#received-tab")).ToBeVisibleAsync();
    //    await Expect(Page.Locator("#sent-tab")).ToBeVisibleAsync();
    //    await Expect(Page.Locator("#history-tab")).ToBeVisibleAsync();
    //}

    ///// <summary>
    ///// UI-EXCH-02: Unauthenticated user is redirected to login
    ///// Expected: Login page is shown
    ///// </summary>
    //[Fact]
    //public async Task UI_EXCH_02_UnauthenticatedUser_RedirectsToLogin()
    //{
    //    await Page.GotoAsync($"{BaseUrl}/ExchangeRequest");

    //    await Expect(Page).ToHaveURLAsync(new Regex(".*/Account/Login.*"));
    //}

    ///// <summary>
    ///// UI-EXCH-03: Sent tab opens
    ///// Expected: Sent panel is visible
    ///// </summary>
    //[Fact]
    //public async Task UI_EXCH_03_SentTab_Loads()
    //{
    //    await LoginAsync("test@test.com", "Test1234!");

    //    await Page.GotoAsync($"{BaseUrl}/ExchangeRequest");

    //    await Page.ClickAsync("#sent-tab");

    //    await Expect(Page).ToHaveURLAsync(new Regex(".*/ExchangeRequest.*tab=sent.*"));
    //}

    ///// <summary>
    ///// UI-EXCH-04: History tab opens
    ///// Expected: History panel is visible
    ///// </summary>
    //[Fact]
    //public async Task UI_EXCH_04_HistoryTab_Loads()
    //{
    //    await LoginAsync("test@test.com", "Test1234!");

    //    await Page.GotoAsync($"{BaseUrl}/ExchangeRequest");

    //    await Page.ClickAsync("#history-tab");

    //    await Expect(Page).ToHaveURLAsync(new Regex(".*/ExchangeRequest.*tab=history.*"));
    //}

    ///// <summary>
    ///// UI-EXCH-05: Wishlist can start exchange request when match exists
    ///// Expected: User goes to create exchange request page
    ///// </summary>
    //[Fact]
    //public async Task UI_EXCH_05_WishlistCreateExchangeButton_OpensCreatePage()
    //{
    //    await LoginAsync("test@test.com", "Test1234!");

    //    await Page.GotoAsync($"{BaseUrl}/Wishlist");

    //    var buttons = Page.Locator(".wishlist-create-exchange-btn");

    //    if (await buttons.CountAsync() > 0)
    //    {
    //        await buttons.First.ClickAsync();

    //        await Expect(Page).ToHaveURLAsync(new Regex(".*/ExchangeRequest/Create.*"));
    //        await Expect(Page.Locator("#exchange-create-page")).ToBeVisibleAsync();
    //        await Expect(Page.Locator("#exchange-create-form")).ToBeVisibleAsync();
    //    }
    //}

    ///// <summary>
    ///// UI-EXCH-06: Create exchange request page has required form fields
    ///// Expected: Price, message, offered listings, and submit button are visible
    ///// </summary>
    //[Fact]
    //public async Task UI_EXCH_06_CreatePage_HasFormFields()
    //{
    //    await LoginAsync("test@test.com", "Test1234!");

    //    await Page.GotoAsync($"{BaseUrl}/Wishlist");

    //    var buttons = Page.Locator(".wishlist-create-exchange-btn");

    //    if (await buttons.CountAsync() > 0)
    //    {
    //        await buttons.First.ClickAsync();

    //        await Expect(Page.Locator("#target-listing-card")).ToBeVisibleAsync();
    //        await Expect(Page.Locator("#exchange-price-input")).ToBeVisibleAsync();
    //        await Expect(Page.Locator("#exchange-message-input")).ToBeVisibleAsync();
    //        await Expect(Page.Locator("#exchange-offered-listings")).ToBeVisibleAsync();
    //        await Expect(Page.Locator("#exchange-submit-btn")).ToBeVisibleAsync();
    //    }
    //}

    ///// <summary>
    ///// UI-EXCH-07: Received request owner can see accept/reject buttons
    ///// Expected: Accept and reject buttons appear when there are pending received requests
    ///// </summary>
    //[Fact]
    //public async Task UI_EXCH_07_ReceivedRequest_HasAcceptRejectButtons()
    //{
    //    await LoginAsync("test@test.com", "Test1234!");

    //    await Page.GotoAsync($"{BaseUrl}/ExchangeRequest?tab=received");

    //    var cards = Page.Locator(".exchange-request-card");

    //    if (await cards.CountAsync() > 0)
    //    {
    //        var acceptButtons = Page.Locator(".exchange-accept-btn");
    //        var rejectButtons = Page.Locator(".exchange-reject-btn");

    //        if (await acceptButtons.CountAsync() > 0)
    //        {
    //            await Expect(acceptButtons.First).ToBeVisibleAsync();
    //            await Expect(rejectButtons.First).ToBeVisibleAsync();
    //        }
    //    }
    //}
}