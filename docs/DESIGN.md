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

B. Book Representation

- No dedicated book table is maintained
- Books are identified using a **single ISBN field**
- ISBN supports both 10 and 13 digit formats
- Book metadata (title, author, etc.) is retrieved from external APIs
- Avoids duplication and simplifies database design

C. Listing Module

- Users create listings using ISBN
- Each listing includes:
  - Condition
  - Price
  - Weight
- Listings belong to users
- No book metadata snapshot stored
- Listing type is derived at exchange time

D. Wishlist Module

- Users store desired books using ISBN
- Wishlist items can be active/inactive
- Prevent duplicate entries per user
- Used for matching

E. Matching and Recommendation Engine

- Matching based on:
  - ISBN equality between listings and wishlists  
    Supports:
- Buy/Sell
- Book swaps
- Multi-book swaps (1:3 limit)

Simplified:

- No valuation formulas
- No condition multipliers
- No AI-based matching

F. Exchange Module

- Users initiate interaction via **ExchangeRequests**
  Types:
- BuySell
- BookSwap
- BookSwapWithCash

Features:

- No negotiation
- No counter-offers
- Request includes:
  - Optional money (Price)
  - Optional offered books

Owner can:

- Accept
- Reject

G. Transaction Module

- Manage transactions for buy/sell, swap, and multi-swap scenarios
- Track participants such as buyer and seller
- Link transactions to one or more listings
- Manage transaction lifecycle through statuses such as proposed, confirmed, shipped, completed, cancelled, and disputed
- Record timestamps for confirmation, completion, and cancellation
- Support negotiation and controlled counter-offer workflow

H. Shipping and Delivery Module

- Uses Google Place ID for addresses
- Distance calculated via external API
- Carrier table with pricing model
- Shipping cost formula:
  `ShippingCost = BaseCost + (WeightKg × CostPerKg) + (DistanceKm × CostPerKm)`
- Stores shipment tracking details
- Supports future integration with shipping APIs

I. Messaging and Negotiation Module

- Provide direct user-to-user communication
- Support text messages and offer-based negotiation messages
- Optionally link messages to listings or transactions
- Store offer amounts for negotiation history
- Track message status such as sent and read
- Support negotiation flow, including limited counter-offers if required by business rules

J. Review and Reputation Module

- Allow users to leave reviews only within the context of completed transactions
- Track reviewer and reviewee for each review
- Store rating and optional comment
- Prevent duplicate reviews for the same transaction and reviewer-reviewee pair
- Use reviews to build trust and user reputation within the platform

K. Notification Module

- Send notifications for matches, wishlist availability, new messages, offers, and transaction updates
- Support notification states such as unread, read, and archived
- Link notifications to related books, listings, wishlists, or transactions
- Record creation and read timestamps
- Help users stay informed about marketplace and transaction activity

---

### 3.2 API Endpoints (MVC Controllers)

A. Account (/account)

- POST /register
- POST /login
- POST /logout

---

B. Users (/users)

- GET /profile
- PUT /update
- GET /{id}
- GET /{id}/reviews
- GET /{id}/addresses
- POST /addresses
- PUT /addresses/{id}
- DELETE /addresses/{id}

---

C. Listings (/listings)

- GET /
- GET /{id}
- POST /create
- PUT /{id}
- DELETE /{id}
- GET /user/{userId}

---

D. Wishlist (/wishlist)

- GET /
- POST /add
- DELETE /remove/{isbn}
- PUT /{id}/toggle

---

E. Matching (/matching)

- GET /suggestions
- GET /listing/{id}

---

F. ExchangeRequests (/exchange)

- POST /create
- GET /{id}
- PUT /{id}/accept
- PUT /{id}/reject
- GET /user

---

G. Transactions (/transactions)

- GET /history
- GET /{id}
- PUT /{id}/status

---

H. Shipments (/shipments)

- POST /create
- GET /{id}
- PUT /{id}/quote
- PUT /{id}/status

---

I. Messages (/messages)

- GET /
- GET /{userId}
- POST /send
- PUT /{id}/read

---

J. Reviews (/reviews)

- POST /create
- GET /user/{userId}

---

K. Notifications (/notifications)

- GET /
- PUT /{id}/read
- PUT /read-all
- DELETE /{id}

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

### Matching Rule

- `Listing.Isbn == Wishlist.Isbn`

---

### Constraints

- Swap ratio: max 3:1 or 1:3
- No negotiation
- No multi-user chain swaps
- Owner decision is final

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
