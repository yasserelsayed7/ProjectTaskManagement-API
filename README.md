# Project & Task Management API

Enterprise-grade **ASP.NET Core Web API** built with **.NET 9**, **Clean Architecture**, **CQRS + MediatR**, **JWT Authentication**, **EF Core**, **SQL Server**, **Redis**, **Docker**, and **xUnit** tests.

---

## Solution Structure

```
ProjectTaskManagement/
├── src/
│   ├── ProjectTaskManagement.Domain/          # Entities, Enums, Interfaces, Domain Exceptions
│   ├── ProjectTaskManagement.Application/     # CQRS, DTOs, Validators, Behaviors
│   ├── ProjectTaskManagement.Infrastructure/  # EF Core, Repositories, JWT, Redis, Seeding
│   └── ProjectTaskManagement.API/             # Controllers, Middleware, Swagger, Program.cs
├── tests/
│   └── ProjectTaskManagement.UnitTests/       # xUnit + Moq + FluentAssertions
├── Dockerfile
├── docker-compose.yml
└── ProjectTaskManagement.sln
```

---

## Architecture

This solution follows **Clean Architecture (Onion Architecture)**:

| Layer | Responsibility |
|-------|----------------|
| **Domain** | Core business entities and rules. No external dependencies. |
| **Application** | Use cases via CQRS (MediatR), validation (FluentValidation), DTOs, abstractions. |
| **Infrastructure** | EF Core, SQL Server, Redis cache, JWT token generation, repositories. |
| **API** | HTTP endpoints, authentication middleware, Swagger, versioning, health checks. |

**Dependency rule:** outer layers depend on inner layers — never the reverse.

**Patterns used:**
- CQRS + MediatR
- Repository + Unit of Work
- Pipeline Behaviors (Validation, Logging)
- Generic API Response Wrapper
- Global Exception Middleware
- JWT Bearer Authentication
- Role-based Authorization (`Admin`, `User`)

---

## Highlights (Production-Ready)

- **Version-based Redis cache invalidation** — project lists stay consistent after mutations
- **Database-level pagination** — efficient `ProjectReadRepository` and `TaskReadRepository`
- **EF Core retry policy** — resilient SQL Server connections
- **Admin dashboard API** — `GET /api/v1/admin/stats` (Admin role only)
- **10 unit tests** — validators + handlers with Moq
- **Swagger enabled in Docker** — test via `http://localhost:5000/swagger`
- **`.http` file** — ready-to-run REST Client requests in VS Code / Rider

## Technologies

- .NET 9 / ASP.NET Core Web API
- Entity Framework Core 9 + SQL Server
- JWT Authentication + BCrypt password hashing
- MediatR + FluentValidation
- Redis distributed caching (with in-memory fallback)
- Serilog
- Swagger / OpenAPI with JWT support
- API Versioning (v1)
- Health Checks
- Docker + docker-compose
- xUnit, Moq, FluentAssertions

