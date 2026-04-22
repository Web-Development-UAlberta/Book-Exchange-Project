# Test Plan – Book Exchange Platform

## 1. Purpose

This document defines a simple test plan for the Book Exchange Platform.  
It covers:

- Unit testing
- Integration testing
- Manual UI testing

The goal is to verify that the core system works correctly for the main business flows such as authentication, profile management, book listing, wishlist management, search, messaging/notifications, and transactions.

---

## 2. Scope

This test plan covers the following functional areas:

- User registration and login
- User profile management
- Book management
- Listings management
- Wishlist management
- Search and filtering
- Match suggestions
- Transactions (buy / sell / swap)
- Messaging
- Notifications
- Basic authorization and validation
- Database integration

This plan focuses on simple and practical scenarios suitable for the current stage of the project.

---

## 3. Test Objectives

The objectives are:

- Verify that business logic behaves as expected
- Verify that application layers work correctly together
- Verify that UI pages and forms work for normal user actions
- Catch validation, navigation, and integration issues early
- Confirm that main user workflows can be completed successfully

---

## 4. Test Types

## 4.1 Unit Testing

Unit tests will validate isolated business logic and helper/service methods.

Examples:
- Validation rules
- Matching logic
- Transaction state changes
- Notification trigger conditions
- Search/filter logic
- Mapping and calculation methods

## 4.2 Integration Testing

Integration tests will validate that multiple parts of the system work together.

Examples:
- Controller → Service → Database flow
- Authentication and authorization with protected routes
- Creating records in PostgreSQL and retrieving them
- End-to-end request handling for main features

## 4.3 Manual UI Testing

Manual UI testing will verify:

- Page rendering
- Navigation
- Form submission
- Validation messages
- Buttons, links, and workflow usability
- Role-based access behavior

---

## 5. Assumptions

- The system is built with ASP.NET Core MVC
- PostgreSQL is used as the database
- Authentication is handled through ASP.NET Identity
- Basic CRUD pages exist for core entities
- Testing will focus on the most important user-facing features first

---

## 6. Test Environment

## 6.1 Unit Test Environment

- .NET test project
- xUnit or NUnit
- Mocking library
- In-memory objects / mocked dependencies

## 6.2 Integration Test Environment

- Test database or isolated PostgreSQL database
- ASP.NET Core test host
- Seeded sample test data
- Clean database state for each test run where possible

## 6.3 Manual UI Test Environment

- Local development environment
- Browser: Chrome / Edge
- Running application with test data
- Test user accounts available

---

## 7. Entry and Exit Criteria

## 7.1 Entry Criteria

Testing can begin when:

- Core features are implemented
- Application builds successfully
- Database migrations are applied
- Test environment is available

## 7.2 Exit Criteria

Testing for a release candidate is complete when:

- Critical flows pass
- No blocker defects remain
- High-priority defects are fixed or accepted
- Main unit and integration tests pass

---

## 8. Unit Test Scenarios

## 8.1 Authentication / User Validation

### UT-AUTH-01
**Scenario:** Register user with valid data  
**Expected Result:** Validation passes and user object/service accepts request

### UT-AUTH-02
**Scenario:** Register user with missing required fields  
**Expected Result:** Validation fails

### UT-AUTH-03
**Scenario:** Register user with duplicate email/username  
**Expected Result:** Validation/service rejects duplicate

---

## 8.2 Book Management

### UT-BOOK-01
**Scenario:** Create a book with valid title, author, and identifiers  
**Expected Result:** Book model/service accepts input

### UT-BOOK-02
**Scenario:** Create a book with missing required title  
**Expected Result:** Validation fails

### UT-BOOK-03
**Scenario:** Reject duplicate ISBN when uniqueness is required  
**Expected Result:** Validation/service rejects duplicate

### UT-BOOK-04
**Scenario:** Update book with valid data  
**Expected Result:** Book fields (title, author, ISBN, published date) are updated successfully

### UT-BOOK-05
**Scenario:** Update book with missing required fields (e.g. empty title)  
**Expected Result:** Validation fails and update is rejected

### UT-BOOK-06
**Scenario:** Delete existing book with no dependencies  
**Expected Result:** Book is deleted successfully

### UT-BOOK-07
**Scenario:** Delete book that is linked to listings or transactions  
**Expected Result:** 
- Either deletion is prevented  
**OR**
- Related records are handled according to business rules (e.g. cascade delete or restriction)

---

## 8.3 Listing Management

### UT-LIST-01
**Scenario:** Create listing with valid owner, book, type, and condition  
**Expected Result:** Listing is created successfully

### UT-LIST-02
**Scenario:** Create listing without book reference  
**Expected Result:** Validation fails

### UT-LIST-03
**Scenario:** Mark listing as unavailable after completed transaction  
**Expected Result:** Listing status updates correctly

