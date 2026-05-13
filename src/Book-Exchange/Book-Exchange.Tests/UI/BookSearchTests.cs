using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace UI;

public class BookSearchTests : PageTest
{
    private const string BaseUrl = "http://localhost:5261";

    /// <summary>
    /// UI-BOOK-01: Book search page loads with search bar
    /// </summary>
    [Fact]
    public async Task BookSearch_LoadsWithSearchBar()
    {
        await Page.GotoAsync($"{BaseUrl}/BookSearch");

        await Expect(Page).ToHaveTitleAsync("Book Search - Book_Exchange");
        await Expect(Page.Locator("#book-search-form")).ToBeVisibleAsync();
        await Expect(Page.Locator("#book-search-input")).ToBeVisibleAsync();
        await Expect(Page.Locator("#book-search-button")).ToBeVisibleAsync();
    }

    /// <summary>
    /// UI-BOOK-02: Empty search shows no books found
    /// </summary>
    [Fact]
    public async Task BookSearch_EmptySearch_ShowsNoBooksFound()
    {
        await Page.GotoAsync($"{BaseUrl}/BookSearch");

        await Page.FillAsync("#book-search-input", "");
        await Page.ClickAsync("#book-search-button");

        await Expect(Page.Locator("#no-books-found")).ToBeVisibleAsync();
    }


    /// <summary>
    /// UI-BOOK-03: Preview link is visible when available
    /// </summary>
    [Fact]
    public async Task BookSearch_ResultWithPreview_ShowsPreviewButton()
    {
        await Page.GotoAsync($"{BaseUrl}/BookSearch");

        await Page.FillAsync("#book-search-input", "harry");
        await Page.ClickAsync("#book-search-button");

        var previewButton = Page.Locator(".book-preview-link").First;

        if (await previewButton.CountAsync() > 0)
        {
            await Expect(previewButton).ToBeVisibleAsync();
        }
    }
}