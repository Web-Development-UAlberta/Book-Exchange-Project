# 📘 Book Exchange Platform

### Table of Contents

1. [Project Overview](#1-project-overview)
2. [Objectives](#2-objectives)
3. [Scope of Features](#3-scope-of-features)
4. [Business Rules](#4-business-rules)
5. [Data & Integration](#5-data--integration)
6. [Non-Functional Requirements](#6-non-functional-requirements)
7. [Out of Scope (Initial Version)](#7-out-of-scope-initial-version)
8. [Success Criteria](#8-success-criteria)
9. [Timeline](#9-timeline)

## 1. Project Overview

The Book Exchange Platform is a web-based system that enables users to exchange, buy, and sell books. Users can list books they own, specify books they want, and interact with other users through swap proposals, direct purchases, or sales.

The platform leverages matching logic and mock book data to connect users and facilitate transactions efficiently.

---

## 2. Objectives

- Enable users to list owned books and desired books
- Provide automated matching between users
- Support multiple transaction types:
  - Swap
  - Buy
  - Sell
- Ensure a user-friendly search and filtering experience
- Build trust through ratings and reviews
- Facilitate communication and transaction completion

---

## 3. Scope of Features

### 3.1 User Management

- User registration and authentication
- User profiles including:
  - Owned books
  - Wishlist
  - Ratings and reviews
- Profile reputation system based on transaction feedback

---

### 3.2 Book Management

- Add/edit/remove owned books
- Add books to wishlist
- Retrieve book metadata using mock data (title, author, ISBN)
- Define book attributes:
  - Author
  - Genre
  - Publishing date
  - Price
  - Condition

---

### 3.3 Transaction Types

#### A. Selling

- Users can list books with a fixed price

#### B. Buying

- Users can submit purchase offers

#### C. Swapping

- Users can propose book exchanges
- System suggests value-balanced swaps
  - Example: 3 × $10 books for 1 × $30 book
  - Example: 2 x $10 books + $10.00 for 1 x $30 book

---

### 3.4 Matching Engine

- Match users based on:
  - Owned books vs wishlist
  - Buy/sell compatibility
  - Swap feasibility
- Consider value discrepancies when suggesting swaps
- Support up to 3 counter-offer permutations per negotiation

---

### 3.5 Search and Filtering

- Advanced filtering by:
  - Author
  - Genre
  - Publishing date
  - Price range
- Keyword-based search

---

### 3.6 Wishlist & Notifications

- Users can maintain a wishlist
- Notification system alerts users when:
  - Desired books become available
  - Matching opportunities are found

---

### 3.7 Book Condition System

- Users specify book condition:
  - New
  - Like New
  - Good
  - Fair
- Provide guidelines for consistency

---

### 3.8 Communication System

- Messaging or notification-based interaction between users
- Used for:
  - Negotiations
  - Finalizing swaps or purchases

---

### 3.9 Rating & Review System

- Users can:
  - Rate transactions
  - Review books and exchange experiences
- Ratings contribute to user trust score

---

## 4. Business Rules

- Users must create an account to interact with the system
- Each book listing must specify:
  - Transaction type (sell, buy, swap)
- Matching logic must:
  - Consider both swap and buy/sell scenarios
  - If book balance isn't equal users can offer money to cover the balance and complete transaction
- System must store:
  - Users
  - Book listings
  - Wishlist data

---

## 5. Data & Integration

- Use mock data APIs for book metadata retrieval
- Database will store:
  - User data
  - Book listings
  - Transactions
  - Reviews

---

## 6. Non-Functional Requirements

### Performance

- Fast search and matching responses

### Scalability

- Designed to support growing user base

### Security

- Secure authentication and user data protection

### Usability

- Simple and intuitive UI/UX

### Reliability

- Consistent matching and notification delivery

---

## 7. Out of Scope (Initial Version)

- Real payment integration (Stripe, PayPal)
- Integration with shipping providers
- Calculate shipping costs based on:
  - Location
  - Book weight/dimensions
- Real-time logistics tracking (basic shipping calculation only)
- AI-based recommendation system
- Mobile app version
- Advanced trust & fraud detection system
- Member vs Verified Member tiers

---

## 8. Success Criteria

- Users can register, authenticate, and manage their profiles
- Ratings and reviews are recorded and contribute to user reputation
  - Ratings and reviews are only available to registered members
- Books can be listed, searched, filtered by relevant attributes
- Books can be rated and reviewed
  - Book reviews are open to the public
- Matching engine successfully identifies swap and buy/sell opportunities
  - All users are altered when a match is found.
- Messaging between users is functional
- Role-based access control is enforced throughout the platform
- Core backend logic is covered by unit and integration tests
- Transactions can be proposed, negotiated, and completed end-to-end

---

## 9. Timeline

| Week       | Tasks                      |
| ---------- | -------------------------- |
| **Week 1** | Documentation Reviewed     |
| **Week 2** | Documentation Finalized    |
| **Week 3** | Back End Set Up & Testing  |
| **Week 4** | Front End Set Up & Testing |
| **Week 5** | First Release              |
| **Week 6** | Final Project              |

[Back to Top](#1-project-overview)