### UT-LIST-04
**Scenario:** Update listing with valid new values  
**Expected Result:** Listing fields are updated successfully

### UT-LIST-05
**Scenario:** Update listing with invalid data (e.g. missing required title or invalid price if applicable)  
**Expected Result:** Validation fails and update is rejected

### UT-LIST-06
**Scenario:** Update listing status from available to unavailable  
**Expected Result:** Status change is saved correctly

### UT-LIST-07
**Scenario:** Delete existing listing  
**Expected Result:** Listing is removed successfully or marked deleted based on design

### UT-LIST-08
**Scenario:** Delete listing that does not exist  
**Expected Result:** System returns proper failure result without crashing

### UT-LIST-09
**Scenario:** User attempts to update another user's listing  
**Expected Result:** Operation is rejected by authorization/business rule

### UT-LIST-10
**Scenario:** User attempts to delete another user's listing  
**Expected Result:** Operation is rejected by authorization/business rule

---

## 8.4 Wishlist Management

### UT-WISH-01
**Scenario:** Add a book to wishlist  
**Expected Result:** Wishlist item is added

### UT-WISH-02
**Scenario:** Add duplicate wishlist item  
**Expected Result:** Duplicate is prevented or handled according to business rule

### UT-WISH-03
**Scenario:** Remove item from wishlist  
**Expected Result:** Item is removed successfully

### UT-WISH-04
**Scenario:** Update wishlist item preferences or notes if supported  
**Expected Result:** Wishlist item is updated successfully

### UT-WISH-05
**Scenario:** Update wishlist item that does not exist  
**Expected Result:** System returns proper failure result

### UT-WISH-06
**Scenario:** Delete wishlist item  
**Expected Result:** Item is removed successfully

### UT-WISH-07
**Scenario:** Delete wishlist item that does not exist  
**Expected Result:** System handles request safely without crashing

### UT-WISH-08
**Scenario:** User attempts to delete another user's wishlist item  
**Expected Result:** Operation is rejected

### UT-WISH-09
**Scenario:** User attempts to update another user's wishlist item  
**Expected Result:** Operation is rejected

---

## 8.5 Search and Filtering

### UT-SEARCH-01
**Scenario:** Filter books by title  
**Expected Result:** Only matching books are returned

### UT-SEARCH-02
**Scenario:** Filter listings by genre  
**Expected Result:** Only matching listings are returned

### UT-SEARCH-03
**Scenario:** Filter listings by transaction type (buy/sell/swap)  
**Expected Result:** Only listings of selected type are returned

---

## 8.6 Match Logic

### UT-MATCH-01
**Scenario:** User wishlist matches another user's available listing  
**Expected Result:** Match is returned

### UT-MATCH-02
**Scenario:** No matching listing exists  
**Expected Result:** Empty result returned

### UT-MATCH-03
**Scenario:** Exclude user's own listing from match results  
**Expected Result:** Own listings are not suggested

---

## 8.7 Transactions

### UT-TRANS-01
**Scenario:** Create buy/sell transaction from valid listing  
**Expected Result:** Transaction is created with correct initial status

### UT-TRANS-02
**Scenario:** Create swap transaction between two valid listings  
**Expected Result:** Transaction is created correctly

### UT-TRANS-03
**Scenario:** Complete transaction  
**Expected Result:** Transaction status changes to completed

### UT-TRANS-04
**Scenario:** Cancel transaction  
**Expected Result:** Status changes to cancelled

### UT-TRANS-05
**Scenario:** Invalid status transition  
**Expected Result:** System rejects invalid transition

---

## 8.8 Reviews / Ratings

### UT-REVIEW-01
**Scenario:** Create review with valid reviewer and target user  
**Expected Result:** Review is accepted

### UT-REVIEW-02
**Scenario:** Rating outside allowed range  
**Expected Result:** Validation fails

### UT-REVIEW-03
**Scenario:** Aggregate user rating from multiple reviews  
**Expected Result:** Average rating is calculated correctly

---

## 8.9 Notifications

### UT-NOTIF-01
**Scenario:** Trigger notification when a match is found  
**Expected Result:** Notification record is created

### UT-NOTIF-02
**Scenario:** Trigger notification when message is received  
**Expected Result:** Notification is created

### UT-NOTIF-03
**Scenario:** Mark notification as read  
**Expected Result:** Read flag updates successfully

---

## 9. Integration Test Scenarios

## 9.1 Authentication Integration

### IT-AUTH-01
**Scenario:** User registers through UI/request pipeline  
**Steps:**
1. Submit registration form
2. Save user in database
3. Redirect to expected page  
**Expected Result:** User record exists and registration flow succeeds

### IT-AUTH-02
**Scenario:** Valid login  
**Expected Result:** User is authenticated and redirected appropriately

### IT-AUTH-03
**Scenario:** Access protected page without login  
**Expected Result:** User is redirected to login page

