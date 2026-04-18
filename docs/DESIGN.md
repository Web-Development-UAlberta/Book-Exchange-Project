# Book Exchange Platform

## 1. Introduction

### 1.1 Purpose

This document describes the system architecture and detailed design for the Book Exchange Platform, a web-based application that allows users to exchange, buy, and sell books.

### 1.2 Scope

The system enables users to:

- List owned books
- Maintain wishlists
- Buy, sell, or swap books
- Receive match suggestions
- Communicate with other users

### 1.3 Definitions

- User: Registered individual using the platform
- Book Listing: A book entry created by a user
- Swap: Exchange of books between users
- Offer: Proposal to buy/sell/swap
- Match Engine: Logic to connect users based on book lists

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
- **Authentication**: ASP.NET Identity / JWT
- **Containerization**: Docker
- **IDE**: Visual Studio 2026

---

## 3. Detailed Design

### 3.1 Database Schema

#### Users Table

```sql
CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    username VARCHAR(100) NOT NULL UNIQUE,
    email VARCHAR(150) NOT NULL UNIQUE,
    password_hash TEXT,
    rating FLOAT,
    created_at TIMESTAMP,
    deleted_at TIMESTAMP -- NULL = active
);
```

#### Books Table

```sql
CREATE TABLE books (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    title VARCHAR(255) NOT NULL,
    isbn_13 CHAR(13) UNIQUE,
    isbn_10 CHAR(10) UNIQUE,
    authors         TEXT[]        NOT NULL DEFAULT '{}',   -- facilitates multiple authors {"Tolkien, J.R.R.","Christopher Tolkien"}
    genre_tags      TEXT[]        NOT NULL DEFAULT '{}',   -- facilitates multiple genres {"Fantasy","Fiction","Adventure"}
    published_date DATE,
    created_at TIMESTAMP NOT NULL DEFAULT now()
);
```

#### Listings Table

```sql
CREATE TABLE listings (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    book_id UUID NOT NULL REFERENCES books(id),
    condition CHAR(2) NOT NULL REFERENCES condition_codes(code),
    price DECIMAL(8,2),
    type VARCHAR(20) NOT NULL CHECK (type IN 'sell', 'buy', 'swap'), -- sell, buy, swap
    created_at TIMESTAMP NOT NULL DEFAULT now()
);
```

### Conditions Table

```sql
CREATE TABLE condition_codes (
    code CHAR(2) PRIMARY KEY, --- LN, VG, GD, AC, PF
    label VARCHAR(50) NOT NULL, --- Like New, Very Good, Good, Acceptable, Poor
    multiplier NUMERIC(8,2) NOT NULL, --- for swap valuation engine
    sort_order SMALLINT NOT NULL UNIQUE -- 1 = best, 5 = worst
);
```

#### Wishlist Table

```sql
CREATE TABLE wishlist (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    book_id UUID NOT NULL REFERENCES books(id),
    is_active BOOLEAN NOT NULL DEFAULT true,
    UNIQUE (user_id, book_id)
);
```

#### Transactions Table

```sql
CREATE TABLE transactions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    type VARCHAR(20) NOT NULL CHECK (type IN ('buy_sell', 'swap', 'multi_swap')),
    buyer_id UUID NOT NULL REFERENCES users(id),
    seller_id UUID NOT NULL REFERENCES users(id),
    listing_id UUID,
    status VARCHAR(20) NOT NULL DEFAULT 'proposed'
                                  CHECK (status IN (
                                      'proposed',
                                      'negotiating',
                                      'confirmed',
                                      'shipped',
                                      'completed',
                                      'cancelled',
                                      'disputed'
                                  )),
    total_value NUMERIC (8,2),
    created_at TIMESTAMP NOT NULL DEFAULT now(),
    confirmed_at TIMESTAMP
);
```

#### Reviews Table

```sql
CREATE TABLE reviews (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id),
    rating INT,
    comment TEXT,
    created_at TIMESTAMP NOT NULL DEFAULT now(),
);
```

---

### 3.2 Core Components

A. User Module

- Registration/Login
- Profile management
- Ratings & reviews

B. Book Module

- Add/edit/remove books
- Fetch metadata (mock data)

C. Listing Module

- Create listings (buy/sell/swap)
- Manage inventory

D. Matching Engine

- Match users based on:
  - Wishlist vs owned books
  - Price/value comparison
- Suggest swaps with balanced values
- Allow up to 3 counter-offers

