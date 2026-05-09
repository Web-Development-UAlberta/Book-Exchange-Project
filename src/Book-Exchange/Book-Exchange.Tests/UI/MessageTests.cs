using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace UI;

/// Seed helper
/// ──────────────────────
/// • test@test.com       / Test1234!  – primary user (sender perspective)
/// • otheruser@test.com  / Test1234!  – secondary user (receiver perspective)
///   Both users must have exchanged at least one message so that each inbox
///   renders at least one .conversation-item.
public class MessageTests : PageTest
{
    private const string BaseUrl = "http://localhost:5261";
    private const string TestEmail = "test@test.com";
    private const string OtherUserEmail = "otheruser@test.com";
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
            throw new Exception($"Login failed for {email} – verify the user exists in the database.");
    }

    /// <summary>
    /// Lands on the inbox, waits for the first conversation item, and returns
    /// its absolute href so conversation tests can navigate directly without
    /// depending on a second locator wait.
    /// </summary>
    private async Task<string> GetFirstConversationUrlAsync()
    {
        await Page.GotoAsync($"{BaseUrl}/Message");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var firstItem = Page.Locator("#conversation-list .conversation-item").First;
        await firstItem.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10_000 });

        var href = await firstItem.GetAttributeAsync("href")
            ?? throw new Exception("First .conversation-item has no href attribute.");

        return href.StartsWith("http") ? href : BaseUrl + href;
    }

    /// <summary>
    /// UI-MSG-01: Messages index page loads for an authenticated user.
    /// Expected: Message page, chat panel, and conversation list are visible
    /// </summary>
    [Fact]
    public async Task UI_MSG_01_Index_LoadsSuccessfully()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/Message");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page).ToHaveTitleAsync("Messages - Book_Exchange");
        await Expect(Page.Locator("#messages-page")).ToBeVisibleAsync();
        await Expect(Page.Locator("#chat-panel")).ToBeVisibleAsync();
        await Expect(Page.Locator("#conversation-list")).ToBeVisibleAsync();
    }

    /// <summary>
    /// UI-MSG-02: Unauthenticated user is redirected to the login page.
    /// Expected: URL contains /Account/Login.
    /// </summary>
    [Fact]
    public async Task UI_MSG_02_Index_UnauthenticatedUser_RedirectsToLogin()
    {
        await Page.GotoAsync($"{BaseUrl}/Message");
        await Expect(Page).ToHaveURLAsync(new Regex(".*/Account/Login.*"));
    }

    /// <summary>
    /// UI-MSG-03: Inbox renders conversation items when messages exist.
    /// Expected: At least one conversation item is visible inside conversation list.
    /// </summary>
    [Fact]
    public async Task UI_MSG_03_Index_ShowsConversationItems()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/Message");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var firstItem = Page.Locator("#conversation-list .conversation-item").First;
        await Expect(firstItem).ToBeVisibleAsync();
    }

    /// <summary>
    /// UI-MSG-04: Conversation item carries the correct user id attribute.
    /// Expected: user id is present and a valid GUID on conversation item.
    /// </summary>
    [Fact]
    public async Task UI_MSG_04_Index_ConversationItem_HasDataUserId()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/Message");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var firstItem = Page.Locator("#conversation-list .conversation-item").First;
        await firstItem.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10_000 });

        var userId = await firstItem.GetAttributeAsync("data-user-id");

        Assert.False(string.IsNullOrWhiteSpace(userId),
            "data-user-id attribute must be present and non-empty on .conversation-item.");
        Assert.True(Guid.TryParse(userId, out _),
            $"data-user-id '{userId}' must be a valid GUID.");
    }

    /// <summary>
    /// UI-MSG-05: Clicking a conversation item navigates to the Conversation view.
    /// Expected: URL changes to /Message/Conversation/{otherUserId}.
    /// </summary>
    [Fact]
    public async Task UI_MSG_05_Index_ClickConversationItem_NavigatesToConversation()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/Message");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var firstItem = Page.Locator("#conversation-list .conversation-item").First;
        await firstItem.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10_000 });
        await firstItem.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page).ToHaveURLAsync(new Regex(".*/Message/Conversation.*"));
    }

    /// <summary>
    /// UI-MSG-06: The right-hand pane on the index page shows the placeholder message thread (no conversation selected state).
    /// Expected: message thread is visible on the index page.
    /// </summary>
    [Fact]
    public async Task UI_MSG_06_Index_RightPane_ShowsPlaceholder()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/Message");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page.Locator("#message-thread")).ToBeVisibleAsync();
    }

    /// <summary>
    /// UI-MSG-07: Conversation view loads for an authenticated user.
    /// Expected: message page, chat panel, chat header, message thread, message input, and message send button are all visible.
    /// </summary>
    [Fact]
    public async Task UI_MSG_07_Conversation_LoadsSuccessfully()
    {
        await LoginAsync();
        var url = await GetFirstConversationUrlAsync();
        await Page.GotoAsync(url);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page.Locator("#messages-page")).ToBeVisibleAsync();
        await Expect(Page.Locator("#chat-panel")).ToBeVisibleAsync();
        await Expect(Page.Locator("#chat-header")).ToBeVisibleAsync();
        await Expect(Page.Locator("#message-thread")).ToBeVisibleAsync();
        await Expect(Page.Locator("#message-input")).ToBeVisibleAsync();
        await Expect(Page.Locator("#message-send-btn")).ToBeVisibleAsync();
    }

    /// <summary>
    /// UI-MSG-08: Message thread renders existing messages.
    /// Expected: At least one message item is present inside the message thread.
    /// </summary>
    [Fact]
    public async Task UI_MSG_08_Conversation_Thread_ShowsMessages()
    {
        await LoginAsync();
        var url = await GetFirstConversationUrlAsync();
        await Page.GotoAsync(url);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page.Locator("#message-thread .message-item").First).ToBeVisibleAsync();
    }

    /// <summary>
    /// UI-MSG-09: Outgoing messages carry the message item outgoing class.
    /// Expected: At least one outgoing message item exists in the thread.
    /// </summary>
    [Fact]
    public async Task UI_MSG_09_Conversation_OutgoingMessages_HaveCorrectClass()
    {
        await LoginAsync();
        var url = await GetFirstConversationUrlAsync();
        await Page.GotoAsync(url);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page.Locator("#message-thread .message-item-outgoing").First).ToBeVisibleAsync();
    }

    /// <summary>
    /// UI-MSG-10: Incoming messages carry the message item incoming class.
    /// Logs in as otheruser@test.com so messages sent by test@test.com appear
    /// as incoming from this user's perspective.
    /// Expected: At least one incoming message item exists in the thread.
    /// </summary>
    [Fact]
    public async Task UI_MSG_10_Conversation_IncomingMessages_HaveCorrectClass()
    {
        await LoginAsync(OtherUserEmail);
        var url = await GetFirstConversationUrlAsync();
        await Page.GotoAsync(url);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page.Locator("#message-thread .message-item-incoming").First).ToBeVisibleAsync();
    }

    /// <summary>
    /// UI-MSG-11: Each message item carries a data message id attribute.
    /// Expected: The first message item has a non-empty, valid GUID data message id.
    /// </summary>
    [Fact]
    public async Task UI_MSG_11_Conversation_MessageItem_HasDataMessageId()
    {
        await LoginAsync();
        var url = await GetFirstConversationUrlAsync();
        await Page.GotoAsync(url);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var firstMsg = Page.Locator("#message-thread .message-item").First;
        await firstMsg.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        var messageId = await firstMsg.GetAttributeAsync("data-message-id");

        Assert.False(string.IsNullOrWhiteSpace(messageId),
            "data-message-id must be present and non-empty.");
        Assert.True(Guid.TryParse(messageId, out _),
            $"data-message-id '{messageId}' must be a valid GUID.");
    }

    /// <summary>
    /// UI-MSG-12: Message input field accepts text.
    /// Expected: Text typed into message input is reflected in the field value.
    /// </summary>
    [Fact]
    public async Task UI_MSG_12_Conversation_MessageInput_AcceptsText()
    {
        await LoginAsync();
        var url = await GetFirstConversationUrlAsync();
        await Page.GotoAsync(url);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Page.FillAsync("#message-input", "Hello from UI test!");
        var value = await Page.InputValueAsync("#message-input");
        Assert.Equal("Hello from UI test!", value);
    }

    /// <summary>
    /// UI-MSG-13: Sending a valid message appends it to the thread.
    /// Expected: After form submit the new message text appears in message thread.
    /// </summary>
    [Fact]
    public async Task UI_MSG_13_Conversation_SendMessage_AppearsInThread()
    {
        await LoginAsync();
        var url = await GetFirstConversationUrlAsync();
        await Page.GotoAsync(url);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var uniqueText = $"UI test message {Guid.NewGuid():N}";
        await Page.FillAsync("#message-input", uniqueText);
        await Page.ClickAsync("#message-send-btn");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page.Locator("#message-thread")).ToContainTextAsync(uniqueText);
    }

    /// <summary>
    /// UI-MSG-14: Submitting a blank message does not navigate away.
    /// Expected: HTML5 `required` validation keeps the browser on the same page.
    /// </summary>
    [Fact]
    public async Task UI_MSG_14_Conversation_SendEmptyMessage_DoesNotSubmit()
    {
        await LoginAsync();
        var url = await GetFirstConversationUrlAsync();
        await Page.GotoAsync(url);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Page.FillAsync("#message-input", "");
        await Page.ClickAsync("#message-send-btn");
        await Page.WaitForTimeoutAsync(600);

        await Expect(Page).ToHaveURLAsync(new Regex(".*/Message/Conversation.*"));
    }

    /// <summary>
    /// UI-MSG-15: Back link on the conversation page returns to the inbox.
    /// Expected: Clicking Back navigates to /Message.
    /// </summary>
    [Fact]
    public async Task UI_MSG_15_Conversation_BackLink_NavigatesToInbox()
    {
        await LoginAsync();
        var url = await GetFirstConversationUrlAsync();
        await Page.GotoAsync(url);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Page.Locator("#chat-header a").ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page).ToHaveURLAsync(new Regex(".*/Message$"));
    }

    /// <summary>
    /// UI-MSG-16: The "Messages" nav link in the layout navigates to the inbox.
    /// Expected: Clicking the link lands on /Message
    /// </summary>
    [Fact]
    public async Task UI_MSG_16_Navbar_MessagesLink_NavigatesToInbox()
    {
        await LoginAsync();
        await Page.GotoAsync($"{BaseUrl}/");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Page.Locator("nav a", new() { HasText = "Messages" }).First.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page).ToHaveURLAsync(new Regex(".*/Message$"));
        await Expect(Page.Locator("#messages-page")).ToBeVisibleAsync();
    }
}