---

## 9.2 Book and Listing Integration

### IT-LIST-01
**Scenario:** Authenticated user creates a listing  
**Steps:**
1. Login
2. Submit listing form
3. Save listing to database  
**Expected Result:** Listing appears in My Listings and database

### IT-LIST-02
**Scenario:** Edit existing listing  
**Expected Result:** Updated values are saved and displayed

### IT-LIST-03
**Scenario:** Delete listing  
**Expected Result:** Listing is removed or marked deleted according to design

### IT-LIST-04
**Scenario:** Authenticated user updates own listing  
**Steps:**
1. Login as listing owner
2. Open edit listing page
3. Submit valid updated values
4. Read listing from database  

**Expected Result:** Updated values are stored and shown in UI

### IT-LIST-05
**Scenario:** Authenticated user submits invalid listing update  
**Expected Result:** Validation errors are shown and database is unchanged

### IT-LIST-06
**Scenario:** Authenticated user deletes own listing  
**Expected Result:** Listing is removed or marked inactive in database and hidden from active results

### IT-LIST-07
**Scenario:** User tries to edit another user's listing  
**Expected Result:** Access is denied or operation is blocked

### IT-LIST-08
**Scenario:** User tries to delete another user's listing  
**Expected Result:** Access is denied or operation is blocked

---

## 9.3 Wishlist Integration

### IT-WISH-01
**Scenario:** User adds a listing/book to wishlist  
**Expected Result:** Wishlist item is saved and shown in wishlist page

### IT-WISH-02
**Scenario:** Remove wishlist item  
**Expected Result:** Item no longer appears in wishlist

### IT-WISH-03
**Scenario:** User updates own wishlist item  
**Expected Result:** Wishlist item changes are saved correctly

### IT-WISH-04
**Scenario:** User deletes own wishlist item  
**Expected Result:** Wishlist item is removed from database and UI

### IT-WISH-05
**Scenario:** User tries to update another user's wishlist item  
**Expected Result:** Access is denied or operation is blocked

### IT-WISH-06
**Scenario:** User tries to delete another user's wishlist item  
**Expected Result:** Access is denied or operation is blocked

---

## 9.4 Search Integration

### IT-SEARCH-01
**Scenario:** Search books by keyword  
**Expected Result:** Matching results are returned from database

### IT-SEARCH-02
**Scenario:** Apply multiple filters  
**Expected Result:** Combined filtering works correctly

---

## 9.5 Transaction Integration

### IT-TRANS-01
**Scenario:** User initiates purchase request  
**Expected Result:** Transaction record is created and linked correctly

### IT-TRANS-02
**Scenario:** User initiates swap request  
**Expected Result:** Swap transaction is created correctly

### IT-TRANS-03
**Scenario:** Completing transaction updates related listing(s)  
**Expected Result:** Transaction status and listing availability are updated

---

## 9.6 Messaging Integration

### IT-MSG-01
**Scenario:** Send message between users  
**Expected Result:** Message is stored and visible in conversation thread

### IT-MSG-02
**Scenario:** Receive notification for new message  
**Expected Result:** Related notification record appears

---

## 9.7 Review Integration

### IT-REVIEW-01
**Scenario:** User submits review after transaction  
**Expected Result:** Review is saved and linked to user/transaction

### IT-REVIEW-02
**Scenario:** User profile displays updated rating  
**Expected Result:** Aggregate rating is updated on profile page

---

## 10. Manual UI Test Scenarios

## 10.1 General Navigation

### MUI-01
**Scenario:** Open home page  
**Expected Result:** Page loads without error and main navigation is visible

### MUI-02
**Scenario:** Navigate through major pages  
**Pages:** Home, Login, Register, Listings, Wishlist, Profile, Messages, Notifications  
**Expected Result:** All links work correctly

---

## 10.2 Registration and Login

### MUI-03
**Scenario:** Register with valid data  
**Expected Result:** Account is created successfully

### MUI-04
**Scenario:** Register with invalid data  
**Expected Result:** Proper validation messages are shown

### MUI-05
**Scenario:** Login with valid credentials  
**Expected Result:** Login succeeds

### MUI-06
**Scenario:** Login with invalid credentials  
**Expected Result:** Error message is shown

### MUI-07
**Scenario:** Logout  
**Expected Result:** Session ends and protected pages are inaccessible

---

## 10.3 Profile UI

### MUI-08
**Scenario:** View profile  
**Expected Result:** User details display correctly

### MUI-09
**Scenario:** Edit profile  
**Expected Result:** Changes are saved and visible

---

## 10.4 Listings UI

### MUI-10
**Scenario:** Create new listing through form  
**Expected Result:** Listing is created and shown in user dashboard/listing page

### MUI-11
**Scenario:** Submit listing form with missing required fields  
**Expected Result:** Validation messages appear near fields

