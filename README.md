# B2B Subscription Management System

## Overview

A microservices-based .NET 8 solution for managing B2B subscriptions, licenses, and payments. Built with clean architecture principles for scalability and maintainability.

## Core Features

- User Management & Authentication
- Subscription Plans & Management
- License Assignment & Tracking
- Payment Processing Integration
- Microservices Architecture with Dedicated Databases

## Tech Stack

- .NET 8
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- Swagger/OpenAPI
- Docker Support

## Project Structure

```
src/
├── Core/ # Domain models & business logic
├── Infrastructure/ # Data access & external services
└── API/ # API endpoints & controllers
```

## Getting Started

### Prerequisites

- .NET SDK 8.x
- SQL Server
- Docker (optional)

### Setup

1. **Clone Repository**

```bash
git clone https://github.com/monica-ty/B2B-Subscription.git
cd B2B-Subscription
```

2. **Configure Databases**

Update connection strings in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "UserConnection": "Server=...;Database=UserDB;...",
    "SubscriptionConnection": "Server=...;Database=SubscriptionDB;...",
    "PaymentConnection": "Server=...;Database=PaymentDB;...",
    "LicenseConnection": "Server=...;Database=LicenseDB;..."
  }
}
```

3. **Run Migrations**

```bash
dotnet ef database update -c UserDbContext
dotnet ef database update -c SubscriptionDbContext
dotnet ef database update -c PaymentDbContext
dotnet ef database update -c LicenseDbContext
```

4. **Start Application**

```bash
dotnet run --project src/API
```

Visit `http://localhost:5215/swagger` for API documentation.

## Microservices Architecture

- User Service (Authentication & Profiles)
- Subscription Service (Plan Management)
- License Service (License Management)
- Payment Service (Payment Processing)

## Contributing

1. Fork repository
2. Create feature branch
3. Submit pull request

## License

MIT License

For detailed API documentation and deployment guides, see `/docs` directory.
