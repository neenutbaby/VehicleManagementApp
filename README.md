# VehicleManagement

This project holds the CRUD operations related to vehicles and vehicle categories based on their weights using ASP.NET, MVC EF Core & SQL Server 2022

## Technology

- .NET 8 / ASP.NET Core MVC
- C#
- Entity Framework Core
- SQL Server 2022
- xUnit

## Database setup

Two ways to create the database ; one using the sql script and the other is using EF Core

### Option A — Let EF Core create/update the database

The project contains an EF Core migration under:

`VehicleManagement.Web/Data/Migrations/`

On application startup, `DatabaseInitializer` calls `Database.MigrateAsync()` and then inserts the initial manufacturers/categories when they are missing.

### Option B — Run the SQL script manually

1. Install SQL Server 2022
2. Open SQL Server Management Studio (SSMS).
3. Open `Database/setup.sql`.
4. Run the entire script.
5. The script creates the `VehicleManagement` database, tables, constraints and initial seed data.
6. Update the connection string in `VehicleManagement.Web/appsettings.json`
7. Run the application.

## Database design

- `Manufacturers` stores the predefined manufacturer list separately ; The predefined Manufacture list is stored in the db for future implementation for manufacture administration.
- `Vehicles` stores owner, manufacturer, year and weight.
- `VehicleCategories` stores the configurable weight ranges and icons.

## Connection string in appsettings.json

"ConnectionStrings": {
"VehicleManagementConnection": "Server=(localdb)\\MSSQLLocalDB;Database=VehicleManagement;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}

## Category boundary rule

Initial Range configuration:

- Light: `[0, 500)`
- Medium: `[500, 2500)`
- Heavy: `[2500, infinity)`

Therefore:

- `499.99` → Light
- `500.00` → Medium
- `2499.99` → Medium
- `2500.00` → Heavy

The application validates that category ranges start at zero, have no gaps, have no overlaps.

## Testing

The test project covers the important vehicle validation and category business rules, including:

- normal category determination;
- 500 kg and 2500 kg boundaries;
- gaps;
- overlaps;
