# Test Plan – Book Exchange Platform

## 1. Purpose

This document defines a simple test plan for the Book Exchange Platform.  
It covers:

- Unit testing
- Integration testing
- Manual UI testing

The goal is to verify that the core system works correctly for the main business flows such as authentication, profile management, ISBN-based listings, wishlist management, matching, exchange requests, transactions, shipping, messaging, notifications, and reviews.

---

## 2. Scope

This test plan covers:

- User registration and login
- User profile and address management
- ISBN-based listing management
- Genre assignment to listings
- Wishlist management
- Search and filtering
- Match suggestions
- Exchange requests:
  - Buy/sell
  - Book swap
  - Book + money swap
- Transactions created from accepted exchange requests
- Shipping cost calculation
- Messaging
- Notifications
- Reviews
- Authorization and validation
- Database integration

This plan focuses on simple and practical scenarios suitable for the current phase of the project.

---

## 3. Test Objectives

The objectives are:

- Verify that business logic behaves as expected
- Verify that application layers work correctly together
- Verify that UI pages and forms support normal user workflows
- Catch validation, navigation, and integration issues early
- Confirm that the main user flows can be completed successfully
- Confirm that simplified ERD rules are correctly enforced

---

## 4. Test Types

### 4.1 Unit Testing

Unit tests will validate isolated business logic and helper/service methods.

Examples:

- ISBN validation
- Listing validation
- Wishlist duplicate prevention
- Matching logic
- Exchange request validation
- Transaction state changes
- Shipping cost calculation
- Notification trigger conditions
- Search/filter logic

---

### 4.2 Integration Testing

Integration tests will validate that multiple parts of the system work together.

Examples:
- Controller → Service → Database flow
- Authentication and authorization with protected routes
- Creating records in PostgreSQL and retrieving them
- Creating exchange requests and transactions
- Creating shipment records from transactions

---

### 4.3 Manual UI Testing

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
- Listings use a single ISBN field
- Book metadata is retrieved from an external API or mocked service
- Addresses use Google Place ID
- Testing will focus on the most important user-facing features first

---

## 6. Test Environment

### 6.1 Unit Test Environment

- .NET test project
- xUnit
- Mocking library
- In-memory objects / mocked dependencies

---

### 6.2 Integration Test Environment

- Test PostgreSQL database
- ASP.NET Core test host
- Seeded sample data
- Clean database state for each test run where possible

---

### 6.3 Manual UI Test Environment

- Local development environment
- Browser: Chrome / Edge
- Running application with test data
- Test user accounts available

---

## 7. Entry and Exit Criteria

### 7.1 Entry Criteria

Testing can begin when:

- Core features are implemented
- Application builds successfully
- Database migrations are applied
- Test environment is available

---

### 7.2 Exit Criteria

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
**Expected Result:** User is accepted and can be created

### UT-AUTH-02
**Scenario:** Register user with missing required fields  
**Expected Result:** Validation fails

### UT-AUTH-03
**Scenario:** Register user with duplicate email/username  
**Expected Result:** Duplicate registration is rejected

---

## 8.2 Address Management

### UT-ADDR-01
**Scenario:** Create address with valid GooglePlaceId  
**Expected Result:** Address is accepted

### UT-ADDR-02
**Scenario:** Create address without GooglePlaceId  
**Expected Result:** Validation fails

### UT-ADDR-03
**Scenario:** User updates own address  
**Expected Result:** Address is updated successfully

### UT-ADDR-04
**Scenario:** User attempts to update another user's address  
**Expected Result:** Operation is rejected

---

## 8.3 Listing Management

### UT-LIST-01
**Scenario:** Create listing with valid ISBN, condition, price, and weight  
**Expected Result:** Listing is created successfully

### UT-LIST-02
**Scenario:** Create listing with invalid ISBN format  
**Expected Result:** Validation fails

### UT-LIST-03
**Scenario:** Create listing with negative price  
**Expected Result:** Validation fails

### UT-LIST-04
**Scenario:** Create listing with zero or negative weight  
**Expected Result:** Validation fails

### UT-LIST-05
**Scenario:** Update listing with valid new values  
**Expected Result:** Listing is updated successfully

### UT-LIST-06
**Scenario:** Update listing status from Active to Pending  
**Expected Result:** Status change is saved correctly

