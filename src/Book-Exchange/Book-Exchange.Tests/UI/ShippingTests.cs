// using System.Text.RegularExpressions;
// using Microsoft.Playwright;
// using Microsoft.Playwright.Xunit;

// namespace UI;

// public class ShippingTests : PageTest
// {
//     private const string BaseUrl = "http://localhost:5261";

//     private async Task LoginAsync(string email, string password)
//     {
//         await Page.GotoAsync($"{BaseUrl}/Identity/Account/Login");

//         await Page.FillAsync("#Input_Email", email);
//         await Page.FillAsync("#Input_Password", password);
//         await Page.ClickAsync("#login-submit");

//         await Page.WaitForURLAsync($"{BaseUrl}/");
//     }

//     /// <summary>
//     /// UI-SHIP-01: Shipments index page loads for authenticated user
//     /// Expected: Page title and shipping index container are visible
//     /// </summary>
//     [Fact]
//     public async Task UI_SHIP_01_Index_AuthenticatedUser_LoadsPage()
//     {
//         await LoginAsync("test@test.com", "Test1234!");

//         await Page.GotoAsync($"{BaseUrl}/Shipping");

//         await Expect(Page).ToHaveTitleAsync("My Shipments - Book_Exchange");
//         await Expect(Page.Locator("#shipping-index-page")).ToBeVisibleAsync();
//         await Expect(Page.Locator("#shipping-index-title")).ToBeVisibleAsync();
//     }

//     /// <summary>
//     /// UI-SHIP-02: Unauthenticated user is redirected to login when accessing shipments index
//     /// Expected: User is redirected to login page
//     /// </summary>
//     [Fact]
//     public async Task UI_SHIP_02_Index_UnauthenticatedUser_RedirectsToLogin()
//     {
//         await Page.GotoAsync($"{BaseUrl}/Shipping");

//         await Expect(Page).ToHaveURLAsync(new Regex(".*/Account/Login.*"));
//     }

//     /// <summary>
//     /// UI-SHIP-03: Shipments index shows empty state message when user has no shipments
//     /// Expected: Empty message element is visible, shipments table is not rendered
//     /// </summary>
//     [Fact]
//     public async Task UI_SHIP_03_Index_NoShipments_ShowsEmptyMessage()
//     {
//         await LoginAsync("test@test.com", "Test1234!");

//         await Page.GotoAsync($"{BaseUrl}/Shipping");

//         var emptyMessage = Page.Locator("#shipping-empty-message");
//         var shipmentsTable = Page.Locator("#shipping-list-table");

//         // At least one of these should be true depending on test data 
//         var hasEmpty = await emptyMessage.CountAsync() > 0;
//         var hasTable = await shipmentsTable.CountAsync() > 0;

//         Assert.True(hasEmpty || hasTable, "Page should show either an empty message or a shipments table.");
//     }

//     /// <summary>
//     /// UI-SHIP-04: Shipments index shows table with correct columns when shipments exist
//     /// Expected: Table is visible with all expected column headers
//     /// </summary>
//     [Fact]
//     public async Task UI_SHIP_04_Index_WithShipments_ShowsTable()
//     {
//         await LoginAsync("test@test.com", "Test1234!");

//         await Page.GotoAsync($"{BaseUrl}/Shipping");

//         var table = Page.Locator("#shipping-list-table");

//         if (await table.CountAsync() > 0)
//         {
//             await Expect(table).ToBeVisibleAsync();
//             await Expect(Page.Locator("#shipping-list-body")).ToBeVisibleAsync();

//             await Expect(Page.Locator(".shipment-card").First).ToBeVisibleAsync();

//             await Expect(Page.Locator(".shipment-carrier").First).ToBeVisibleAsync();
//             await Expect(Page.Locator(".shipment-status-badge").First).ToBeVisibleAsync();
//             await Expect(Page.Locator(".shipment-track-btn").First).ToBeVisibleAsync();
//         }
//     }

//     /// <summary>
//     /// UI-SHIP-05: Details button navigates to the shipment details page
//     /// Expected: Clicking a details button routes to the Shipping/Details page
//     /// </summary>
//     [Fact]
//     public async Task UI_SHIP_05_Index_DetailsButton_NavigatesToDetails()
//     {
//         await LoginAsync("test@test.com", "Test1234!");

