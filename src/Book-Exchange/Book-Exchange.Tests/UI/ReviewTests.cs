// using System.Text.RegularExpressions;
// using Microsoft.Playwright;
// using Microsoft.Playwright.Xunit;

// namespace UI;

// // ── Motes ─────────────────────────────────────────────────────────────
// // Some tests require the test database to be seeded before the run.
// // UI-REVIEW-05 additionally requires a transaction that has NOT yet been reviewed
// // by User1 — re-seed or use a fresh transaction ID after each run of that test.
// // ─────────────────────────────────────────────────────────────────────────────

// public class ReviewTests : PageTest
// {
//     private const string BaseUrl = "http://localhost:5261";
//     private const string User1Email = "test@test.com";
//     private const string User1Password = "Test1234!";

//     // Replace with real seeded values before running
//     private const string CompletedTransactionId = "REPLACE-WITH-SEEDED-COMPLETED-TRANSACTION-ID";
//     private const string RevieweeUserId = "REPLACE-WITH-SEEDED-REVIEWEE-USER-ID";
//     private const string ExistingReviewId = "REPLACE-WITH-SEEDED-EXISTING-REVIEW-ID";
//     private const string IncompleteTransactionId = "REPLACE-WITH-SEEDED-INCOMPLETE-TRANSACTION-ID";
//     private const string NoReviewUserId = "REPLACE-WITH-SEEDED-NO-REVIEW-USER-ID";
//     // FreshTransactionId: a completed transaction User1 has NOT yet reviewed.
//     // Must be replaced each run once UI-REVIEW-07 has consumed it.
//     private const string FreshTransactionId = "REPLACE-WITH-FRESH-UNREVIEWD-TRANSACTION-ID";

//     // Helpers - to seed data for tests
//     private async Task LoginAsync(string email = User1Email, string password = User1Password)
//     {
//         await Page.GotoAsync($"{BaseUrl}/Identity/Account/Login");
//         await Page.FillAsync("#Input_Email", email);
//         await Page.FillAsync("#Input_Password", password);
//         await Page.ClickAsync("#login-submit");
//         await Page.WaitForURLAsync($"{BaseUrl}/");
//     }

//     private string CreateUrl(string transactionId, string revieweeId)
//         => $"{BaseUrl}/Review/Create?transactionId={transactionId}&revieweeId={revieweeId}";

//     /// <summary>
//     /// UI-REVIEW-01: Unauthenticated user is redirected to login on all Review routes
//     /// </summary>
//     [Fact]
//     public async Task UI_REVIEW_01_UnauthenticatedUser_RedirectsToLogin()
//     {
//         await Page.GotoAsync(CreateUrl(CompletedTransactionId, RevieweeUserId));
//         await Expect(Page).ToHaveURLAsync(new Regex(".*/Account/Login.*"));

//         await Page.GotoAsync($"{BaseUrl}/Review/GetUser/{RevieweeUserId}");
//         await Expect(Page).ToHaveURLAsync(new Regex(".*/Account/Login.*"));

//         await Page.GotoAsync($"{BaseUrl}/Review/Details/{ExistingReviewId}");
//         await Expect(Page).ToHaveURLAsync(new Regex(".*/Account/Login.*"));
//     }

//     /// <summary>
//     /// UI-REVIEW-02: Create page loads
//     /// </summary>
//     [Fact]
//     public async Task UI_REVIEW_02_CreatePage_RendersWireframeElements()
//     {
//         await LoginAsync();
//         await Page.GotoAsync(CreateUrl(CompletedTransactionId, RevieweeUserId));

//         await Expect(Page).ToHaveTitleAsync("Leave a Review - Book_Exchange");
//         await Expect(Page.Locator("#review-form")).ToBeVisibleAsync();
//         await Expect(Page.Locator("#review-transaction-summary")).ToBeVisibleAsync();
//         await Expect(Page.Locator(".review-txn-card")).ToBeVisibleAsync();
//         await Expect(Page.Locator(".star-rating-btn").First).ToBeVisibleAsync();
//         await Expect(Page.Locator("#review-comment-input")).ToBeVisibleAsync();
//         await Expect(Page.Locator("#submit-review-btn")).ToBeVisibleAsync();
//         await Expect(Page.Locator("#submit-review-btn")).ToBeDisabledAsync();
//         await Expect(Page.Locator("#cancel-review-btn")).ToBeVisibleAsync();
//     }

//     /// <summary>
//     /// UI-REVIEW-03: Clicking a star sets review rating and enables submit button
//     /// </summary>
//     [Fact]
//     public async Task UI_REVIEW_03_StarWidget_ClickSetsRatingAndEnablesSubmit()
//     {
//         await LoginAsync();
//         await Page.GotoAsync(CreateUrl(CompletedTransactionId, RevieweeUserId));