### MUI-12
**Scenario:** Edit listing from My Listings  
**Expected Result:** Updated data is shown after save

### MUI-13
**Scenario:** Delete listing  
**Expected Result:** Listing is removed from visible list or marked unavailable

---

## 10.5 Wishlist UI

### MUI-14
**Scenario:** Add item to wishlist  
**Expected Result:** Item appears in wishlist page

### MUI-15
**Scenario:** Remove item from wishlist  
**Expected Result:** Item disappears from wishlist page

---

## 10.6 Search UI

### MUI-16
**Scenario:** Search by title/keyword  
**Expected Result:** Search results match input

### MUI-17
**Scenario:** Use filters such as genre, condition, or listing type  
**Expected Result:** Results update correctly

### MUI-18
**Scenario:** Search with no results  
**Expected Result:** Friendly “no results found” message is shown

---

## 10.7 Transaction UI

### MUI-19
**Scenario:** Start buy request  
**Expected Result:** Request is submitted and confirmation is shown

### MUI-20
**Scenario:** Start swap request  
**Expected Result:** Swap flow works and request is recorded

### MUI-21
**Scenario:** View transaction history  
**Expected Result:** Past and current transactions are displayed correctly

---

## 10.8 Messaging and Notifications UI

### MUI-22
**Scenario:** Open message thread and send message  
**Expected Result:** Message appears in conversation

### MUI-23
**Scenario:** Open notifications page  
**Expected Result:** Notifications are visible and readable

### MUI-24
**Scenario:** Mark notification as read  
**Expected Result:** Notification state changes visually

---

## 10.9 Authorization UI

### MUI-25
**Scenario:** Unauthenticated user tries to access protected page  
**Expected Result:** Redirect to login page

### MUI-26
**Scenario:** User tries to edit another user's content  
**Expected Result:** Access denied or redirect occurs

---

## 10.10 Additional Manual UI Test Scenarios for Listings

### MUI-27
**Scenario:** Edit own listing through UI  
**Expected Result:** Updated values appear correctly after saving

### MUI-28
**Scenario:** Edit listing with missing required fields  
**Expected Result:** Validation messages are displayed and changes are not saved

### MUI-29
**Scenario:** Delete own listing from My Listings page  
**Expected Result:** Listing disappears from active listing page or shows deleted/unavailable state

### MUI-30
**Scenario:** Try to open edit page for another user's listing by URL  
**Expected Result:** Access denied, redirect, or error page shown properly

### MUI-31
**Scenario:** Try to delete another user's listing by URL or action manipulation  
**Expected Result:** Operation is blocked

---

## 10.11 Additional Manual UI Test Scenarios for Wishlist

### MUI-32
**Scenario:** Update wishlist item from wishlist page  
**Expected Result:** Changes are saved and displayed correctly

### MUI-33
**Scenario:** Delete wishlist item from wishlist page  
**Expected Result:** Item is removed from the list immediately or after refresh

### MUI-34
**Scenario:** Delete wishlist item twice  
**Expected Result:** System handles gracefully without crashing

### MUI-35
**Scenario:** Try to access another user's wishlist item edit page  
**Expected Result:** Access denied or redirect occurs

### MUI-36
**Scenario:** Try to delete another user's wishlist item through direct URL or manipulated request  
**Expected Result:** Operation is blocked

---

## 11. Non-Functional Checks

These are simple checks for now:

### NFT-01 Performance
Open common pages and confirm they load within acceptable time in local environment

### NFT-02 Error Handling
Trigger invalid inputs and verify user-friendly messages appear

### NFT-03 Data Integrity
Verify related records remain consistent after create/update/delete actions

### NFT-04 Basic Usability
Check labels, buttons, forms, and navigation for clarity

---

## 12. Test Data

Suggested sample data:

- 3 test users
- 5–10 books
- Multiple listing types:
  - Buy
  - Sell
  - Swap
- Wishlist entries
- Sample messages
- Sample transactions in different states:
  - Pending
  - Accepted
  - Completed
  - Cancelled

---

## 13. Risks

- Requirements may still evolve
- Match logic may change later
- UI pages may be incomplete during early testing
- Some features may depend on final database schema decisions

---

## 14. Recommended Initial Priority

Start with these first:

1. Registration and login
2. Create/edit/delete listing
3. Wishlist add/remove
4. Search and filtering
5. Buy/sell/swap transaction creation
6. Basic messaging
7. Notifications
8. Review/rating logic

---

## 15. Deliverables

Testing deliverables may include:

- Unit test project
- Integration test project
- Manual UI checklist
- Bug list / defect log
- Test summary report

---

## 16. Conclusion

This test plan provides a simple and practical approach for validating the Book Exchange Platform during development.  
It focuses on core functionality first and can be expanded later as the project grows.