# 📚 Book Exchange Platform

> **Connect. Trade. Read More.**
> A community-driven marketplace for buying, selling, and swapping books — built with ASP.NET Core and powered by real book data.

---

## 🚀 What Is This?

The **Book Exchange Platform** is a full-stack web application that lets users list books they own, wishlist books they want, and get automatically matched with other readers for swaps, purchases, or sales. Think of it as a smart, social marketplace for book lovers.

Built on a clean three-tier architecture using **.NET 10**, **PostgreSQL**, and **ASP.NET Core MVC** — it's designed for reliability, testability, and straightforward local setup.

---

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

---

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

---

## 🔑 Configuration & Secrets

This project uses .NET's [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) for local development. **Never commit API keys to source control.**

From the main application project directory, run:

```bash
dotnet user-secrets init
dotnet user-secrets set "GoogleBooks:ApiKey" "YOUR_GOOGLE_BOOKS_API_KEY"
dotnet user-secrets set "GoogleMaps:ApiKey" "YOUR_GOOGLE_MAPS_API_KEY"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=bookexchange;Username=postgres;Password=yourpassword"
```

---

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

#### Step 2 — Create test user accounts

UI tests authenticate using two pre-existing accounts. Register these users in the running application before executing tests:

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

---

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

---

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

---

## 🔄 CI/CD

This project uses **GitHub Actions** for automated builds and test runs.

- Pull requests to `main` require **1 approver**
- Direct pushes to `main` are **blocked**
- GitHub Actions runs all tests on every PR and push to `main`

---

## 📄 Documentation

| Document      | Location              |
| ------------- | --------------------- |
| System Design | `./docs/DESIGN.md`    |
| Feature Scope | `./docs/SCOPE.md`     |
| Test Plan     | `./docs/TEST-PLAN.md` |
| ERD           | `./docs/ERD.png`      |
| Wireframes    | `./docs/wireframes/`  |

---

## 🤝 Contributing

1. Branch off `main` using the naming convention `feature/your-feature-name`
2. Write or update tests for any new logic
3. Open a pull request — **1 approver required** before merging
4. Ensure all GitHub Actions checks pass before requesting review

---

> Built with ☕ and a love of books.
