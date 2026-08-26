# eBookStore

eBookStore is a full-stack e-book marketplace built with ASP.NET Core MVC. It lets users browse books, manage a shopping cart, complete Stripe-powered checkouts, rent books, and keep purchased or borrowed titles in a personal library.

## Features

- User registration, sign-in, and session-based authentication
- Book catalog and gallery browsing
- Shopping cart and checkout workflow
- Stripe payment integration
- Personal library for purchased and borrowed books
- Book rental queue and borrowing history
- Ratings and feedback
- Discounts, sale end-date checks, and purchase history
- SQL Server database initialization
- Docker-based development environment with hot reload

## Tech stack

- ASP.NET Core MVC on .NET 8
- C# and Razor views
- Microsoft SQL Server / Azure SQL Edge
- Microsoft.Data.SqlClient and Entity Framework Core
- Stripe.net
- Docker and Docker Compose

## Project structure

```text
.
|-- db/                         # Database schema
|-- db-init/                    # Database initialization container
|-- eBookStore/
|   |-- Controllers/            # MVC controllers
|   |-- Models/                 # Domain and view models
|   |-- Repository/             # Data-access layer
|   |-- Services/               # Background and application services
|   |-- Views/                  # Razor views
|   |-- wwwroot/                # Static assets
|   |-- Program.cs              # Application startup
|   `-- eBookStore.csproj
|-- Dockerfile
|-- compose.yaml
`-- eBookStore.sln
```

## Prerequisites

For the Docker workflow:

- Docker Desktop with Docker Compose

For local development:

- .NET 8 SDK
- SQL Server
- A Stripe test account for payment testing

## Run with Docker

1. Clone the repository:

   ```bash
   git clone https://github.com/oryona97/eBookStore.git
   cd eBookStore
   ```

2. Create a `.env` file in the repository root and set a strong SQL Server password:

   ```dotenv
   DB_PASSWORD=replace-with-a-strong-password
   ```

3. Build and start the application and database:

   ```bash
   docker compose up --build
   ```

4. Open `https://localhost:5282` in your browser.

The database initialization service waits for SQL Server and then runs `db/schema.sql`. The current Compose configuration targets Linux ARM64; if your machine uses another architecture, remove or adjust the `platform` entries in `compose.yaml`.

## Run locally

1. Clone the repository and restore the dependencies:

   ```bash
   git clone https://github.com/oryona97/eBookStore.git
   cd eBookStore
   dotnet restore eBookStore.sln
   ```

2. Create the `eBookStore` database and run `db/schema.sql` against it.

3. Configure the database connection and Stripe test credentials. For development, use environment variables or .NET user secrets instead of committing credentials:

   ```bash
   dotnet user-secrets init --project eBookStore/eBookStore.csproj
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=eBookStore;User Id=SA;Password=YOUR_PASSWORD;TrustServerCertificate=True" --project eBookStore/eBookStore.csproj
   dotnet user-secrets set "Stripe:SecretKey" "YOUR_STRIPE_TEST_SECRET_KEY" --project eBookStore/eBookStore.csproj
   dotnet user-secrets set "Stripe:PublicKey" "YOUR_STRIPE_TEST_PUBLIC_KEY" --project eBookStore/eBookStore.csproj
   ```

4. Trust the ASP.NET Core development certificate if needed:

   ```bash
   dotnet dev-certs https --trust
   ```

5. Start the application:

   ```bash
   dotnet run --project eBookStore/eBookStore.csproj
   ```

6. Open `https://localhost:5282` in your browser.

## Configuration

The application reads these configuration values:

| Setting | Purpose |
| --- | --- |
| `ConnectionStrings:DefaultConnection` | SQL Server connection string |
| `Stripe:SecretKey` | Stripe server-side test key |
| `Stripe:PublicKey` | Stripe client-side test key |

ASP.NET Core environment-variable names use double underscores, for example `Stripe__SecretKey`.

## Security

Do not commit passwords or Stripe secret keys. Before publishing or deploying this project, rotate any credentials that have previously been committed, remove them from tracked configuration files, and load them from environment variables, .NET user secrets, or a dedicated secret manager.

## Contributing

Contributions are welcome. Fork the repository, create a focused branch, and open a pull request with a clear description of your changes.