---

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server) (or Docker)
- [Redis](https://redis.io/) (optional — falls back to in-memory cache)
- [Docker Desktop](https://www.docker.com/) (optional)

---

## Quick Start (Local)

### 1. Clone and restore

```bash
git clone <your-repo-url>
cd assessment
dotnet restore
```

### 2. Update connection string

Edit `src/ProjectTaskManagement.API/appsettings.Development.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=ProjectTaskManagementDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;",
  "Redis": "localhost:6379"
}
```

### 3. Apply migrations

```bash
dotnet ef database update --project src/ProjectTaskManagement.Infrastructure --startup-project src/ProjectTaskManagement.API
```

### 4. Run API

```bash
dotnet run --project src/ProjectTaskManagement.API
```

Open Swagger: `https://localhost:7xxx/swagger` (see `launchSettings.json` for port).

---

## Docker

```bash
docker-compose up --build
```

| Service | URL |
|---------|-----|
| API | http://localhost:5000 |
| Swagger | http://localhost:5000/swagger |
| Health | http://localhost:5000/health |
| SQL Server | localhost:1433 |
| Redis | localhost:6379 |

Migrations and seed data run automatically on startup.

---

## Seed Users

| Email | Password | Role |
|-------|----------|------|
| admin@projecttask.com | Admin@123 | Admin |
| user@projecttask.com | User@123 | User |

---

## API Endpoints (v1)

Base URL: `/api/v1`

### Authentication (Public)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/auth/register` | Register new user |
| POST | `/auth/login` | Login and receive JWT |

### Projects (Authorized)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/projects` | Create project |
| GET | `/projects?pageNumber=1&pageSize=10&search=` | List projects (paginated) |
| GET | `/projects/{id}` | Get project by ID |
| PUT | `/projects/{id}` | Update project |
| DELETE | `/projects/{id}` | Delete project and its tasks |

### Tasks (Authorized)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/tasks` | Create task |
| PATCH | `/tasks/{id}/status` | Update task status |
| GET | `/tasks/project/{projectId}?status=&pageNumber=1&pageSize=20` | List tasks by project |
| DELETE | `/tasks/{id}` | Delete task |

### Admin (Admin role only)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/admin/stats` | System-wide user/project/task statistics |

### Standard Response Format

```json
{
  "success": true,
  "message": "Optional message",
  "data": { },
  "errors": null
}
```

### Task Status Values (`TaskItemStatus`)

`Todo` (0), `InProgress` (1), `Done` (2), `Cancelled` (3)

### Task Priority Values

`Low` (0), `Medium` (1), `High` (2), `Critical` (3)

---

## Swagger JWT Usage

1. Call `POST /api/v1/auth/login` with credentials.
2. Copy the `token` from the response `data` object.
3. Click **Authorize** in Swagger.
4. Enter: `Bearer <your-token>`

---

## EF Core Commands

```bash
# Add migration
dotnet ef migrations add <MigrationName> --project src/ProjectTaskManagement.Infrastructure --startup-project src/ProjectTaskManagement.API --output-dir Persistence/Migrations

# Update database
dotnet ef database update --project src/ProjectTaskManagement.Infrastructure --startup-project src/ProjectTaskManagement.API

# Remove last migration
dotnet ef migrations remove --project src/ProjectTaskManagement.Infrastructure --startup-project src/ProjectTaskManagement.API
```

---

## Run Tests

```bash
dotnet test   # 10 tests
```

## REST Client

Use `ProjectTaskManagement.API.http` in VS Code (REST Client extension) or Rider to exercise all endpoints with saved variables.

## Architecture Doc

See [ARCHITECTURE.md](ARCHITECTURE.md) for diagrams and design decisions.

---

## GitHub Commit Strategy

Recommended incremental commits for a professional submission:

```text
1. chore: initialize clean architecture solution structure
2. feat(domain): add entities, enums, and repository abstractions
3. feat(application): implement CQRS handlers, DTOs, and FluentValidation
4. feat(infrastructure): add EF Core, repositories, JWT, and Redis caching
5. feat(api): add controllers, middleware, swagger, and versioning
6. feat(auth): implement JWT authentication and role-based authorization
7. chore(docker): add Dockerfile and docker-compose setup
8. test: add unit tests for validators and handlers
9. docs: add README and API documentation
```

---

## Production Notes

- Change `JwtSettings:Secret` to a strong key stored in **Azure Key Vault**, **AWS Secrets Manager**, or environment variables.
- Restrict CORS policy to known frontend origins (replace `AllowAll`).
- Use **HTTPS** termination at reverse proxy (NGINX / Azure App Gateway).
- Enable **SQL Server backups** and connection resiliency (EF retry policy).
- Configure **Redis** for production cluster with authentication.
- Add **rate limiting** and **API gateway** when moving to microservices.
- Consider splitting into bounded contexts (Auth Service, Project Service) for true microservices.

---

## License

MIT — built as a technical assessment demonstration project.
