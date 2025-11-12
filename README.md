# **Business Card Manager -- Backend API**

A clean, production-ready **.NET Web API** backend for managing business
cards with full CRUD, import/export (CSV/XML), QR decoding, and a
layered architecture following modern best practices.

------------------------------------------------------------------------

# 📦 **Tech Stack**

  - Framework    | .NET (Minimal Hosting Model)
  - Database     | PostgreSQL (EF Core + Npgsql)
  - Mapping      | AutoMapper
  - Logging      | NLog
  - QR Decoder   | ZXing
  - API Docs     | Swagger / OpenAPI
  - Tests        | xUnit

------------------------------------------------------------------------

# 📁 **Solution Structure**

    BusinessCardManager.sln
    │
    ├── BusinessCardManager.API/            # Controllers, Middleware, DI, Swagger
    ├── BusinessCardManager.Application/    # Services, DTOs, Interfaces, Mapping, Responses
    ├── BusinessCardManager.Domain/         # Entities, Enums, Constants
    ├── BusinessCardManager.Infrastructure/ # EF Core, DbContext, Migrations, Repositories, UoW
    └── BusinessCardManager.Tests/          # Unit tests (xUnit)

------------------------------------------------------------------------

# 🏛 **Architecture Overview**

    API Layer
       ↓ (Controllers)
    Application Layer
       ↓ (Business services, DTO mapping)
    Infrastructure Layer
       ↓ (EF Core, PostgreSQL, Repositories, Migrations)
    Domain Layer

✅ Clean Separation of Concerns\
✅ Testable Architecture\
✅ Fully async pipeline

------------------------------------------------------------------------

# 🚀 **Features**

### ✅ Business Card CRUD

-   Create
-   List with filters
-   Delete
-   Bulk Create

### ✅ Importing

-   CSV / XML file preview
-   Validation for missing fields
-   Returns valid + invalid rows

### ✅ Exporting

-   Export filtered results to CSV
-   Export filtered results to XML

### ✅ QR Import

-   Upload QR image
-   Decode payload using `ZXing`
-   Convert to `BusinessCardRequestDto`

### ✅ Global Response Wrapper

Every endpoint returns a unified structure:

``` json
{
  "status": "success",
  "message": "Successfully retrieved 5 business card(s).",
  "data": { },
  "errors": [],
  "timeGenerated": "2025-01-01T00:00:00Z"
}
```

------------------------------------------------------------------------

# 🧱 **Domain Model**

### **BusinessCard**

    Id (int)
    Name (string)
    Gender (string?)
    DateOfBirth (DateOnly?)
    Email (string)
    Phone (string)
    Photo (string?)     // Base64 string
    Address (string?)
    CreatedAt (DateTime)
    UpdatedAt (DateTime?)

Configured in:\
`BusinessCardManager.Infrastructure/Persistence/Configurations/BusinessCardConfiguration.cs`

------------------------------------------------------------------------

# 🗄 **Database & EF Core**

### DbContext

`ApplicationDbContext`
- Auto timestamps (CreatedAt / UpdatedAt)
- Applies configurations from assembly
- PostgreSQL provider via Npgsql

### Migrations

Found in:\
`BusinessCardManager.Infrastructure/Migrations/`

- Initial migration creates: 
    - ✔ Table
    - ✔ PK
    - ✔ Constraints (unique Email & Phone)

------------------------------------------------------------------------

# 🔥 **API Endpoints**

## **Business Cards**

**Base Route:** `/api/business-cards`

| Method | Route     | Description            |
|--------|-----------|------------------------|
| GET    | `/`       | List cards + filtering |
| POST   | `/bulk`   | Bulk Create            |
| POST   | `/`       | Create business card   |
| DELETE | `/{id}`   | Delete card            |

### Available Filters:

`name, email, phone, gender, dateOfBirth`

------------------------------------------------------------------------

## **Import**

**Base:** `/api/business-cards/import`

### POST `/preview`

-   Accepts CSV or XML
-   Returns:
    ```{=html}
    totalRows
    validRows
    invalidRows
    errors[]
    cards[]
    ```

------------------------------------------------------------------------

## **Export**

**Base:** `/api/business-cards/export`

| Method | Route     | Returns       |
|--------|-----------|---------------|
| GET    | `/csv`    | CSV file      |
| GET    | `/xml`    | XML file      |

Filters are supported (same as list).

------------------------------------------------------------------------

## **QR Import**

**POST `/api/qr/import`**

- Accepts:\
`file` = qr-image.png/jpg

- Returns decoded fields:
   ```txt
    name, gender, email, phone, address, dateOfBirth
   ```
------------------------------------------------------------------------

# ⚙️ **Configuration**

### appsettings.Development.json

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=business_card_db;Username=postgres;Password=123"
}
```
> **Note:** You should replace the connection string values with **your own database credentials** before using it.


### Enable Swagger

Automatic in Development environment.

### CORS

Policy: `AllowAngularFrontend`

Connects perfectly with your Angular front-end.

------------------------------------------------------------------------

# 🏗 **Running the Project**

### 1️⃣ Install Dependencies

-   .NET SDK 8+
-   PostgreSQL

### 2️⃣ Update DB Connection

`BusinessCardManager.API/appsettings.Development.json`

### 3️⃣ Apply Migrations

Run from Infrastructure project:

``` bash
dotnet ef database update --project BusinessCardManager.Infrastructure --startup-project BusinessCardManager.API
```

### 4️⃣ Run API

``` bash
dotnet run --project BusinessCardManager.API
```

API default URL:\
✅ `https://localhost:7060`

Swagger UI:\
✅ `https://localhost:7060/swagger`

------------------------------------------------------------------------

# ✅ **Testing**

Uses **xUnit**.

Test suites:
- BusinessCardServiceTests
- ImportServiceTests
- ExportServiceTests

Mocks: 
- InMemoryRepository
- InMemoryUnitOfWork

------------------------------------------------------------------------

# 📤 **cURL Examples**

### Create

``` bash
curl -X POST "https://localhost:7060/api/business-cards" \
  -H "Content-Type: application/json" \
  -d '{"name":"Ali Ahmad","email":"ali@example.com","phone":"+9627000000"}'
```

### List

``` bash
curl "https://localhost:7060/api/business-cards?name=ali"
```

### Delete

``` bash
curl -X DELETE "https://localhost:7060/api/business-cards/5"
```

### Import Preview

``` bash
curl -F "file=@cards.csv" https://localhost:7060/api/business-cards/import/preview
```

------------------------------------------------------------------------

# ✨ **Author**

*Baha'aldin Hammad - ProgressSoft Task*\
Business Card Manager Backend\
2025