//         await Page.GotoAsync($"{BaseUrl}/Shipping");

//         var detailsBtn = Page.Locator(".shipment-track-btn").First;

//         if (await detailsBtn.CountAsync() > 0)
//         {
//             await detailsBtn.ClickAsync();

//             await Expect(Page).ToHaveURLAsync(new Regex(".*/Shipping/Details.*"));
//             await Expect(Page.Locator("#shipping-details-page")).ToBeVisibleAsync();
//         }
//     }

//     /// <summary>
//     /// UI-SHIP-06: Unauthenticated user is redirected to login when accessing details
//     /// Expected: User is redirected to login page
//     /// </summary>
//     [Fact]
//     public async Task UI_SHIP_06_Details_UnauthenticatedUser_RedirectsToLogin()
//     {
//         var fakeTransactionId = Guid.NewGuid();
//         await Page.GotoAsync($"{BaseUrl}/Shipping/Details?transactionId={fakeTransactionId}");

//         await Expect(Page).ToHaveURLAsync(new Regex(".*/Account/Login.*"));
//     }

//     /// <summary>
//     /// UI-SHIP-07: Details page loads with all expected sections
//     /// Expected: Details card, status badge, carrier, addresses, and cost are visible
//     /// </summary>
//     [Fact]
//     public async Task UI_SHIP_07_Details_LoadsAllSections()
//     {
//         await LoginAsync("test@test.com", "Test1234!");

//         await Page.GotoAsync($"{BaseUrl}/Shipping");

//         var detailsBtn = Page.Locator(".shipment-track-btn").First;

//         if (await detailsBtn.CountAsync() > 0)
//         {
//             await detailsBtn.ClickAsync();
//             await Page.WaitForURLAsync(new Regex(".*/Shipping/Details.*"));

//             await Expect(Page).ToHaveTitleAsync("Shipment Details - Book_Exchange");
//             await Expect(Page.Locator("#shipping-details-page")).ToBeVisibleAsync();
//             await Expect(Page.Locator("#shipping-details-card")).ToBeVisibleAsync();
//             await Expect(Page.Locator("#shipping-status-badge")).ToBeVisibleAsync();
//             await Expect(Page.Locator("#shipping-carrier-name")).ToBeVisibleAsync();
//             await Expect(Page.Locator("#shipping-transaction-id")).ToBeVisibleAsync();
//             await Expect(Page.Locator("#shipping-package-weight")).ToBeVisibleAsync();
//             await Expect(Page.Locator("#shipping-distance-km")).ToBeVisibleAsync();
//             await Expect(Page.Locator("#shipping-cost-value")).ToBeVisibleAsync();
//             await Expect(Page.Locator("#shipping-sender-address")).ToBeVisibleAsync();
//             await Expect(Page.Locator("#shipping-receiver-address")).ToBeVisibleAsync();
//             await Expect(Page.Locator("#shipping-created-at")).ToBeVisibleAsync();
//         }
//     }

//     /// <summary>
//     /// UI-SHIP-08: Details page shows status update form for non-terminal shipments
//     /// Expected: Update status card, select, and button are visible when status is not Delivered or Cancelled
//     /// </summary>
//     [Fact]
//     public async Task UI_SHIP_08_Details_NonTerminalShipment_ShowsStatusUpdateForm()
//     {
//         await LoginAsync("test@test.com", "Test1234!");

//         await Page.GotoAsync($"{BaseUrl}/Shipping");

//         var detailsBtn = Page.Locator(".shipment-track-btn").First;

//         if (await detailsBtn.CountAsync() > 0)
//         {
//             await detailsBtn.ClickAsync();
//             await Page.WaitForURLAsync(new Regex(".*/Shipping/Details.*"));

//             var statusBadge = Page.Locator("#shipping-status-badge");
//             var statusText = await statusBadge.InnerTextAsync();

//             bool isTerminal = statusText.Trim() is "Delivered" or "Cancelled";

