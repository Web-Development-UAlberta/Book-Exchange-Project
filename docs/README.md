<a name="top"></a>

# 📚 Book Exchange Platform

> **Connect. Trade. Read More.**
> A community-driven marketplace for buying, selling, and swapping books — built with ASP.NET Core and powered by real book data.

---

## 📋 Table of Contents

- [What Is This?](#-what-is-this)
- [Tech Stack](#️-tech-stack)
- [Prerequisites](#-prerequisites)
- [Configuration & Secrets](#-configuration--secrets)
- [Getting Started](#-getting-started)
- [Running Tests](#-running-tests)
- [Project Structure](#-project-structure)
- [Key Routes](#-key-routes)
- [Project Usage](#-project-usage)
- [CI/CD](#-cicd)
- [Documentation](#-documentation)
- [Contributing](#-contributing)

---

## 🚀 What Is This?

The **Book Exchange Platform** is a full-stack web application that lets users list books they own, wishlist books they want, and get automatically matched with other readers for swaps, purchases, or sales. Think of it as a smart, social marketplace for book lovers.

Built on a clean three-tier architecture using **.NET 10**, **PostgreSQL**, and **ASP.NET Core MVC** — it's designed for reliability, testability, and straightforward local setup.

## [🔝 Back to Top](#top)

## 🛠️ Tech Stack

| Layer        | Technology                         |
| ------------ | ---------------------------------- |
| **Backend**  | ASP.NET Core MVC (.NET 10), C#     |
| **Frontend** | Razor Views, Bootstrap, JavaScript |
| **Database** | PostgreSQL                         |
| **ORM**      | Entity Framework Core              |
| **Auth**     | ASP.NET Identity                   |
| **Testing**  | xUnit, Microsoft Playwright        |
| **IDE**      | Visual Studio 2022+                |

## [🔝 Back to Top](#top)

## ✅ Prerequisites

Make sure you have the following installed before you begin:

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio 2022+](https://visualstudio.microsoft.com/) _(recommended)_ or VS Code with the C# extension
- [PostgreSQL](https://www.postgresql.org/download/)
- [PowerShell (`pwsh`)](https://github.com/PowerShell/PowerShell/releases) — required to install Playwright browsers for UI tests
- [Entity Framework Core CLI tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

```bash
dotnet tool install --global dotnet-ef
```

You will also need API keys for the following external services:

- **Google Books API** — for book metadata lookup by ISBN
- **Google Maps / Places API** — for address autocomplete and validation

## [🔝 Back to Top](#top)

## 🔑 Configuration & Secrets

This project uses .NET's [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) for local development. **Never commit API keys to source control.**

From the main application project directory, run:

```bash
dotnet user-secrets init
dotnet user-secrets set "GoogleBooks:ApiKey" "YOUR_GOOGLE_BOOKS_API_KEY"
dotnet user-secrets set "GoogleMaps:ApiKey" "YOUR_GOOGLE_MAPS_API_KEY"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=bookexchange;Username=postgres;Password=yourpassword"
```

## [🔝 Back to Top](#top)

## 💻 Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/Web-Development-UAlberta/Book-Exchange-Project
cd book-exchange-platform
```

### 2. Set up the database

Create a PostgreSQL database locally:

```sql
CREATE DATABASE bookexchange;
```

### 3. Apply migrations

```bash
dotnet ef database update
```

### 4. Run the application

```bash
dotnet run
```

The app will be available at `http://localhost:5261`.

---

## 🧪 Running Tests

The project has three layers of testing — **unit**, **integration**, and **UI (Playwright)** — all built with xUnit.

---

### Unit Tests

Unit tests use mocked dependencies and run in isolation with no external requirements.

```bash
dotnet test --filter "FullyQualifiedName~UnitTests"
```

---

### Integration Tests

Integration tests use an **in-memory database** and require no running PostgreSQL instance.

```bash
dotnet test --filter "FullyQualifiedName~Integration"
```

---

### 🎭 UI Tests (Playwright)

UI tests run against a live instance of the application at `http://localhost:5261`. The app **must be running** before executing UI tests.

#### Step 1 — Install Playwright browsers

After building the UI test project, run the Playwright install script to download the browser binaries:

```bash
# From the UI test project directory
dotnet build
pwsh bin/Debug/net10.0/playwright.ps1 install
```

To install only a specific browser:

```bash
pwsh bin/Debug/net10.0/playwright.ps1 install chromium
```

> 💡 **PowerShell required:** The install script requires `pwsh` (PowerShell Core), not the legacy `powershell.exe`.
>
> **macOS:** `brew install --cask powershell`
> **Linux:** See [Microsoft's install guide](https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-linux)
> **Windows:** Download from the [PowerShell GitHub releases](https://github.com/PowerShell/PowerShell/releases)

#### Step 2 — Test user accounts

When running in **Development**, these accounts are created automatically by the database seeder on first launch. If you are running tests against a non-Development environment or a fresh database without seed data, register these accounts manually in the running application before executing tests:

| Role                | Email                | Password    |
| ------------------- | -------------------- | ----------- |
| Primary test user   | `test@test.com`      | `Test1234!` |
| Secondary test user | `otheruser@test.com` | `Test1234!` |

#### Step 3 — Start the application

```bash
dotnet run
```

Confirm the app is accessible at `http://localhost:5261` before proceeding.

#### Step 4 — Run the UI tests

```bash
# Run all UI tests
dotnet test --filter "FullyQualifiedName~UI"

# Run tests for a specific area
dotnet test --filter "FullyQualifiedName~UI.ListingTests"
dotnet test --filter "FullyQualifiedName~UI.AddressTests"
dotnet test --filter "FullyQualifiedName~UI.WishlistTests"
dotnet test --filter "FullyQualifiedName~UI.ExchangeRequestTests"
dotnet test --filter "FullyQualifiedName~UI.TransactionTests"
dotnet test --filter "FullyQualifiedName~UI.BookSearchTests"
```

#### CI — GitHub Actions

For automated runs in CI, add a step to install Playwright browsers along with their OS-level dependencies:

```yaml
- name: Install Playwright browsers
  run: pwsh bin/Debug/net10.0/playwright.ps1 install --with-deps
```

The `--with-deps` flag installs required system libraries (fonts, graphics libs) needed for headless browsers on Linux runners.

## [🔝 Back to Top](#top)

## 📁 Project Structure

```
/
├── Controllers/          # MVC Controllers (Listings, Transactions, Messages, etc.)
├── Models/               # Domain models and DTOs
│   └── DTOs/             # Data Transfer Objects
├── Services/             # Business logic layer
│   └── Interfaces/       # Service contracts
├── Views/                # Razor views
├── Tests/
│   ├── Unit/             # xUnit unit tests (mocked dependencies)
│   ├── Integration/      # xUnit integration tests (in-memory database)
│   └── UI/               # Playwright UI tests (requires running app)
├── docs/
│   ├── DESIGN.md         # System architecture and design
│   ├── SCOPE.md          # Feature scope
│   ├── TEST-PLAN.md      # Test plan
│   └── ERD.png           # Entity Relationship Diagram
└── appsettings.json
```

## [🔝 Back to Top](#top)

## 🌐 Key Routes

| Area          | Route                        | Description                   |
| ------------- | ---------------------------- | ----------------------------- |
| Auth          | `/Identity/Account/Register` | Register a new user           |
| Auth          | `/Identity/Account/Login`    | Log in                        |
| Listings      | `/Listing`                   | Browse all listings           |
| Listings      | `/Listing/Create`            | Create a new listing          |
| Wishlist      | `/Wishlist`                  | View wishlist                 |
| Exchange      | `/ExchangeRequest`           | View exchange requests        |
| Transactions  | `/Transaction`               | View transaction history      |
| Messages      | `/Message`                   | View messages                 |
| Notifications | `/Notification`              | View notifications            |
| Addresses     | `/Addresses`                 | Manage saved addresses        |
| Book Search   | `/BookSearch`                | Search books via Google Books |

## [🔝 Back to Top](#top)

## 🎉 Project Usage

### First Run & Data Seeding

The application does not require manual data entry to get started. When running in the Development environment, the startup pipeline automatically applies any pending database migrations and seeds the database with sample data — but only if that data does not already exist (all seed operations are idempotent).

The following seed data is created on first run:

|                   |                                                                                                                                                                           |
| ----------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Category          | Details                                                                                                                                                                   |
| Users             | [test@test.com](mailto:test@test.com) / Test1234!, [otheruser@test.com](mailto:otheruser@test.com) / Test1234!, [noreview@test.com](mailto:noreview@test.com) / Test1234! |
| Addresses         | One address per seeded user (Edmonton & Calgary, using Google Place IDs)                                                                                                  |
| Carriers          | Canada Post, Purolator, FedEx Canada, UPS Canada                                                                                                                          |
| Listings          | 8–10 listings across both test users with various ISBNs, conditions, and prices                                                                                           |
| Wishlists         | ISBN-based wishlist entries for both test users, including cross-matches                                                                                                  |
| Exchange Requests | BuySell, BookSwap, and BookSwapWithCash requests in various states                                                                                                        |
| Transactions      | Transactions in Confirmed, Shipped, Completed, and Cancelled states                                                                                                       |
| Shipments         | Sample shipments linked to transactions                                                                                                                                   |
| Messages          | Sample message threads between test users                                                                                                                                 |
| Notifications     | Pre-seeded notifications for both users                                                                                                                                   |
| Reviews           | Reviews from completed transactions                                                                                                                                       |

In Production, the database starts empty. Users must register, add addresses, and create listings manually before any matching or exchange activity is possible.

### Registering & Setting Up Your Profile

1. Navigate to the application in your browser (default: [https://localhost:5261](https://localhost:55261)).
2. Click Register and create an account with your email and password.
3. After logging in, go to Profile to view your user page by clicking the down arrow next to your user name and select Profile.
4. Navigate to My Addresses and add a shipping address using the Google Places address lookup.
   ![Registration form](src/Book-Exchange/img/Register.png)
   ![Add an Address](src/Book-Exchange/img/Address.png)

### Creating a Listing

Listings represent books you own and are willing to sell or swap.

1. From the navigation bar, click Listings → Create Listing.
2. Enter the book's Author or title. Book metadata (title, author, cover, ISBN) is fetched automatically from the Google Books API.
3. Set the condition, price, and weight (in grams).
4. Submit the form. Your listing will appear under My Listings and be visible in the public book search.

![Listings page](src/Book-Exchange/img/Listings.png)
![Create a new listing](src/Book-Exchange/img/Create-Listing.png)

### Managing Your Wishlist

The wishlist tracks books you want. The matching engine uses it to notify you when a matching listing appears.

1. Go to Wishlist → Add to Wishlist.
2. Enter the Book Title or Author of the book you're looking for.
3. Your wishlist items can be toggled to be removed or restored at any time.

   ![Wishlist page with items](src/Book-Exchange/img/Wishlist.png)

### Match Suggestions

When a listing matches a book in your wishlist (or vice versa), the platform sends you a Match Found notification and surfaces the notification on your dashboard.

1. Check your Notifications for alerts.
2. Click a match notification and then view the listing to message the listing owner or create an exchange request.

   ![Match notification](src/Book-Exchange/img/Match-Notification.png)

### Submitting an Exchange Request

1. From a listing's detail page, click Create an Exchange Request.
2. Select up to 3 books to swap with the listing owner, offer cash only or combine cash and books.
3. Optionally message to the listing owner.
4. Submit. The listing owner will be notified.

   ![Create an exchange request](src/Book-Exchange/img/Create-Exchange)

### Accepting or Rejecting Requests (Listing Owner)

1. Go to Exchange Requests and click on a listing with a pending request.
2. Review the incoming exchange requests by clicking details.
3. Click Accept to create a transaction, or Reject to decline.

Only one request can be accepted per listing. Accepting automatically moves the listing to Pending status and creates a Transaction.

![Pending Exchnage Request](src/Book-Exchange/img/Offered-Exchange.png)

### Transactions & Shipping

Once an exchange request is accepted, a Transaction is created.

1. Navigate to Transactions to see your active and past transactions.
2. Navigate to shipping to see the quoted shipping estimates.

- A Single Transaction will quote multiple shipping options.
- Review the options by viewing the details.
- Select the best one by canceling the others.

4. Update the shipment status as the book moves through delivery.

   ![Shipping options grid](src/Book-Exchange/img/Shipping-table.png)
   ![Shipping details page](src/Book-Exchange/img/shipment-details.ong)

### Reviews

After a transaction is marked Completed, both parties can leave a review.

1. Go to Transactions → History and find the completed transaction you want to review.
2. Click Leave a Review, provide a rating (1–5) and an optional comment.
3. Reviews appear on the other user's public profile and contribute to their reputation score.

   ![Review submission form](src/Book-Exchange/img/review-form.png)
   ![Profile with reviews](src/Book-Exchange/img/Profile-Reviews.png)

## [🔝 Back to Top](#top)

## 🔄 CI/CD

This project uses **GitHub Actions** for automated builds and test runs.

- Pull requests to `main` require **1 approver**
- Direct pushes to `main` are **blocked**
- GitHub Actions runs all tests on every PR and push to `main`

## [🔝 Back to Top](#top)

## 📄 Documentation

| Document      | Location              |
| ------------- | --------------------- |
| System Design | `./docs/DESIGN.md`    |
| Feature Scope | `./docs/SCOPE.md`     |
| Test Plan     | `./docs/TEST-PLAN.md` |
| ERD           | `./docs/ERD.png`      |
| Wireframes    | `./docs/wireframes/`  |

## [🔝 Back to Top](#top)

## 🤝 Contributing

1. Branch off `main` using the naming convention `feature/your-feature-name`
2. Write or update tests for any new logic
3. Open a pull request — **1 approver required** before merging
4. Ensure all GitHub Actions checks pass before requesting review

## [🔝 Back to Top](#top)

> Built with ☕ and a love of books.
