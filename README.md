# AuthTask

AuthTask is an ASP.NET Core Web API for authentication and employee management.

## Projects

- `AuthTask`: Web API host
- `AuthTask.Application`: application services and DTOs
- `AuthTask.Domain`: domain entities
- `AuthTask.Infrastructure`: EF Core data access and repositories
- `AuthTask.UnitTests`: test project

## Prerequisites

- .NET SDK 9.0+
- PostgreSQL

## Run Locally

```bash
dotnet restore AuthTask.slnx
dotnet build AuthTask.slnx
dotnet run --project AuthTask/AuthTask.csproj
```

## OpenAPI

In development, OpenAPI is available at:

- `/openapi/v1.json`

Static reference spec:

- `AuthTask/swagger.yml`
