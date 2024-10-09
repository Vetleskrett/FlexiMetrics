# FlexiMetrics

## Prerequisites

Before you can set up the project locally, ensure you have the following prerequisites installed on your system:

```
dotnet
docker
```

## Setup Locally

Follow these steps to set up and run FlexiMetrics on your local machine:

### Clone the Repository

```
git clone https://github.com/Vetleskrett/FlexiMetrics.git
cd FlexiMetrics
```

## Backend

The backend consists of a ASP.NET Core Web API, a PostgreSQL database and .NET Aspire to orchestrate and monitor the projects.

### Running the backend
```
cd Backend
dotnet run --project AppHost/AppHost.csproj
```

or open the `Backend.sln` file in Visual Studio 2022.

### Solution structure
 - `AppHost`: The .NET Aspire orchestration startup project.
 - `Api`: The ASP.NET Core Web API with all business logic.
 - `Database`: The `AppDbContext`, the database models and the migrations.
 - `MigrationService`: A worker service for automatically applying migrations on startup.
 - `ServiceDefaults`: Common configuration for the other services. 

### Add migration
Make sure you have `Entity Framework Core tools` installed. To install run the following command:
```
dotnet tool install --global dotnet-ef
```

To update the database schema after changing the `AppDbContext` and/or its models run the following command:

```
cd Backend/Api
dotnet ef migrations add NEW_MIGRATION_NAME_HERE --project ..\Database\Database.csproj
```

This will add a migration to the `Database/Migrations` folder.
The `MigrationService` will automatically apply these changes to the database on next startup.