# Project & Task Management API

[![CI](https://github.com/yasserelsayed7/ProjectTaskManagement-API/actions/workflows/ci.yml/badge.svg)](https://github.com/yasserelsayed7/ProjectTaskManagement-API/actions/workflows/ci.yml)
[![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean%20%2F%20Onion-2ea44f)](ARCHITECTURE.md)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

> **Backend .NET Developer Technical Assessment** — A production-style REST API for managing projects and tasks with JWT authentication, Clean Architecture, and enterprise patterns.

**Author:** [Yasser Elsayed](https://github.com/yasserelsayed7)

---

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Quick Start](#quick-start)
- [Docker](#docker)
- [API Reference](#api-reference)
- [Testing](#testing)
- [Project Structure](#project-structure)
- [Documentation](#documentation)
- [Assessment Checklist](#assessment-checklist)

---

## Overview

Each authenticated user can create and manage **projects**, and manage **tasks** within those projects. The API enforces ownership so users only access their own data, with an **Admin** role for system statistics.

**Live repository:** [github.com/yasserelsayed7/ProjectTaskManagement-API](https://github.com/yasserelsayed7/ProjectTaskManagement-API)

---

## Features

| Category | Implemented |
|----------|-------------|
| Authentication | Register, Login, JWT, BCrypt hashing |
| Projects CRUD | Create, Read (list + by id), Update, Delete |
| Tasks | Create, Update status, List by project, Delete |
| Architecture | Clean / Onion, SOLID, DI, Repository + UoW |
| CQRS | MediatR commands & queries |
| Validation | FluentValidation pipeline |
| Caching | Redis with version-based invalidation |
| API | Versioning (v1), Swagger + JWT, CORS, health checks |
| DevOps | Dockerfile, docker-compose, GitHub Actions CI |
| Testing | 10 xUnit tests (Moq, FluentAssertions) |
| Extras | Serilog, pagination, search, role-based admin API |

---

## Tech Stack

.NET 9 · ASP.NET Core Web API · EF Core 9 · SQL Server · JWT · MediatR · FluentValidation · Redis · Serilog · Swagger · Docker · xUnit

---

## Quick Start

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download) (or SDK 10+ with roll-forward)
- SQL Server (local or Docker)

### Clone & run

```bash
git clone https://github.com/yasserelsayed7/ProjectTaskManagement-API.git
cd ProjectTaskManagement-API
dotnet restore ProjectTaskManagement.sln
dotnet ef database update --project src/ProjectTaskManagement.Infrastructure --startup-project src/ProjectTaskManagement.API
dotnet run --project src/ProjectTaskManagement.API
```

Open **Swagger:** http://localhost:5000/swagger

### Demo accounts

| Email | Password | Role |
|-------|----------|------|
| `user@projecttask.com` | `User@123` | User |
| `admin@projecttask.com` | `Admin@123` | Admin |

---

## Docker

One command runs **API + SQL Server + Redis**:

```bash
docker-compose up --build
```

| Service | URL |
|---------|-----|
| Swagger | http://localhost:5000/swagger |
| Health | http://localhost:5000/health |
| SQL Server | `localhost:1433` |
| Redis | `localhost:6379` |

Migrations and seed data apply automatically on startup.

---

## API Reference

**Base URL:** `/api/v1`

<details>
<summary><strong>Authentication</strong> (public)</summary>

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/auth/register` | Register |
| `POST` | `/auth/login` | Login → JWT |

</details>

<details>
<summary><strong>Projects</strong> (Bearer token required)</summary>

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/projects` | Create |
| `GET` | `/projects?pageNumber=1&pageSize=10&search=` | List (paginated) |
| `GET` | `/projects/{id}` | Get by id |
| `PUT` | `/projects/{id}` | Update |
| `DELETE` | `/projects/{id}` | Delete (+ cascade tasks) |

</details>

<details>
<summary><strong>Tasks</strong> (Bearer token required)</summary>

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/tasks` | Create |
| `PATCH` | `/tasks/{id}/status` | Update status |
| `GET` | `/tasks/project/{projectId}` | List by project |
| `DELETE` | `/tasks/{id}` | Delete |

**Status:** `Todo=0` · `InProgress=1` · `Done=2` · `Cancelled=3`  
**Priority:** `Low=0` · `Medium=1` · `High=2` · `Critical=3`

</details>

<details>
<summary><strong>Admin</strong> (Admin role only)</summary>

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/admin/stats` | Users / projects / tasks counts |

</details>

### Response envelope

```json
{
  "success": true,
  "message": "Optional message",
  "data": { },
  "errors": null
}
```

### Swagger JWT

1. `POST /api/v1/auth/login` → copy `data.token`
2. Swagger **Authorize** → `Bearer <token>`

---

## Testing

```bash
dotnet test ProjectTaskManagement.sln
```

### Postman Collection

Import from the `postman/` folder:

| File | Purpose |
|------|---------|
| `postman/ProjectTaskManagement-API.postman_collection.json` | All API endpoints with auto JWT token |
| `postman/ProjectTaskManagement-Local.postman_environment.json` | Local `baseUrl` variables |

See [postman/README.md](postman/README.md) for import steps and test flow.

### REST Client (VS Code / Rider)

Use **`ProjectTaskManagement.API.http`** for manual API testing.

---

## Project Structure

```
ProjectTaskManagement-API/
├── src/
│   ├── ProjectTaskManagement.Domain/
│   ├── ProjectTaskManagement.Application/    # CQRS, DTOs, Validation
│   ├── ProjectTaskManagement.Infrastructure/ # EF Core, JWT, Redis
│   └── ProjectTaskManagement.API/
├── tests/
│   └── ProjectTaskManagement.UnitTests/
├── Dockerfile
├── docker-compose.yml
├── postman/                                  # Postman collection + environment
├── ARCHITECTURE.md
└── ProjectTaskManagement.sln
```

---

## Documentation

- [ARCHITECTURE.md](ARCHITECTURE.md) — layers, request flow, caching, security
- [SUBMISSION.md](SUBMISSION.md) — assessment requirements mapping

---

## Assessment Checklist

| Requirement | Status |
|-------------|--------|
| .NET 9 Web API | ✅ |
| Clean / Onion Architecture | ✅ |
| EF Core + SQL Server | ✅ |
| JWT Authentication | ✅ |
| Projects module (full CRUD) | ✅ |
| Tasks module | ✅ |
| CQRS + MediatR | ✅ |
| FluentValidation | ✅ |
| Global exception handling | ✅ |
| Repository pattern | ✅ |
| Docker | ✅ |
| Unit tests | ✅ |
| Redis caching | ✅ |
| API versioning + Swagger JWT | ✅ |
| Role-based authorization | ✅ |
| Pagination | ✅ |

---

## License

[MIT](LICENSE) © 2026 Yasser Elsayed