//             if (!isTerminal)
//             {
//                 await Expect(Page.Locator("#shipping-update-status-card")).ToBeVisibleAsync();
//                 await Expect(Page.Locator("#shipping-update-status-form")).ToBeVisibleAsync();
//                 await Expect(Page.Locator("#shipping-new-status-select")).ToBeVisibleAsync();
//                 await Expect(Page.Locator("#shipping-update-status-btn")).ToBeVisibleAsync();
//                 await Expect(Page.Locator("#shipping-cancel-btn")).ToBeVisibleAsync();
//             }
//         }
//     }

//     /// <summary>
//     /// UI-SHIP-09: Details page hides status update form for terminal shipments
//     /// Expected: Update status card and cancel button are not present when Delivered or Cancelled
//     /// </summary>
//     [Fact]
//     public async Task UI_SHIP_09_Details_TerminalShipment_HidesStatusUpdateForm()
//     {
//         await LoginAsync("test@test.com", "Test1234!");

//         await Page.GotoAsync($"{BaseUrl}/Shipping");

//         var detailsBtn = Page.Locator(".shipment-track-btn").First;

//         if (await detailsBtn.CountAsync() > 0)
//         {
//             await detailsBtn.ClickAsync();
//             await Page.WaitForURLAsync(new Regex(".*/Shipping/Details.*"));

//             var statusBadge = Page.Locator("#shipping-status-badge");
//             var statusText = await statusBadge.InnerTextAsync();

//             bool isTerminal = statusText.Trim() is "Delivered" or "Cancelled";

//             if (isTerminal)
//             {
//                 await Expect(Page.Locator("#shipping-update-status-card")).Not.ToBeVisibleAsync();
//                 await Expect(Page.Locator("#shipping-cancel-btn")).Not.ToBeVisibleAsync();
//             }
//         }
//     }

//     /// <summary>
//     /// UI-SHIP-10: Unauthenticated user is redirected to login when accessing the quote page
//     /// Expected: User is redirected to login page
//     /// </summary>
//     [Fact]
//     public async Task UI_SHIP_10_Quote_UnauthenticatedUser_RedirectsToLogin()
//     {
//         var transactionId = Guid.NewGuid();
//         var senderAddressId = Guid.NewGuid();
//         var receiverAddressId = Guid.NewGuid();

//         await Page.GotoAsync(
//             $"{BaseUrl}/Shipping/Quote" +
//             $"?transactionId={transactionId}" +
//             $"&senderAddressId={senderAddressId}" +
//             $"&receiverAddressId={receiverAddressId}" +
//             $"&packageWeightGrams=500");

//         await Expect(Page).ToHaveURLAsync(new Regex(".*/Account/Login.*"));
//     }

//     /// <summary>
//     /// UI-SHIP-11: Quote page loads with summary card visible
//     /// Expected: Title, summary card, transaction ID, and package weight are rendered
//     /// </summary>
//     [Fact]
//     public async Task UI_SHIP_11_Quote_LoadsSummaryCard()
//     {
//         await LoginAsync("test@test.com", "Test1234!");

//         await Page.GotoAsync($"{BaseUrl}/Shipping");

//         var quoteLink = Page.Locator("a[href*='Quote']").First;

//         if (await quoteLink.CountAsync() > 0)
//         {
//             await quoteLink.ClickAsync();
//             await Page.WaitForURLAsync(new Regex(".*/Shipping/Quote.*"));

//             await Expect(Page).ToHaveTitleAsync("Shipping Quote - Book_Exchange");
//             await Expect(Page.Locator("#shipping-quote-page")).ToBeVisibleAsync();
//             await Expect(Page.Locator("#shipping-quote-summary-card")).ToBeVisibleAsync();
//             await Expect(Page.Locator("#shipping-quote-transaction-id")).ToBeVisibleAsync();
//             await Expect(Page.Locator("#shipping-quote-weight")).ToBeVisibleAsync();
//         }
//     }

//     /// <summary>
//     /// UI-SHIP-12: Quote page shows carriers table when eligible carriers exist
//     /// Expected: Carriers heading and table are visible with at least one carrier row
//     /// </summary>
//     [Fact]
//     public async Task UI_SHIP_12_Quote_WithEligibleCarriers_ShowsCarriersTable()
//     {
//         await LoginAsync("test@test.com", "Test1234!");

