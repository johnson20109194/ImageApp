# 📸 ImageApp (ASP.NET Core)

ImageApp is a cloud-native media sharing backend built with **ASP.NET Core**.  
It provides REST endpoints for authentication, photo browsing and uploads (creator-only), commenting, and rating.  
The backend integrates **MySQL**, **Redis**, and **Azure Blob Storage** (Azurite for local development).

---

## Table of Contents

- [Architecture](#architecture)
- [Tech Stack](#tech-stack)
- [Solution Structure](#solution-structure)
- [Local Development with Docker Compose](#local-development-with-docker-compose)
- [Configuration](#configuration)
- [Database Migrations](#database-migrations)
- [Seeded Users](#seeded-users)
- [API Endpoints](#api-endpoints)
- [Troubleshooting](#troubleshooting)

---

## Architecture

High-level architecture:


| HTTPS (REST)
v
+----------------------+
| ImageApp |
| ASP.NET Core |
+----------+----------+
|
+-------+-------+----------------+
| | |
v v v
MySQL Redis Azure Blob
(Relational) (Cache) Storage
(Azurite)


### Architectural Principles

- Stateless API secured with JWT
- Clean Architecture separation:
    - **Domain** – entities and core rules
    - **Application** – services, DTOs, use cases
    - **Infrastructure** – EF Core, Redis, Blob Storage
    - **API** – controllers, middleware, authentication
- Object storage for media
- Relational database for integrity and consistency

---

## Tech Stack

### Backend
- ASP.NET Core Web
- Entity Framework Core
- MySQL 8 (Pomelo EF Core Provider)
- Redis (StackExchange.Redis via `IDistributedCache`)
- Azure Blob Storage SDK
- JWT Authentication & Role-based Authorization

### Infrastructure
- Docker & Docker Compose
- Azurite (Azure Storage Emulator)

---

## Solution Structure

src/
├── ImageApp.Api/ # Controllers, middleware, Program.cs
├── ImageApp.Application/ # Application services, DTOs, interfaces
├── ImageApp.Domain/ # Entities, enums, domain rules
└── ImageApp.Infrastructure/ # EF Core, caching, blob storage, persistence

---

## Configuration

### Configuration is loaded from:
- appsettings.json
- appsettings.Development.json
- Environment variables (Docker Compose)
- User Secrets (optional)

### Required Environment Variables (Docker)
- Database
  - ConnectionStrings__DbConnection
- Redis
  - ConnectionStrings__RedisConnection
- Blob Storage
  - Storage__ConnectionString
  - Storage__OriginalContainer 
  - Storage__ThumbnailContainer 
  - Storage__SasTtlMinutes

### Redis Connection String (correct format)
```text
local-cache:6379,abortConnect=false,allowAdmin=true
```

### Azurite Connection String (Docker network)
```text
DefaultEndpointsProtocol=http;
AccountName=devstoreaccount1;
AccountKey=<FULL_AZURITE_ACCOUNT_KEY>;
BlobEndpoint=http://azurite:10000/devstoreaccount1;
```

### Important:
- Do not truncate the Azurite account key.
- When running inside Docker, do not use localhost for Azurite.

---

## Database Migrations

### Using `dotnet ef` (recommended)

Install EF CLI (once):
```bash
dotnet tool install --global dotnet-ef
```

### Create migration:
```bash
dotnet ef migrations add InitialCreate \
--project src/ImageApp.Infrastructure \
--startup-project src/ImageApp.Api
```

### Apply migration:
```bash
dotnet ef database update \
  --project src/ImageApp.Infrastructure \
  --startup-project src/ImageApp.Api
```

### Using Visual Studio (Package Manager Console)
```powershell
Add-Migration InitialCreate
Update-Database
```

---

## Seeded Users

| Role    | Email                                                       | Password    |
| ------- | ----------------------------------------------------------- | ----------- |
| Admin   | [admin@ImageApp.local](mailto:admin@ImageApp.local)     | Admin@123   |
| Creator | [creator@ImageApp.local](mailto:creator@ImageApp.local) | Creator@123 |


### These are intended for local development only.

---

## API Endpoints

### Base URL:
```aeduino
http://localhost:8100
```
### Authentication
- POST /api/auth/register
- POST /api/auth/login
- GET /api/auth/me (authenticated)

### Photos
- GET /api/photos – public feed / search
- GET /api/photos/{photoId}

### Creator-only:
- POST /api/photos – upload photo

### Comments
- GET /api/photos/{photoId}/comments
- POST /api/photos/{photoId}/comments

### Ratings
- GET /api/photos/{photoId}/ratings/summary
- POST /api/photos/{photoId}/ratings

---

## Troubleshooting

### Redis Errors
- Unknown key Server or boolean parsing errors:
    - Use comma-separated StackExchange.Redis format
    - Remove all semicolons

### Azurite Errors
- No valid combination of account information found
    - Ensure full Azurite account key
    - Ensure correct config key (Storage:ConnectionString)
    - Ensure Docker service name (azurite) is used

### MySQL Migration Errors
- Common SQL Server artifacts that must be removed for MySQL:
    - SYSUTCDATETIME() → use UTC_TIMESTAMP(6) or set in code
    - nvarchar(max) → use longtext or json
    - [Score] → use Score or `Score`

---

## Notes

- Redis cache failures degrade gracefully
- Blob containers are created automatically at runtime
- The backend is fully containerized and cloud-ready
- Designed for Azure deployment but portable across clouds