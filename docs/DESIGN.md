# Book Exchange Platform

### Table of Contents

1. [Introduction](#1-introduction)
2. [System Architecture](#2-system-architecture)
3. [Detailed Design](#3-detailed-design)
4. [Security Considerations](#4-security-considerations)
5. [Matching Logic Design](#5-matching-logic-design)
6. [Testing Strategy](#6-testing-strategy)
7. [Deployment Plan](#7-deployment-plan)
8. [Timeline & Milestones Detailed](#8-timeline-and-milestones-starting-april-13-2026)
9. [Appendix](#9-appendix)

## 1. Introduction

This document describes the system architecture and detailed design for the Book Exchange Platform, a web-based application that allows users to exchange, buy, and sell books.

---

## 2. System Architecture

### 2.1 High-Level Architecture

The system follows a **three-tier architecture**:

- **Presentation Layer**: Razor Views (UI)
- **Application Layer**: ASP.NET Core MVC (.NET 10)
- **Data Layer**: PostgreSQL Database

---

### 2.2 Technology Stack

- **Backend**: .NET 10, C# ASP.NET Core MVC
- **Frontend**: Razor, Bootstrap, JavaScript, HTML, CSS
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core
- **Authentication**: ASP.NET Identity
- **Containerization**: Docker
- **IDE**: Visual Studio 2026

---

## 3. Detailed Design

### 3.1 Core Components

A. Identity and User Module

- User registration, login, logout, and authentication using ASP.NET Identity
- Role-based access control using users, roles, claims, logins, and tokens
- User profile management
- Address management for shipping and delivery
- Location support based on city, province/state, and country
- User reputation through transaction-based ratings and reviews

B. Book Catalog Module

- Maintain master book records
- Store book details such as title, ISBN-10, ISBN-13, and published date
- Manage authors and book-author relationships
- Manage genres and book-genre relationships
- Support book metadata retrieval from mock or external sources
- Reuse shared book records across multiple users and listings

C. Wishlist Module

- Allow users to add books to their wishlist
- Mark wishlist items as active or inactive
- Track user interest in books for future matching and notifications
- Prevent duplicate wishlist entries for the same user and book

D. Listing Module

- Create book listings for selling, buying, or swapping
- Store listing-specific details such as condition, price, weight, and listing type
- Use condition codes and multipliers to support valuation and swap balancing
- Associate each listing with a user and a book
- Manage listing lifecycle and availability for transactions

E. Matching and Recommendation Engine

- Match users based on wishlist books and available listings
- Detect potential buy, sell, and swap opportunities
- Compare listing prices and condition-based values
- Suggest balanced swaps using book condition multipliers
- Support simple multi-party or multi-listing matching scenarios
- Trigger notifications when suitable matches are found

F. Transaction Module

- Manage transactions for buy/sell, swap, and multi-swap scenarios
- Track participants such as buyer and seller
- Link transactions to one or more listings
- Manage transaction lifecycle through statuses such as proposed, negotiating, confirmed, shipped, completed, cancelled, and disputed
- Record timestamps for confirmation, completion, and cancellation
- Support negotiation and controlled counter-offer workflow

G. Shipping and Delivery Module

- Store user shipping addresses
- Manage carriers and carrier pricing rules
- Calculate shipping cost using base cost, package weight, and travel distance
- Support location-to-location distance lookup
- Create shipment records linked to transactions
- Track shipment status, carrier, tracking number, label URL, and shipping cost
- Enable future integration with external shipping carrier APIs for label generation and shipment automation

H. Messaging and Negotiation Module

- Provide direct user-to-user communication
- Support text messages and offer-based negotiation messages
- Optionally link messages to listings or transactions
- Store offer amounts for negotiation history
- Track message status such as sent and read
- Support negotiation flow, including limited counter-offers if required by business rules

I. Review and Reputation Module

- Allow users to leave reviews only within the context of completed transactions
- Track reviewer and reviewee for each review
- Store rating and optional comment
- Prevent duplicate reviews for the same transaction and reviewer-reviewee pair
- Use reviews to build trust and user reputation within the platform

J. Notification Module

- Send notifications for matches, wishlist availability, new messages, offers, and transaction updates
- Support notification states such as unread, read, and archived
- Link notifications to related books, listings, wishlists, or transactions
- Record creation and read timestamps
- Help users stay informed about marketplace and transaction activity

---

### 3.2 API Endpoints (MVC Controllers)

A. Authentication Controller (/account)

- POST /account/register → Register new user
- POST /account/login → Authenticate user
- POST /account/logout → Logout user

---

B. Users Controller (/users)

- GET /users/profile → Get current user profile
- PUT /users/update → Update profile info
- GET /users/{id} → Get public user profile
- GET /users/{id}/reviews → Get user reviews and rating
- GET /users/{id}/addresses → Get user addresses
- POST /users/addresses → Add new address
- PUT /users/addresses/{id} → Update address
- DELETE /users/addresses/{id} → Delete address

---

C. Books Controller (/books)

- GET /books/search → Search books (title, ISBN, author, genre)
- GET /books/{id} → Get book details
- POST /books/add → Add new book (admin or system use)
- POST /books/{id}/authors → Assign authors to book
- POST /books/{id}/genres → Assign genres to book

---

D. Wishlist Controller (/wishlist)

- GET /wishlist → Get user wishlist
- POST /wishlist/add → Add book to wishlist
- DELETE /wishlist/remove/{bookId} → Remove book from wishlist
- PUT /wishlist/{id}/toggle → Activate/deactivate wishlist item

---

E. Listings Controller (/listings)

- GET /listings → Get all listings (with filters: type, price, condition, location)
- GET /listings/{id} → Get listing details
- POST /listings/create → Create new listing
- PUT /listings/{id} → Update listing
- DELETE /listings/{id} → Delete listing
- GET /listings/user/{userId} → Get listings by user

---

F. Matching Controller (/matching)

- GET /matching/suggestions → Get personalized match suggestions
- GET /matching/swaps → Get potential swap matches
- GET /matching/listing/{id} → Get matches for a specific listing

---

G. Transactions Controller (/transactions)

- POST /transactions/create → Create new transaction (buy/sell/swap)
- GET /transactions/history → Get user transaction history
- GET /transactions/{id} → Get transaction details
- PUT /transactions/{id}/status → Update transaction status
- POST /transactions/{id}/confirm → Confirm transaction
- POST /transactions/{id}/cancel → Cancel transaction

---

H. Shipments Controller (/shipments)

- POST /shipments/create → Create shipment for a transaction
- GET /shipments/{id} → Get shipment details
- PUT /shipments/{id}/quote → Calculate shipping cost
- PUT /shipments/{id}/label → Generate shipping label
- PUT /shipments/{id}/status → Update shipment status

---

I. Messaging Controller (/messages)

- GET /messages → Get user conversations
- GET /messages/{userId} → Get conversation with a user
- POST /messages/send → Send message
- PUT /messages/{id}/read → Mark message as read

---

J. Reviews Controller (/reviews)

- POST /reviews/create → Create review after transaction
- GET /reviews/user/{userId} → Get reviews for a user

---

K. Notifications Controller (/notifications)

- GET /notifications → Get user notifications
- PUT /notifications/{id}/read → Mark notification as read
- PUT /notifications/read-all → Mark all as read
- DELETE /notifications/{id} → Archive/delete notification

---

## 4. Security Considerations

### 4.1 Authentication & Authorization

- ASP.NET Identity

### 4.2 Data Protection

- HTTPS enforced
- Input validation & sanitization
- Protection against:
  - SQL Injection
  - XSS
  - CSRF

---

## 5. Matching Logic Design

- Match when:
  - User A has a book User B wants
  - User B has a book User A wants
- Swap balancing:
  - Compare total values
  - Suggest combinations (e.g., 3 low-value books for 1 high-value)
- Counter-offers:
  - Limit to 3 iterations per negotiation

---

## 6. Testing Strategy

### 6.1 Unit Testing

- xUnit for backend logic

### 6.2 Integration Testing

- Test controllers and database interactions

### 6.3 UI Testing

- Manual Q/A
- Selenium or Playwright (Future Work)

---

## 7. Deployment Plan

### 7.1 Development

- Docker Compose:
- ASP.NET App
- PostgreSQL

### 7.2 Staging

- Cloud VM or Kubernetes

### 7.3 Production

- Dockerized deployment
- Reverse proxy (NGINX)
- CI/CD with GitHub Actions

---

## 8. Timeline and Milestones (Starting April 13, 2026)

### Week 01 (Apr 13 – Apr 19, 2026)

- Team Norms Document (Thursday Night) → `./docs/TEAM-NORMS.md`
- Scope Document Draft → `./docs/SCOPE.md`
- Design Document Draft → `./docs/DESIGN.md`
- Configure GitHub Repository:
  - Pull Requests to main require 2 approvals
  - Protect main branch from direct commits/pushes

---

### Week 02 (Apr 20 – Apr 26, 2026)

- Draft Entity-Relationship Diagram Revision 1 → `./docs/ERD.drawio`
- Design Document Final
- Scope Document Final → `./docs/SCOPE.md`
  - Add in-scope features to GitHub Issues
- Draft Wireframes → `./docs/wireframes/*.drawio | *.fig`
  - Include id/class names for frontend testing
- Draft Entity-Relationship Diagram Revision 2 → `./docs/ERD.drawio`
- Draft Test Plan → `./docs/TEST-PLAN.md`

---

### Week 03 (Apr 27 – May 3, 2026)

- ERD Final (Sunday Night) → `./docs/ERD.drawio`
- Final Wireframes → `./docs/wireframes/*.drawio | *.fig`
- Final Test Plan → `./docs/TEST-PLAN.md`
- Implement Object-Relational Model
- Implement Back-End Tests:
  - Configure GitHub Actions to build & run tests on main and PRs

---

### Week 04 (May 4 – May 10, 2026)

- Implementation of ORM
- README: Project Setup/Startup → `./docs/README.md`
- Implement Draft Business Logic
- Implement Front-End Tests:
  - Configure GitHub Actions for automated testing

---

### Week 05 (May 11 – May 17, 2026)

- Implementation of Business Logic
- README: Project Usage → `./docs/README.md`
- Release Candidate

---

### Week 06 (May 18 – May 24, 2026)

- Final Project Presentation and Q&A

---

### End of Course (After May 24, 2026)

- Final Revisions After Presentation
- Ensure All Tests Pass

---

## 9. Appendix

### References

- ASP.NET Core Documentation
- PostgreSQL Documentation
- Entity Framework Core Docs
- Docker Documentation

---

This design is adapted and extended based on the provided reference structure and aligned with the selected technology stack.