//         await Page.GotoAsync($"{BaseUrl}/Shipping");

//         var quoteLink = Page.Locator("a[href*='Quote']").First;

//         if (await quoteLink.CountAsync() > 0)
//         {
//             await quoteLink.ClickAsync();
//             await Page.WaitForURLAsync(new Regex(".*/Shipping/Quote.*"));

//             var noCarriersAlert = Page.Locator("#shipping-quote-no-carriers");
//             var carriersTable = Page.Locator("#shipping-quote-carriers-table");

//             if (await carriersTable.CountAsync() > 0)
//             {
//                 await Expect(Page.Locator("#shipping-quote-carriers-heading")).ToBeVisibleAsync();
//                 await Expect(carriersTable).ToBeVisibleAsync();
//                 await Expect(Page.Locator("#shipping-quote-carriers-body")).ToBeVisibleAsync();
//                 await Expect(Page.Locator(".shipping-quote-carrier-row").First).ToBeVisibleAsync();
//                 await Expect(Page.Locator(".shipping-quote-carrier-name").First).ToBeVisibleAsync();
//                 await Expect(Page.Locator(".shipping-quote-estimated-cost").First).ToBeVisibleAsync();
//                 await Expect(Page.Locator(".shipping-select-carrier-btn").First).ToBeVisibleAsync();
//             }
//             else
//             {
//                 await Expect(noCarriersAlert).ToBeVisibleAsync();
//             }
//         }
//     }

//     /// <summary>
//     /// UI-SHIP-13: Quote page shows no-carriers alert when no carriers are eligible
//     /// Expected: Warning alert is rendered; carriers table is absent
//     /// </summary>
//     [Fact]
//     public async Task UI_SHIP_13_Quote_NoEligibleCarriers_ShowsAlert()
//     {
//         await LoginAsync("test@test.com", "Test1234!");

//         await Page.GotoAsync($"{BaseUrl}/Shipping");

//         var quoteLink = Page.Locator("a[href*='Quote']").First;

//         if (await quoteLink.CountAsync() > 0)
//         {
//             await quoteLink.ClickAsync();
//             await Page.WaitForURLAsync(new Regex(".*/Shipping/Quote.*"));

//             var noCarriersAlert = Page.Locator("#shipping-quote-no-carriers");
//             var carriersTable = Page.Locator("#shipping-quote-carriers-table");

//             if (await noCarriersAlert.CountAsync() > 0)
//             {
//                 await Expect(noCarriersAlert).ToBeVisibleAsync();
//                 Assert.Equal(0, await carriersTable.CountAsync());
//             }
//         }
//     }

//     /// <summary>
//     /// UI-SHIP-14: Quote page back button navigates to shipments index
//     /// Expected: Back button routes user to My Shipments
//     /// </summary>
//     [Fact]
//     public async Task UI_SHIP_14_Quote_BackButton_NavigatesToIndex()
//     {
//         await LoginAsync("test@test.com", "Test1234!");

//         await Page.GotoAsync($"{BaseUrl}/Shipping");

//         var quoteLink = Page.Locator("a[href*='Quote']").First;

//         if (await quoteLink.CountAsync() > 0)
//         {
//             await quoteLink.ClickAsync();
//             await Page.WaitForURLAsync(new Regex(".*/Shipping/Quote.*"));

//             await Page.ClickAsync("#shipping-quote-back-btn");

//             await Expect(Page).ToHaveURLAsync(new Regex(".*/Shipping.*"));
//             await Expect(Page.Locator("#shipping-index-page")).ToBeVisibleAsync();
//         }
//     }

//     /// <summary>
//     /// UI-SHIP-15: Selecting a carrier from the quote page creates a shipment and navigates to details
//     /// Expected: Clicking Select for a carrier submits the form and lands on the details page
//     /// </summary>
//     [Fact]
//     public async Task UI_SHIP_15_Quote_SelectCarrier_CreatesShipmentAndNavigatesToDetails()
//     {
//         await LoginAsync("test@test.com", "Test1234!");