### UT-LIST-07
**Scenario:** Mark listing as Completed after successful transaction  
**Expected Result:** Listing is no longer shown as active

### UT-LIST-08
**Scenario:** Delete existing listing  
**Expected Result:** Listing is removed or marked unavailable based on implementation

### UT-LIST-09
**Scenario:** User attempts to update another user's listing  
**Expected Result:** Operation is rejected

### UT-LIST-10
**Scenario:** User attempts to delete another user's listing  
**Expected Result:** Operation is rejected

---

## 8.4 Genre Management

### UT-GENRE-01
**Scenario:** Create genre with valid name  
**Expected Result:** Genre is created successfully

### UT-GENRE-02
**Scenario:** Create duplicate genre  
**Expected Result:** Duplicate is rejected

### UT-GENRE-03
**Scenario:** Assign genre to listing  
**Expected Result:** ListingGenres record is created

### UT-GENRE-04
**Scenario:** Assign same genre to same listing twice  
**Expected Result:** Duplicate relationship is prevented

---

## 8.5 Wishlist Management

### UT-WISH-01
**Scenario:** Add valid ISBN to wishlist  
**Expected Result:** Wishlist item is added

### UT-WISH-02
**Scenario:** Add invalid ISBN to wishlist  
**Expected Result:** Validation fails

### UT-WISH-03
**Scenario:** Add duplicate ISBN to same user's wishlist  
**Expected Result:** Duplicate is prevented

### UT-WISH-04
**Scenario:** Remove item from wishlist  
**Expected Result:** Item is removed successfully

### UT-WISH-05
**Scenario:** Toggle wishlist item active/inactive  
**Expected Result:** IsActive updates correctly

### UT-WISH-06
**Scenario:** User attempts to update another user's wishlist item  
**Expected Result:** Operation is rejected

### UT-WISH-07
**Scenario:** User attempts to delete another user's wishlist item  
**Expected Result:** Operation is rejected

---

## 8.6 Search and Filtering

### UT-SEARCH-01
**Scenario:** Search listings by ISBN  
**Expected Result:** Matching listings are returned

### UT-SEARCH-02
**Scenario:** Filter listings by genre  
**Expected Result:** Only listings with selected genre are returned

### UT-SEARCH-03
**Scenario:** Filter listings by condition  
**Expected Result:** Only listings with selected condition are returned

### UT-SEARCH-04
**Scenario:** Filter listings by status Active  
**Expected Result:** Only active listings are returned

---

## 8.7 Match Logic

### UT-MATCH-01
**Scenario:** User wishlist ISBN matches another user's active listing ISBN  
**Expected Result:** Match is returned

### UT-MATCH-02
**Scenario:** No matching listing exists  
**Expected Result:** Empty result returned

### UT-MATCH-03
**Scenario:** Exclude user's own listing from match results  
**Expected Result:** User's own listings are not suggested

### UT-MATCH-04
**Scenario:** Inactive wishlist item matches a listing  
**Expected Result:** Match is not returned

### UT-MATCH-05
**Scenario:** Completed or cancelled listing matches wishlist  
**Expected Result:** Match is not returned

---

## 8.8 Exchange Requests

### UT-EXCH-01
**Scenario:** Create BuySell exchange request for active listing  
**Expected Result:** ExchangeRequest is created with Requested status

### UT-EXCH-02
**Scenario:** Create BookSwap exchange request with one offered listing  
**Expected Result:** ExchangeRequest and ExchangeRequestItems are created

### UT-EXCH-03
**Scenario:** Create BookSwapWithCash request with offered listing and price  
**Expected Result:** ExchangeRequest is created successfully

### UT-EXCH-04
**Scenario:** Create swap request with more than three offered listings  
**Expected Result:** Request is rejected by business rule

### UT-EXCH-05
**Scenario:** Create exchange request for own listing  
**Expected Result:** Request is rejected

### UT-EXCH-06
**Scenario:** Accept valid exchange request  
**Expected Result:** ExchangeRequest status becomes Accepted

### UT-EXCH-07
**Scenario:** Reject exchange request  
**Expected Result:** ExchangeRequest status becomes Rejected