E. Notification System

- Alerts for matches
- Wishlist availability notifications

F. Messaging Module

- User-to-user communication
- Negotiation support

---

### 3.3 API Endpoints (MVC Controllers)

Authentication

- POST /account/register
- POST /account/login
- POST /account/logout

Users

- GET /users/profile
- PUT /users/update

Books

- GET /books/search
- POST /books/add

Listings

- GET /listings
- POST /listings/create
- POST /listings/update
- DELETE /listings/delete

Transactions

- POST /transactions/create
- GET /transactions/history

---

## 4. User Interface Design

### 4.1 Key Screens

- Home Page
- Dashboard
- Book Listings
- Wishlist
- User Profile
- Messages
- Notifications

  4.2 Navigation Structure

```
Home
├── Dashboard
├── Books
│   ├── Search
│   └── Listings
├── Wishlist
├── Messages
├── Profile
└── Settings
```

---

## 5. Security Considerations

### 5.1 Authentication & Authorization

- ASP.NET Identity or JWT
- Role-based access control (RBAC)

### 5.2 Data Protection

- HTTPS enforced
- Input validation & sanitization
- Protection against:
  - SQL Injection
  - XSS
  - CSRF

---

## 6. Matching Logic Design

- Match when:
  - User A has a book User B wants
  - User B has a book User A wants
- Swap balancing:
  - Compare total values
  - Suggest combinations (e.g., 3 low-value books for 1 high-value)
- Counter-offers:
  - Limit to 3 iterations per negotiation

---

## 7. Testing Strategy

### 7.1 Unit Testing

- xUnit for backend logic

### 7.2 Integration Testing

- Test controllers and database interactions

### 7.3 UI Testing

- Manual Q/A
- Selenium or Playwright (Future Work)

---

## 8. Deployment Plan

### 8.1 Development

- Docker Compose:
  - ASP.NET App
  - PostgreSQL

### 8.2 Staging

- Cloud VM or Kubernetes

## 8.3 Production

- Dockerized deployment
- Reverse proxy (NGINX)
- CI/CD with GitHub Actions

---

## 9. Timeline and Milestones (Starting April 13, 2026)

## Week 01 (Apr 13 – Apr 19, 2026)

- Team Norms Document (Thursday Night) → `./docs/TEAM-NORMS.md`
- Scope Document Draft → `./docs/SCOPE.md`
- Design Document Draft → `./docs/DESIGN.md`
- Configure GitHub Repository:
  - Pull Requests to main require 2 approvals
  - Protect main branch from direct commits/pushes

---

## Week 02 (Apr 20 – Apr 26, 2026)

- Draft Entity-Relationship Diagram Revision 1 → `./docs/ERD.drawio`
- Design Document Final
- Scope Document Final → `./docs/SCOPE.md`
  - Add in-scope features to GitHub Issues
- Draft Wireframes → `./docs/wireframes/*.drawio | *.fig`
  - Include id/class names for frontend testing
- Draft Entity-Relationship Diagram Revision 2 → `./docs/ERD.drawio`
- Draft Test Plan → `./docs/TEST-PLAN.md`

---

## Week 03 (Apr 27 – May 3, 2026)

- ERD Final (Sunday Night) → `./docs/ERD.drawio`
- Final Wireframes → `./docs/wireframes/*.drawio | *.fig`
- Final Test Plan → `./docs/TEST-PLAN.md`
- Implement Object-Relational Model
- Implement Back-End Tests:
  - Configure GitHub Actions to build & run tests on main and PRs

---

## Week 04 (May 4 – May 10, 2026)

- Implementation of ORM
- README: Project Setup/Startup → `./docs/README.md`
- Implement Draft Business Logic
- Implement Front-End Tests:
  - Configure GitHub Actions for automated testing

---

## Week 05 (May 11 – May 17, 2026)

- Implementation of Business Logic
- README: Project Usage → `./docs/README.md`
- Release Candidate

---

## Week 06 (May 18 – May 24, 2026)

- Final Project Presentation and Q&A

---

## End of Course (After May 24, 2026)

- Final Revisions After Presentation
- Ensure All Tests Pass

---

## 10. Appendix

### References

- ASP.NET Core Documentation
- PostgreSQL Documentation
- Entity Framework Core Docs
- Docker Documentation

---

This design is adapted and extended based on the provided reference structure and aligned with the selected technology stack.
