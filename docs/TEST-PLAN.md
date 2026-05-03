# Test Plan – Book Exchange Platform

### Table of Contents

1. [Purpose](#1-purpose)
2. [Scope](#2-scope)
3. [Test Objectives](#3-test-objectives)
4. [Test Types](#4-test-types)
5. [Assumptions](#5-assumptions)
6. [Test Environment](#6-test-environment)
7. [Entry and Exit Criteria](#7-entry-and-exit-criteria)
8. [Unit Test Scenario](#8-unit-test-scenarios)
9. [Integration Test Scenarios](#9-integration-test-scenarios)
10. [Manual UI Test Scenarios](#10-manual-ui-test-scenarios)
11. [Non-Functional Checks](#11-non-functional-checks)
12. [Test Data](#12-test-data)
13. [Risks](#13-risks)
14. [Recommended Initial Priority](#14-recommended-initial-priority)
15. [Deliverables](#15-deliverables)
16. [Conclusion](#16-conclusion)

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

### 8.1 Authentication / User Validation

| Number     | Scenario                                    | Expected Result                     |
| ---------- | ------------------------------------------- | ----------------------------------- |
| UT-AUTH-01 | Register user with valid data               | User is accepted and can be created |
| UT-AUTH-02 | Register user with missing required fields  | Validation fails                    |
| UT-AUTH-03 | Register user with duplicate email/username | Duplicate registration is rejected  |

### 8.2 Address Management

| Number     | Scenario                                             | Expected Result     |
| ---------- | ---------------------------------------------------- | ------------------- |
| UT-ADDR-01 | Create address with valid FullName and GooglePlaceId | Address is accepted |
| UT-ADDR-02 | Create address with null or empty FullName           | Validation fails    |
| UT-ADDR-03 | Create address with null or empty FullName           | Validation fails    |
| UT-ADDR-04 | Create address with invalid GooglePlaceId            | Validation fails    |
| UT-ADDR-05 | Get address by ID belonging to the user              | Address is returned |
| UT-ADDR-06 | Get another users address                            | Access is denied    |
| UT-ADDR-07 | Update Address with new GooglePlaceID                | Address is accepted |
| UT-ADDR-08 | Delete an address not being used on a shipment       | Address is deleted  |
| UT-ADDR-09 | Delete an address that is being used on a shipment   | Validation Fails    |

### 8.3 Listing Management

| Number     | Scenario                                                     | Expected Result                                                  |
| ---------- | ------------------------------------------------------------ | ---------------------------------------------------------------- |
| UT-LIST-01 | Create listing with valid ISBN, condition, price, and weight | Listing is created successfully                                  |
| UT-LIST-02 | Create listing with invalid ISBN format                      | Validation fails                                                 |
| UT-LIST-03 | Create listing with negative price                           | Validation fails                                                 |
| UT-LIST-04 | Create listing with zero or negative weight                  | Validation fails                                                 |
| UT-LIST-05 | Update listing with valid new values                         | Listing is updated successfully                                  |
| UT-LIST-06 | Update listing status from Active to Pending                 | Status change is saved correctly                                 |
| UT-LIST-07 | Mark listing as Completed after successful transaction       | Listing is no longer shown as active                             |
| UT-LIST-08 | Delete existing listing                                      | Listing is removed or marked unavailable based on implementation |
| UT-LIST-09 | User attempts to update another user's listing               | Operation is rejected                                            |
| UT-LIST-10 | User attempts to delete another user's listing               | Operation is rejected                                            |

---

### 8.4 Wishlist Management

| Number     | Scenario                                             | Expected Result              |
| ---------- | ---------------------------------------------------- | ---------------------------- |
| UT-WISH-01 | Add valid ISBN to wishlist                           | Wishlist item is added       |
| UT-WISH-02 | Add invalid ISBN to wishlist                         | Validation fails             |
| UT-WISH-03 | Add duplicate ISBN to same user's wishlist           | Duplicate is prevented       |
| UT-WISH-04 | Remove item from wishlist                            | Item is removed successfully |
| UT-WISH-05 | Toggle wishlist item active/inactive                 | IsActive updates correctly   |
| UT-WISH-06 | User attempts to update another user's wishlist item | Operation is rejected        |
| UT-WISH-07 | User attempts to delete another user's wishlist item | Operation is rejected        |

---

### 8.5 Search and Filtering

| Number       | Scenario                                        | Expected Result                                             |
| ------------ | ----------------------------------------------- | ----------------------------------------------------------- |
| UT-SEARCH-01 | Search listings by ISBN                         | Matching listings are returned                              |
| UT-SEARCH-02 | Filter listings by genre                        | Only listings with selected genre are returned              |
| UT-SEARCH-03 | Filter listings by condition                    | Only listings with selected condition are returned          |
| UT-SEARCH-04 | Filter listings by status Active                | Only active listings are returned                           |
| UT-SEARCH-05 | Search listings by book title keyword           | Listings whose book title contains the keyword are returned |
| UT-SEARCH-06 | Search listings by author name                  | Listings whose book author matches the query are returned   |
| UT-SEARCH-07 | Search with keyword and active filters combined | Results match both the keyword and all applied filters      |
| UT-SEARCH-08 | Search is case-insensitive                      | "tolkien" and "Tolkien" return the same results             |

---

### 8.6 Match Logic

| Number      | Scenario                                                      | Expected Result                       |
| ----------- | ------------------------------------------------------------- | ------------------------------------- |
| UT-MATCH-01 | User wishlist ISBN matches another user's active listing ISBN | Match is returned                     |
| UT-MATCH-02 | No matching listing exists                                    | Empty result returned                 |
| UT-MATCH-03 | Exclude user's own listing from match results                 | User's own listings are not suggested |
| UT-MATCH-04 | Inactive wishlist item matches a listing                      | Match is not returned                 |
| UT-MATCH-05 | Completed or cancelled listing matches wishlist               | Match is not returned                 |

---

### 8.7 Exchange Requests

| Number     | Scenario                                                       | Expected Result                                      |
| ---------- | -------------------------------------------------------------- | ---------------------------------------------------- |
| UT-EXCH-01 | Create BuySell exchange request for active listing             | ExchangeRequest is created with Requested status     |
| UT-EXCH-02 | Create BookSwap exchange request with one offered listing      | ExchangeRequest and ExchangeRequestItems are created |
| UT-EXCH-03 | Create BookSwapWithCash request with offered listing and price | ExchangeRequest is created successfully              |
| UT-EXCH-04 | Create swap request with more than three offered listings      | Request is rejected by business rule                 |
| UT-EXCH-05 | Create exchange request for own listing                        | Request is rejected                                  |
| UT-EXCH-06 | Accept valid exchange request                                  | ExchangeRequest status becomes Accepted              |
| UT-EXCH-07 | Reject exchange request                                        | ExchangeRequest status becomes Rejected              |
| UT-EXCH-08 | Invalid exchange status transition                             | System rejects invalid transition                    |

---

### 8.8 Transactions

| Number      | Scenario                                            | Expected Result                               |
| ----------- | --------------------------------------------------- | --------------------------------------------- |
| UT-TRANS-01 | Create transaction from accepted exchange request   | Transaction is created with Confirmed status  |
| UT-TRANS-02 | Attempt to create transaction from rejected request | Transaction creation is rejected              |
| UT-TRANS-03 | Complete transaction                                | Transaction status changes to Completed       |
| UT-TRANS-04 | Cancel transaction                                  | Transaction status changes to Cancelled       |
| UT-TRANS-05 | Invalid status transition                           | System rejects invalid transition             |
| UT-TRANS-06 | Mark transaction as shipped                         | Transaction status confrimed moves to shipped |
| UT-TRANS-07 | Dispute a Transaction                               | Transaction status transition is valid        |

---

### 8.9 Shipping

| Number     | Scenario                                                         | Expected Result                                      |
| ---------- | ---------------------------------------------------------------- | ---------------------------------------------------- |
| UT-SHIP-01 | Calculate shipping cost with valid carrier, weight, and distance | Shipping cost is calculated correctly                |
| UT-SHIP-02 | Shipping weight exceeds carrier max weight                       | Carrier is rejected or unavailable                   |
| UT-SHIP-03 | Missing sender or receiver address                               | Shipment creation fails                              |
| UT-SHIP-04 | Distance API returns valid distance                              | DistanceKm is stored and shipping cost is calculated |
| UT-SHIP-05 | Distance API fails                                               | System handles failure gracefully                    |

---

### 8.10 Reviews / Ratings

| Number       | Scenario                                               | Expected Result                        |
| ------------ | ------------------------------------------------------ | -------------------------------------- |
| UT-REVIEW-01 | Create review for completed transaction                | Review is accepted                     |
| UT-REVIEW-02 | Create review before transaction completion            | Review is rejected                     |
| UT-REVIEW-03 | Rating outside allowed range                           | Validation fails                       |
| UT-REVIEW-04 | Duplicate review for same transaction by same reviewer | Duplicate is rejected                  |
| UT-REVIEW-05 | Aggregate user rating from multiple reviews            | Average rating is calculated correctly |

---

### 8.11 Notifications

| Number      | Scenario                                               | Expected Result                                |
| ----------- | ------------------------------------------------------ | ---------------------------------------------- |
| UT-NOTIF-01 | Trigger notification when a match is found             | Notification record is created                 |
| UT-NOTIF-02 | Trigger notification when message is received          | Notification is created                        |
| UT-NOTIF-03 | Trigger notification when exchange request is accepted | Notification is created                        |
| UT-NOTIF-04 | Mark notification as read                              | Status changes to Read and ReadAt is populated |

---

### 8.12 Messages

| Number    | Scenario                                   | Expected Result                                           |
| --------- | ------------------------------------------ | --------------------------------------------------------- |
| UT-MSG-01 | Send message with valid data               | Message is created and sent                               |
| UT-MSG-02 | Send message to self                       | Validation fails                                          |
| UT-MSG-03 | Send message with empty text               | Validation fails                                          |
| UT-MSG-04 | Send message to a user that does not exist | Validation fails                                          |
| UT-MSG-05 | Retrieve a message by its ID               | The correct message is received                           |
| UT-MSG-06 | Retrieve a message by non-existent ID      | Validation fails                                          |
| UT-MSG-07 | Mark message as read by the receiver       | Operation Completes successfully                          |
| UT-MSG-08 | Get unread message count                   | Correct count of messages is received                     |
| UT-MSG-09 | Get conversation between users             | Messages are returned in ascending order                  |
| UT-MSG-10 | Get inbox for user                         | Conversations are listed by latest messages in descending |
| UT-MSG-11 | Mark conversation as read                  | All messages in coversation are marked as read            |

---

## 9. Integration Test Scenarios

### 9.1 Authentication Integration

| Number     | Scenario                                | Expected Result                                    |
| ---------- | --------------------------------------- | -------------------------------------------------- |
| IT-AUTH-01 | User registers through request pipeline | User record exists and registration flow succeeds  |
| IT-AUTH-02 | Valid login                             | User is authenticated and redirected appropriately |
| IT-AUTH-03 | Access protected page without login     | User is redirected to login page                   |

---

### 9.2 Listing Integration

| Number     | Scenario                                  | Expected Result                                     |
| ---------- | ----------------------------------------- | --------------------------------------------------- |
| IT-LIST-01 | Authenticated user creates a listing      | Listing appears in My Listings and database         |
| IT-LIST-02 | User creates listing with invalid ISBN    | Validation error is shown and database is unchanged |
| IT-LIST-03 | Edit existing listing                     | Updated values are saved and displayed              |
| IT-LIST-04 | Delete listing                            | Listing is removed or marked unavailable            |
| IT-LIST-05 | User tries to edit another user's listing | Access is denied or operation is blocked            |

---

### 9.3 Wishlist Integration

| Number     | Scenario                                          | Expected Result                          |
| ---------- | ------------------------------------------------- | ---------------------------------------- |
| IT-WISH-01 | User adds ISBN to wishlist                        | Wishlist item is saved and shown         |
| IT-WISH-02 | User adds duplicate ISBN                          | Duplicate is blocked                     |
| IT-WISH-03 | Remove wishlist item                              | Item no longer appears                   |
| IT-WISH-04 | User tries to update another user's wishlist item | Access is denied or operation is blocked |

---

### 9.4 Matching Integration

| Number      | Scenario                                                                  | Expected Result          |
| ----------- | ------------------------------------------------------------------------- | ------------------------ |
| IT-MATCH-01 | User wishlist matches another user's active listing                       | Match suggestion appears |
| IT-MATCH-02 | Matching listing belongs to same user                                     | Match is excluded        |
| IT-MATCH-03 | Listing has an accepted exchange request with a non-cancelled transaction | Match is excluded        |

---

### 9.5 Exchange Request Integration

| Number     | Scenario                                       | Expected Result                                                       |
| ---------- | ---------------------------------------------- | --------------------------------------------------------------------- |
| IT-EXCH-01 | User creates BuySell exchange request          | ExchangeRequest is saved with Requested status                        |
| IT-EXCH-02 | User creates BookSwap exchange request         | ExchangeRequest and ExchangeRequestItems are saved                    |
| IT-EXCH-03 | User creates BookSwapWithCash exchange request | Request includes offered listing(s) and Price                         |
| IT-EXCH-04 | Listing owner accepts exchange request         | ExchangeRequest status becomes Accepted and Transaction is created    |
| IT-EXCH-05 | Listing owner rejects exchange request         | ExchangeRequest status becomes Rejected and no Transaction is created |

---

### 9.6 Transaction Integration

| Number      | Scenario                                      | Expected Result                          |
| ----------- | --------------------------------------------- | ---------------------------------------- |
| IT-TRANS-01 | Accepted exchange creates transaction         | Transaction is linked to ExchangeRequest |
| IT-TRANS-02 | Completing transaction updates listing status | Related listing(s) are no longer active  |
| IT-TRANS-03 | Transaction history is requested              | User sees relevant transactions          |

---

### 9.7 Shipping Integration

| Number     | Scenario                                          | Expected Result                             |
| ---------- | ------------------------------------------------- | ------------------------------------------- |
| IT-SHIP-01 | Create shipment for transaction                   | Shipment is saved and linked to transaction |
| IT-SHIP-02 | Quote shipment using carrier pricing and distance | DistanceKm and ShippingCost are calculated  |
| IT-SHIP-03 | Update shipment status                            | Shipment status updates correctly           |

---

### 9.8 Messaging Integration

| Number    | Scenario                                 | Expected Result                                       |
| --------- | ---------------------------------------- | ----------------------------------------------------- |
| IT-MSG-01 | Send message between users               | Message is stored and visible in conversation thread  |
| IT-MSG-02 | Conversation thread is returned in order | Messages between users is returned in ascending order |
| IT-MSG-03 | Received marks unread message as read    | Operation completes successfully                      |
| IT-MSG-04 | Mark conversation as read                | All messages in coversation are marked as read        |
| IT-MSG-05 | Get unread message count                 | Correct count of messages is received                 |

---

### 9.9 Review Integration

| Number       | Scenario                                        | Expected Result                             |
| ------------ | ----------------------------------------------- | ------------------------------------------- |
| IT-REVIEW-01 | User submits review after completed transaction | Review is saved and linked to transaction   |
| IT-REVIEW-02 | User profile displays updated rating            | Aggregate rating is updated on profile page |

---

## 9.10 Notification Integration Tests

| Number      | Scenario                                          | Expected Result                                                                |
| ----------- | ------------------------------------------------- | ------------------------------------------------------------------------------ |
| IT-NOTIF-01 | System creates notification for user              | Notification is saved in database with IsRead = false and ReadAt = null        |
| IT-NOTIF-02 | User retrieves notifications                      | Only notifications belonging to the user are returned                          |
| IT-NOTIF-03 | User retrieves unread notifications               | Only notifications with IsRead = false are returned                            |
| IT-NOTIF-04 | User marks notification as read                   | IsRead = true and ReadAt is populated                                          |
| IT-NOTIF-05 | User attempts to mark another user’s notification | Operation is rejected and notification remains unchanged                       |
| IT-NOTIF-06 | System counts unread notifications                | Correct number of unread notifications is returned                             |
| IT-NOTIF-07 | System creates notification with related entities | Notification is saved with correct RelatedListingId / ExchangeRequestId / etc. |

---

### 9.11 Search Integration

| Number       | Scenario                                                  | Expected Result                                               |
| ------------ | --------------------------------------------------------- | ------------------------------------------------------------- |
| IT-SEARCH-01 | Search listings by title keyword through request pipeline | Matching active listings are returned from the database       |
| IT-SEARCH-02 | Search listings by author name through request pipeline   | Matching active listings are returned from the database       |
| IT-SEARCH-03 | Combine keyword search with genre and condition filters   | Results satisfy all search and filter criteria simultaneously |

---

### 9.12 Address Integration

| Number     | Scenario                                                | Expected Result                               |
| ---------- | ------------------------------------------------------- | --------------------------------------------- |
| IT-ADDR-01 | User creates a valid address                            | Address is saved                              |
| IT-ADDR-02 | Create address with invalid GooglePlaceId               | Validation fails                              |
| IT-ADDR-03 | User retrieves all their saved addresses                | Only addresses belonging to them are returned |
| IT-ADDR-04 | User updates their address                              | New address is saved with new GooglePlaceID   |
| IT-ADDR-05 | User deletes their address that has no active shipments | Address is removed                            |
| IT-ADDR-06 | User deletes their address that has active shipments    | Validation fails                              |

---

## 10. Manual UI Test Scenarios

### 10.1 General Navigation

| Number | Scenario                                                                                                   | Expected Result                                    |
| ------ | ---------------------------------------------------------------------------------------------------------- | -------------------------------------------------- |
| MUI-01 | Open home page                                                                                             | Page loads without error and navigation is visible |
| MUI-02 | Navigate through major pages (Home, Login, Register, Listings, Wishlist, Profile, Messages, Notifications) | All links work correctly                           |

---

### 10.2 Registration and Login

| Number | Scenario                       | Expected Result                                   |
| ------ | ------------------------------ | ------------------------------------------------- |
| MUI-03 | Register with valid data       | Account is created successfully                   |
| MUI-04 | Register with invalid data     | Validation messages are shown                     |
| MUI-05 | Login with valid credentials   | Login succeeds                                    |
| MUI-06 | Login with invalid credentials | Error message is shown                            |
| MUI-07 | Logout                         | Session ends and protected pages are inaccessible |

---

### 10.3 Profile and Address UI

| Number | Scenario                                           | Expected Result                |
| ------ | -------------------------------------------------- | ------------------------------ |
| MUI-08 | View profile                                       | User details display correctly |
| MUI-09 | Edit profile                                       | Changes are saved and visible  |
| MUI-10 | Add address using Google Place ID / address lookup | Address is saved successfully  |
| MUI-11 | Add address without valid location                 | Validation message is shown    |

---

### 10.4 Listings UI

| Number | Scenario                                  | Expected Result                           |
| ------ | ----------------------------------------- | ----------------------------------------- |
| MUI-12 | Create new listing through form           | Listing is created and shown              |
| MUI-13 | Submit listing form with invalid ISBN     | Validation message appears                |
| MUI-14 | Submit listing with negative price        | Validation message appears                |
| MUI-15 | Edit own listing                          | Updated values appear after saving        |
| MUI-16 | Delete own listing                        | Listing disappears or becomes unavailable |
| MUI-17 | Try to edit another user's listing by URL | Access denied or redirect                 |

---

### 10.5 Wishlist UI

| Number | Scenario                             | Expected Result                    |
| ------ | ------------------------------------ | ---------------------------------- |
| MUI-18 | Add ISBN to wishlist                 | Item appears in wishlist page      |
| MUI-19 | Add duplicate ISBN to wishlist       | Duplicate is blocked               |
| MUI-20 | Remove wishlist item                 | Item disappears from wishlist page |
| MUI-21 | Toggle wishlist item active/inactive | Status updates visually            |

---

### 10.6 Search UI

| Number | Scenario                               | Expected Result                                                       |
| ------ | -------------------------------------- | --------------------------------------------------------------------- |
| MUI-22 | Search by ISBN                         | Matching listings are shown                                           |
| MUI-23 | Use filters such as genre or condition | Results update correctly                                              |
| MUI-24 | Search with no results                 | Friendly "no results found" message is shown                          |
| MUI-25 | Search by book title keyword           | Matching listings appear with correct title displayed on result card  |
| MUI-26 | Search by author name                  | Matching listings appear with correct author displayed on result card |
| MUI-27 | Search is case-insensitive             | Results are the same regardless of capitalisation used in the query   |

---

### 10.7 Matching and Exchange UI

| Number | Scenario                           | Expected Result                             |
| ------ | ---------------------------------- | ------------------------------------------- |
| MUI-28 | View match suggestions             | Matching listings are displayed             |
| MUI-29 | Start buy/sell request             | Exchange request is submitted               |
| MUI-30 | Start book swap request            | User can select up to 3 offered books       |
| MUI-31 | Start book + money swap request    | User can select books and enter extra Price |
| MUI-32 | Attempt to offer more than 3 books | UI blocks or validation message appears     |
| MUI-33 | Listing owner accepts request      | Transaction is created                      |
| MUI-34 | Listing owner rejects request      | Request status becomes rejected             |

---

### 10.8 Transaction and Shipping UI

| Number | Scenario                        | Expected Result                             |
| ------ | ------------------------------- | ------------------------------------------- |
| MUI-35 | View transaction history        | Past and current transactions are displayed |
| MUI-36 | Create shipment for transaction | Shipment form saves correctly               |
| MUI-37 | Quote shipping cost             | Shipping cost displays correctly            |
| MUI-38 | Update shipment status          | Shipment status updates visually            |

---

### 10.9 Messaging and Notifications UI

| Number | Scenario                             | Expected Result                     |
| ------ | ------------------------------------ | ----------------------------------- |
| MUI-39 | Open message thread and send message | Message appears in conversation     |
| MUI-40 | Open notifications page              | Notifications are visible           |
| MUI-41 | Mark notification as read            | Notification state changes visually |

---

### 10.10 Authorization UI

| Number | Scenario                                            | Expected Result                  |
| ------ | --------------------------------------------------- | -------------------------------- |
| MUI-42 | Unauthenticated user tries to access protected page | Redirect to login page           |
| MUI-43 | User tries to edit another user's content           | Access denied or redirect occurs |

---

## 11. Non-Functional Checks

| Number | Check                | Description                                                                                       |
| ------ | -------------------- | ------------------------------------------------------------------------------------------------- |
| NFT-01 | Performance          | Open common pages and confirm they load within acceptable time locally                            |
| NFT-02 | Error Handling       | Trigger invalid inputs and verify user-friendly messages appear                                   |
| NFT-03 | Data Integrity       | Verify related records remain consistent after create/update/delete actions                       |
| NFT-04 | Basic Usability      | Check labels, buttons, forms, and navigation for clarity                                          |
| NFT-05 | External API Failure | Simulate Google Books or Google distance lookup failure and confirm the app handles it gracefully |

---

## 12. Test Data

Suggested sample data:

- 3 test users
- 8–10 listings with ISBNs
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

[Back to Top](#1-purpose)