### UT-EXCH-08
**Scenario:** Invalid exchange status transition  
**Expected Result:** System rejects invalid transition

---

## 8.9 Transactions

### UT-TRANS-01
**Scenario:** Create transaction from accepted exchange request  
**Expected Result:** Transaction is created with Confirmed status

### UT-TRANS-02
**Scenario:** Attempt to create transaction from rejected request  
**Expected Result:** Transaction creation is rejected

### UT-TRANS-03
**Scenario:** Complete transaction  
**Expected Result:** Transaction status changes to Completed

### UT-TRANS-04
**Scenario:** Cancel transaction  
**Expected Result:** Transaction status changes to Cancelled

### UT-TRANS-05
**Scenario:** Invalid status transition  
**Expected Result:** System rejects invalid transition

---

## 8.10 Shipping

### UT-SHIP-01
**Scenario:** Calculate shipping cost with valid carrier, weight, and distance  
**Expected Result:** Shipping cost is calculated correctly

### UT-SHIP-02
**Scenario:** Shipping weight exceeds carrier max weight  
**Expected Result:** Carrier is rejected or unavailable

### UT-SHIP-03
**Scenario:** Missing sender or receiver address  
**Expected Result:** Shipment creation fails

### UT-SHIP-04
**Scenario:** Distance API returns valid distance  
**Expected Result:** DistanceKm is stored and shipping cost is calculated

### UT-SHIP-05
**Scenario:** Distance API fails  
**Expected Result:** System handles failure gracefully

---

## 8.11 Reviews / Ratings

### UT-REVIEW-01
**Scenario:** Create review for completed transaction  
**Expected Result:** Review is accepted

### UT-REVIEW-02
**Scenario:** Create review before transaction completion  
**Expected Result:** Review is rejected

### UT-REVIEW-03
**Scenario:** Rating outside allowed range  
**Expected Result:** Validation fails

### UT-REVIEW-04
**Scenario:** Duplicate review for same transaction by same reviewer  
**Expected Result:** Duplicate is rejected

### UT-REVIEW-05
**Scenario:** Aggregate user rating from multiple reviews  
**Expected Result:** Average rating is calculated correctly

---

## 8.12 Notifications

### UT-NOTIF-01
**Scenario:** Trigger notification when a match is found  
**Expected Result:** Notification record is created

### UT-NOTIF-02
**Scenario:** Trigger notification when message is received  
**Expected Result:** Notification is created

### UT-NOTIF-03
**Scenario:** Trigger notification when exchange request is accepted  
**Expected Result:** Notification is created

### UT-NOTIF-04
**Scenario:** Mark notification as read  
**Expected Result:** Status changes to Read and ReadAt is populated

---

## 9. Integration Test Scenarios

## 9.1 Authentication Integration

### IT-AUTH-01
**Scenario:** User registers through request pipeline  
**Expected Result:** User record exists and registration flow succeeds

### IT-AUTH-02
**Scenario:** Valid login  
**Expected Result:** User is authenticated and redirected appropriately

### IT-AUTH-03
**Scenario:** Access protected page without login  
**Expected Result:** User is redirected to login page

---

## 9.2 Listing Integration

### IT-LIST-01
**Scenario:** Authenticated user creates a listing  
**Expected Result:** Listing appears in My Listings and database

### IT-LIST-02
**Scenario:** User creates listing with invalid ISBN  
**Expected Result:** Validation error is shown and database is unchanged

### IT-LIST-03
**Scenario:** Edit existing listing  
**Expected Result:** Updated values are saved and displayed

### IT-LIST-04
**Scenario:** Delete listing  
**Expected Result:** Listing is removed or marked unavailable

### IT-LIST-05
**Scenario:** User tries to edit another user's listing  
**Expected Result:** Access is denied or operation is blocked

### IT-LIST-06
**Scenario:** User assigns genres to listing  
**Expected Result:** ListingGenres records are saved correctly

---

## 9.3 Wishlist Integration

### IT-WISH-01
**Scenario:** User adds ISBN to wishlist  
**Expected Result:** Wishlist item is saved and shown

### IT-WISH-02
**Scenario:** User adds duplicate ISBN  
**Expected Result:** Duplicate is blocked

### IT-WISH-03
**Scenario:** Remove wishlist item  
**Expected Result:** Item no longer appears