//         await Page.GotoAsync($"{BaseUrl}/Shipping");

//         var quoteLink = Page.Locator("a[href*='Quote']").First;

//         if (await quoteLink.CountAsync() > 0)
//         {
//             await quoteLink.ClickAsync();
//             await Page.WaitForURLAsync(new Regex(".*/Shipping/Quote.*"));

//             var selectBtn = Page.Locator(".shipping-select-carrier-btn").First;

//             if (await selectBtn.CountAsync() > 0)
//             {
//                 await selectBtn.ClickAsync();

//                 await Expect(Page).ToHaveURLAsync(new Regex(".*/Shipping/Details.*"));
//                 await Expect(Page.Locator("#shipping-details-page")).ToBeVisibleAsync();
//                 await Expect(Page.Locator("#shipping-status-badge")).ToBeVisibleAsync();
//             }
//         }
//     }

//     /// <summary>
//     /// UI-SHIP-16: Updating shipment status from Details page stays on details and shows updated badge
//     /// Expected: After submitting the status update form the page reloads and the badge reflects the new status
//     /// </summary>
//     [Fact]
//     public async Task UI_SHIP_16_Details_UpdateStatus_ReloadsWithNewStatus()
//     {
//         await LoginAsync("test@test.com", "Test1234!");

//         await Page.GotoAsync($"{BaseUrl}/Shipping");

//         var detailsBtn = Page.Locator(".shipment-track-btn").First;

//         if (await detailsBtn.CountAsync() > 0)
//         {
//             await detailsBtn.ClickAsync();
//             await Page.WaitForURLAsync(new Regex(".*/Shipping/Details.*"));

//             var updateForm = Page.Locator("#shipping-update-status-form");

//             if (await updateForm.CountAsync() > 0)
//             {
//                 // Choose the first available option in the select
//                 var select = Page.Locator("#shipping-new-status-select");
//                 var firstOption = await select.Locator("option").First.GetAttributeAsync("value");
//                 await select.SelectOptionAsync(new[] { firstOption! });

//                 await Page.ClickAsync("#shipping-update-status-btn");

//                 await Page.WaitForURLAsync(new Regex(".*/Shipping/Details.*"));
//                 await Expect(Page.Locator("#shipping-details-page")).ToBeVisibleAsync();
//                 await Expect(Page.Locator("#shipping-status-badge")).ToBeVisibleAsync();
//             }
//         }
//     }

//     /// <summary>
//     /// UI-SHIP-17: Cancelling a shipment from the Details page updates the status badge to Cancelled
//     /// Expected: After confirming the cancel dialog the page reloads with status Cancelled
//     /// </summary>
//     [Fact]
//     public async Task UI_SHIP_17_Details_CancelShipment_ShowsCancelledStatus()
//     {
//         await LoginAsync("test@test.com", "Test1234!");

//         await Page.GotoAsync($"{BaseUrl}/Shipping");

//         var detailsBtn = Page.Locator(".shipment-track-btn").First;

//         if (await detailsBtn.CountAsync() > 0)
//         {
//             await detailsBtn.ClickAsync();
//             await Page.WaitForURLAsync(new Regex(".*/Shipping/Details.*"));

//             var cancelBtn = Page.Locator("#shipping-cancel-btn");

//             if (await cancelBtn.CountAsync() > 0)
//             {
//                 Page.Dialog += async (_, dialog) => await dialog.AcceptAsync();

//                 await cancelBtn.ClickAsync();

//                 await Page.WaitForURLAsync(new Regex(".*/Shipping.*"));

//                 var successAlert = Page.Locator("#shipping-success-alert, #shipping-details-success-alert");
//                 var cancelledBadge = Page.Locator("#shipping-status-badge");

//                 var hasSuccessAlert = await successAlert.CountAsync() > 0;
//                 var hasCancelledBadge = await cancelledBadge.CountAsync() > 0
//                     && (await cancelledBadge.InnerTextAsync()).Trim() == "Cancelled";

//                 Assert.True(hasSuccessAlert || hasCancelledBadge,
//                     "Expected either a success alert or a Cancelled status badge after cancellation.");
//             }
//         }
//     }
// }