# Stark Customer Management system

## Business Requirements
- Manage business, residential, government and educator types of customers
- Manage our products
- Accept orders from customer online or through our call center

## Implementation details
**Stages**
- Identifying classes
- Separating responsabilities
- Establishing relationships
- Leveraging reuse

**Relationships**
1. Customer Repository uses a Customer (Collaboration)
2. Order has a Customer/Order Item/Address (Composition)
    - Aggregation: Order => Customer
    - Composition: Order => Order Item
3. Customer is a Business/Residential/Educator (Inheritance)

**Generators**
Using Bogus, all essential entities now have a way to generate stub information.
There are 2 approaches:
- Use the Data Builder to Build stubs for all entities.
- Use the Partial Data Builder to build stubs for a specific entity and its dependencies.

**LINQ Playground**
Small proyect that uses Data generator and writes a CSV file. The objective is to practice different LINQ statement; 
you can choose if you want to use the CSV content or generate new information.

## 🚀 Commands

### Project Commands

#### Build and Run Application

```bash
# Compile the project
dotnet build LINQ.Playground/LINQ.Playground.csproj

# Run application with data generation and database population
cd LINQ.Playground
echo -e "1\n/tmp\n1" | dotnet run

# Run application with data generation only (no database)
cd LINQ.Playground
echo -e "1\n/tmp\n2" | dotnet run

# Run application with saved data
cd LINQ.Playground
echo -e "2\n/tmp\n2" | dotnet run
```

#### Data Management

```bash
# Generate new data and save to CSV
cd LINQ.Playground
echo -e "1\n/tmp\n2" | dotnet run

# Use existing CSV data
cd LINQ.Playground
echo -e "2\n/tmp\n2" | dotnet run
```

### Entity Framework Core - Database Management

#### Migration Commands

##### Create Migrations

```bash
# Compile first
dotnet build LINQ.Playground/LINQ.Playground.csproj

# Create initial migration
dotnet ef migrations add InitialCreate --project Stark.BL --startup-project LINQ.Playground

# Create new migration
dotnet ef migrations add MigrationName --project Stark.BL --startup-project LINQ.Playground
```

##### Manage Migrations

```bash
# Compile first
dotnet build LINQ.Playground/LINQ.Playground.csproj

# Remove last migration
dotnet ef migrations remove --project Stark.BL --startup-project LINQ.Playground

# List all migrations
dotnet ef migrations list --project Stark.BL --startup-project LINQ.Playground
```

##### Update Database

```bash
# Compile first
dotnet build LINQ.Playground/LINQ.Playground.csproj

# Apply all pending migrations
dotnet ef database update --project Stark.BL --startup-project LINQ.Playground

# Apply specific migration
dotnet ef database update MigrationName --project Stark.BL --startup-project LINQ.Playground

# Revert to previous migration
dotnet ef database update PreviousMigration --project Stark.BL --startup-project LINQ.Playground
```

##### Database Management

```bash
# Compile first
dotnet build LINQ.Playground/LINQ.Playground.csproj

# Drop database completely
dotnet ef database drop --project Stark.BL --startup-project LINQ.Playground

# Get database information
dotnet ef dbcontext info --project Stark.BL --startup-project LINQ.Playground
```

##### Generate SQL Scripts

```bash
# Compile first
dotnet build LINQ.Playground/LINQ.Playground.csproj

# Generate script in console for all migrations
dotnet ef migrations script --project Stark.BL --startup-project LINQ.Playground

# Generate .sql file with all migrations
dotnet ef migrations script --project Stark.BL --startup-project LINQ.Playground --output migrations.sql

# Generate script for specific migration
dotnet ef migrations script InitialMigration FinalMigration --project Stark.BL --startup-project LINQ.Playground
```