### IT-WISH-04
**Scenario:** User tries to update another user's wishlist item  
**Expected Result:** Access is denied or operation is blocked

---

## 9.4 Matching Integration

### IT-MATCH-01
**Scenario:** User wishlist matches another user's active listing  
**Expected Result:** Match suggestion appears

### IT-MATCH-02
**Scenario:** Matching listing belongs to same user  
**Expected Result:** Match is excluded

### IT-MATCH-03
**Scenario:** Listing status is Completed  
**Expected Result:** Match is excluded

---

## 9.5 Exchange Request Integration

### IT-EXCH-01
**Scenario:** User creates BuySell exchange request  
**Expected Result:** ExchangeRequest is saved with Requested status

### IT-EXCH-02
**Scenario:** User creates BookSwap exchange request  
**Expected Result:** ExchangeRequest and ExchangeRequestItems are saved

### IT-EXCH-03
**Scenario:** User creates BookSwapWithCash exchange request  
**Expected Result:** Request includes offered listing(s) and Price

### IT-EXCH-04
**Scenario:** Listing owner accepts exchange request  
**Expected Result:** ExchangeRequest status becomes Accepted and Transaction is created

### IT-EXCH-05
**Scenario:** Listing owner rejects exchange request  
**Expected Result:** ExchangeRequest status becomes Rejected and no Transaction is created

---

## 9.6 Transaction Integration

### IT-TRANS-01
**Scenario:** Accepted exchange creates transaction  
**Expected Result:** Transaction is linked to ExchangeRequest

### IT-TRANS-02
**Scenario:** Completing transaction updates listing status  
**Expected Result:** Related listing(s) are no longer active

### IT-TRANS-03
**Scenario:** Transaction history is requested  
**Expected Result:** User sees relevant transactions

---

## 9.7 Shipping Integration

### IT-SHIP-01
**Scenario:** Create shipment for transaction  
**Expected Result:** Shipment is saved and linked to transaction

### IT-SHIP-02
**Scenario:** Quote shipment using carrier pricing and distance  
**Expected Result:** DistanceKm and ShippingCost are calculated

### IT-SHIP-03
**Scenario:** Update shipment status  
**Expected Result:** Shipment status updates correctly

---

## 9.8 Messaging Integration

### IT-MSG-01
**Scenario:** Send message between users  
**Expected Result:** Message is stored and visible in conversation thread

### IT-MSG-02
**Scenario:** Receive notification for new message  
**Expected Result:** Related notification record appears

---

## 9.9 Review Integration

### IT-REVIEW-01
**Scenario:** User submits review after completed transaction  
**Expected Result:** Review is saved and linked to transaction

### IT-REVIEW-02
**Scenario:** User profile displays updated rating  
**Expected Result:** Aggregate rating is updated on profile page

---

## 10. Manual UI Test Scenarios

## 10.1 General Navigation

### MUI-01
**Scenario:** Open home page  
**Expected Result:** Page loads without error and navigation is visible

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
**Expected Result:** Validation messages are shown

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

## 10.3 Profile and Address UI

### MUI-08
**Scenario:** View profile  
**Expected Result:** User details display correctly

### MUI-09
**Scenario:** Edit profile  
**Expected Result:** Changes are saved and visible

### MUI-10
**Scenario:** Add address using Google Place ID / address lookup  
**Expected Result:** Address is saved successfully

### MUI-11
**Scenario:** Add address without valid location  
**Expected Result:** Validation message is shown

---

## 10.4 Listings UI

### MUI-12
**Scenario:** Create new listing through form  
**Expected Result:** Listing is created and shown

### MUI-13
**Scenario:** Submit listing form with invalid ISBN  
**Expected Result:** Validation message appears

### MUI-14
**Scenario:** Submit listing with negative price  
**Expected Result:** Validation message appears

### MUI-15
**Scenario:** Edit own listing  
**Expected Result:** Updated values appear after saving

### MUI-16
**Scenario:** Delete own listing  
**Expected Result:** Listing disappears or becomes unavailable

### MUI-17
**Scenario:** Try to edit another user's listing by URL  
**Expected Result:** Access denied or redirect

---

## 10.5 Wishlist UI