//         await Expect(Page.Locator("#submit-review-btn")).ToBeDisabledAsync();

//         await Page.Locator(".star-rating-btn[data-value='4']").ClickAsync();

//         await Expect(Page.Locator("#review-rating-input")).ToHaveValueAsync("4");
//         await Expect(Page.Locator("#submit-review-btn")).ToBeEnabledAsync();
//     }

//     /// <summary>
//     /// UI-REVIEW-04: Clicking a lower star after a higher one updates the rating
//     /// </summary>
//     [Fact]
//     public async Task UI_REVIEW_04_StarWidget_ClickingLowerStarUpdatesRating()
//     {
//         await LoginAsync();
//         await Page.GotoAsync(CreateUrl(CompletedTransactionId, RevieweeUserId));

//         await Page.Locator(".star-rating-btn[data-value='5']").ClickAsync();
//         await Expect(Page.Locator("#review-rating-input")).ToHaveValueAsync("5");

//         await Page.Locator(".star-rating-btn[data-value='2']").ClickAsync();
//         await Expect(Page.Locator("#review-rating-input")).ToHaveValueAsync("2");
//     }

//     /// <summary>
//     /// UI-REVIEW-05: Successful submission redirects to Transaction/Index and shows review success message
//     /// Requires: FreshTransactionId — a completed transaction User1 has NOT yet reviewed.
//     /// Replace FreshTransactionId after each run of this test.
//     /// </summary>
//     [Fact]
//     public async Task UI_REVIEW_05_SuccessfulSubmission_RedirectsAndShowsSuccessMsg()
//     {
//         await LoginAsync();
//         await Page.GotoAsync(CreateUrl(FreshTransactionId, RevieweeUserId));

//         await Page.Locator(".star-rating-btn[data-value='5']").ClickAsync();
//         await Page.FillAsync("#review-comment-input", "Great exchange, smooth process!");
//         await Page.ClickAsync("#submit-review-btn");

//         await Expect(Page).ToHaveURLAsync(new Regex(".*/Transaction.*"));
//         await Expect(Page.Locator("#review-success-msg")).ToBeVisibleAsync();
//     }

//     /// <summary>
//     /// UI-REVIEW-06: Submitting without a rating shows error message
//     /// </summary>
//     [Fact]
//     public async Task UI_REVIEW_06_SubmitWithoutRating_ShowsErrorMsg()
//     {
//         await LoginAsync();
//         await Page.GotoAsync(CreateUrl(CompletedTransactionId, RevieweeUserId));

//         await Page.EvaluateAsync(
//             "document.getElementById('submit-review-btn').removeAttribute('disabled')");
//         await Page.ClickAsync("#submit-review-btn");

//         await Expect(Page).ToHaveTitleAsync("Leave a Review - Book_Exchange");
//         await Expect(Page.Locator("#review-error-msg")).ToBeVisibleAsync();
//     }

//     /// <summary>
//     /// UI-REVIEW-07: Duplicate review shows error message containing "already reviewed"
//     /// Requires: CompletedTransactionId must be a completed transaction that User1 has already reviewed.
//     /// Replace CompletedTransactionId or use a fresh transaction ID after each run of this test.
//     /// </summary>
//     [Fact]
//     public async Task UI_REVIEW_07_DuplicateReview_ShowsErrorMsg()
//     {
//         await LoginAsync();
//         await Page.GotoAsync(CreateUrl(CompletedTransactionId, RevieweeUserId));

//         await Page.Locator(".star-rating-btn[data-value='3']").ClickAsync();
//         await Page.ClickAsync("#submit-review-btn");

//         await Expect(Page).ToHaveTitleAsync("Leave a Review - Book_Exchange");
//         await Expect(Page.Locator("#review-error-msg")).ToBeVisibleAsync();
//         await Expect(Page.Locator("#review-error-msg")).ToContainTextAsync("already reviewed");
//     }

//     /// <summary>
//     /// UI-REVIEW-08: Reviewing a non-completed transaction shows error message containing "must be completed"
//     /// </summary>
//     [Fact]
//     public async Task UI_REVIEW_08_IncompleteTransaction_ShowsErrorMsg()
//     {
//         await LoginAsync();
//         await Page.GotoAsync(CreateUrl(IncompleteTransactionId, RevieweeUserId));

//         await Page.Locator(".star-rating-btn[data-value='4']").ClickAsync();
//         await Page.ClickAsync("#submit-review-btn");

//         await Expect(Page).ToHaveTitleAsync("Leave a Review - Book_Exchange");
//         await Expect(Page.Locator("#review-error-msg")).ToBeVisibleAsync();
//         await Expect(Page.Locator("#review-error-msg")).ToContainTextAsync("must be completed");
//     }

