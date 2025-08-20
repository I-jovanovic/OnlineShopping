# OnlineShopping API

A RESTful API for an online shopping platform built with ASP.NET Core 8.0.

## Solution Structure

- **OnlineShopping.Core** - Domain entities, business logic, and interfaces
- **OnlineShopping.Infrastructure** - Data access, external services, and infrastructure concerns
- **OnlineShopping** - Web API project with controllers and API configuration
- **OnlineShopping.Tests** - Unit and integration tests

## Technologies

- ASP.NET Core 8.0
- Entity Framework Core
- Swagger/OpenAPI
- xUnit

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB or full instance)

### Running the API

```bash
cd OnlineShopping
dotnet run
```

The API will be available at:
- https://localhost:5001
- http://localhost:5000

Swagger UI: https://localhost:5001/swagger

### Running Tests

```bash
dotnet test
```

## API Endpoints

- `GET /health` - Health check endpoint