### MUI-18
**Scenario:** Add ISBN to wishlist  
**Expected Result:** Item appears in wishlist page

### MUI-19
**Scenario:** Add duplicate ISBN to wishlist  
**Expected Result:** Duplicate is blocked

### MUI-20
**Scenario:** Remove wishlist item  
**Expected Result:** Item disappears from wishlist page

### MUI-21
**Scenario:** Toggle wishlist item active/inactive  
**Expected Result:** Status updates visually

---

## 10.6 Search UI

### MUI-22
**Scenario:** Search by ISBN  
**Expected Result:** Matching listings are shown

### MUI-23
**Scenario:** Use filters such as genre or condition  
**Expected Result:** Results update correctly

### MUI-24
**Scenario:** Search with no results  
**Expected Result:** Friendly “no results found” message is shown

---

## 10.7 Matching and Exchange UI

### MUI-25
**Scenario:** View match suggestions  
**Expected Result:** Matching listings are displayed

### MUI-26
**Scenario:** Start buy/sell request  
**Expected Result:** Exchange request is submitted

### MUI-27
**Scenario:** Start book swap request  
**Expected Result:** User can select up to 3 offered books

### MUI-28
**Scenario:** Start book + money swap request  
**Expected Result:** User can select books and enter extra Price

### MUI-29
**Scenario:** Attempt to offer more than 3 books  
**Expected Result:** UI blocks or validation message appears

### MUI-30
**Scenario:** Listing owner accepts request  
**Expected Result:** Transaction is created

### MUI-31
**Scenario:** Listing owner rejects request  
**Expected Result:** Request status becomes rejected

---

## 10.8 Transaction and Shipping UI

### MUI-32
**Scenario:** View transaction history  
**Expected Result:** Past and current transactions are displayed

### MUI-33
**Scenario:** Create shipment for transaction  
**Expected Result:** Shipment form saves correctly

### MUI-34
**Scenario:** Quote shipping cost  
**Expected Result:** Shipping cost displays correctly

### MUI-35
**Scenario:** Update shipment status  
**Expected Result:** Shipment status updates visually

---

## 10.9 Messaging and Notifications UI

### MUI-36
**Scenario:** Open message thread and send message  
**Expected Result:** Message appears in conversation

### MUI-37
**Scenario:** Open notifications page  
**Expected Result:** Notifications are visible

### MUI-38
**Scenario:** Mark notification as read  
**Expected Result:** Notification state changes visually

---

## 10.10 Authorization UI

### MUI-39
**Scenario:** Unauthenticated user tries to access protected page  
**Expected Result:** Redirect to login page

### MUI-40
**Scenario:** User tries to edit another user's content  
**Expected Result:** Access denied or redirect occurs

---

## 11. Non-Functional Checks

### NFT-01 Performance
Open common pages and confirm they load within acceptable time locally

### NFT-02 Error Handling
Trigger invalid inputs and verify user-friendly messages appear

### NFT-03 Data Integrity
Verify related records remain consistent after create/update/delete actions

### NFT-04 Basic Usability
Check labels, buttons, forms, and navigation for clarity

### NFT-05 External API Failure
Simulate Google Books or Google distance lookup failure and confirm the app handles it gracefully

---

## 12. Test Data

Suggested sample data:

- 3 test users
- 8–10 listings with ISBNs
- 5 genres
- Wishlist entries with matching and non-matching ISBNs
- Exchange requests:
  - BuySell
  - BookSwap
  - BookSwapWithCash
- Transactions in different states:
  - Confirmed
  - Shipped
  - Completed
  - Cancelled
- Sample carriers
- Sample addresses with GooglePlaceId
- Sample messages
- Sample notifications
- Sample reviews

---

## 13. Risks

- External APIs may fail or return incomplete data
- Requirements may still evolve
- Match logic may change later
- UI pages may be incomplete during early testing
- Shipping distance calculation may require mocking during tests
- ISBN normalization may need refinement

---

## 14. Recommended Initial Priority

Start with:

1. Registration and login
2. Create/edit/delete listing
3. Wishlist add/remove
4. ISBN validation
5. Search and filtering
6. Match suggestions
7. Exchange request creation and accept/reject
8. Transaction creation
9. Shipping quote calculation
10. Messaging and notifications
11. Reviews

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