//     /// <summary>
//     /// UI-REVIEW-09: Cancel button navigates back to Transaction index without saving
//     /// </summary>
//     [Fact]
//     public async Task UI_REVIEW_09_CancelButton_NavigatesWithoutSaving()
//     {
//         await LoginAsync();
//         await Page.GotoAsync(CreateUrl(CompletedTransactionId, RevieweeUserId));

//         await Page.ClickAsync("#cancel-review-btn");

//         await Expect(Page).ToHaveURLAsync(new Regex(".*/Transaction.*"));
//     }

//     /// <summary>
//     /// UI-REVIEW-10: GetUser page shows aggregate card and at least one review card
//     /// Requires: RevieweeUserId has at least one review seeded end-to-end
//     ///           (Listing → ExchangeRequest → Transaction → Review)
//     /// </summary>
//     [Fact]
//     public async Task UI_REVIEW_10_GetUserPage_ShowsAggregateAndReviewCards()
//     {
//         await LoginAsync();
//         await Page.GotoAsync($"{BaseUrl}/Review/GetUser/{RevieweeUserId}");

//         await Expect(Page).ToHaveTitleAsync("Reviews - Book_Exchange");
//         await Expect(Page.Locator("#review-aggregate-card")).ToBeVisibleAsync();
//         await Expect(Page.Locator("#review-average-score")).ToBeVisibleAsync();
//         await Expect(Page.Locator("#review-total-count")).ToBeVisibleAsync();
//         await Expect(Page.Locator(".review-card").First).ToBeVisibleAsync();
//     }

//     /// <summary>
//     /// UI-REVIEW-11: GetUser page shows empty state when user has no reviews
//     /// </summary>
//     [Fact]
//     public async Task UI_REVIEW_11_GetUserPage_NoReviews_ShowsEmptyState()
//     {
//         await LoginAsync();
//         await Page.GotoAsync($"{BaseUrl}/Review/GetUser/{NoReviewUserId}");

//         await Expect(Page.Locator("#review-empty-state")).ToBeVisibleAsync();
//         await Expect(Page.Locator(".review-card")).ToHaveCountAsync(0);
//     }

//     /// <summary>
//     /// UI-REVIEW-12: Average score is a valid decimal 0.00–5.00
//     /// </summary>
//     [Fact]
//     public async Task UI_REVIEW_12_GetUserPage_AverageScore_IsCorrect()
//     {
//         await LoginAsync();
//         await Page.GotoAsync($"{BaseUrl}/Review/GetUser/{RevieweeUserId}");

//         var scoreText = await Page.Locator("#review-average-score").InnerTextAsync();
//         Assert.Matches(new Regex(@"^\d(\.\d{1,2})?$"), scoreText.Trim());
//     }

//     /// <summary>
//     /// UI-REVIEW-13: Details page renders all meta fields
//     /// Requires: ExistingReviewId is seeded in the test database
//     /// </summary>
//     [Fact]
//     public async Task UI_REVIEW_13_DetailsPage_RendersAllFields()
//     {
//         await LoginAsync();
//         await Page.GotoAsync($"{BaseUrl}/Review/Details/{ExistingReviewId}");

//         await Expect(Page).ToHaveTitleAsync("Review Details - Book_Exchange");
//         await Expect(Page.Locator("#review-detail-stars")).ToBeVisibleAsync();
//         await Expect(Page.Locator("#review-detail-comment")).ToBeVisibleAsync();
//         await Expect(Page.Locator("#review-detail-reviewer")).ToBeVisibleAsync();
//         await Expect(Page.Locator("#review-detail-reviewee")).ToBeVisibleAsync();
//         await Expect(Page.Locator("#review-detail-date")).ToBeVisibleAsync();
//         await Expect(Page.Locator("#review-detail-transaction-link")).ToBeVisibleAsync();
//     }

//     /// <summary>
//     /// UI-REVIEW-14: Back link navigates to GetUser for the reviewee
//     /// </summary>
//     [Fact]
//     public async Task UI_REVIEW_14_DetailsPage_BackLink_NavigatesToGetUser()
//     {
//         await LoginAsync();
//         await Page.GotoAsync($"{BaseUrl}/Review/Details/{ExistingReviewId}");

//         await Page.ClickAsync("#review-back-link");

//         await Expect(Page).ToHaveURLAsync(new Regex(".*/Review/GetUser/.*"));
//     }

//     /// <summary>
//     /// UI-REVIEW-15: Details page for a non-existent review returns 404
//     /// </summary>
//     [Fact]
//     public async Task UI_REVIEW_15_DetailsPage_InvalidId_Returns404()
//     {
//         await LoginAsync();
//         var response = await Page.GotoAsync(
//             $"{BaseUrl}/Review/Details/{Guid.NewGuid()}");

//         Assert.Equal(404, response?.Status);
//     }
// }