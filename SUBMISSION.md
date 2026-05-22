# Technical Assessment Submission

**Candidate:** Yasser Elsayed  
**Repository:** [ProjectTaskManagement-API](https://github.com/yasserelsayed7/ProjectTaskManagement-API)  
**Role:** Backend .NET Developer  

---

## Assignment Summary

Built a **Project & Task Management REST API** using **.NET 9**, demonstrating Clean Architecture, SOLID principles, and production-ready patterns suitable for microservices evolution.

---

## Requirements Mapping

### Mandatory stack

| Requirement | Implementation |
|-------------|----------------|
| .NET 9 | All projects target `net9.0` |
| ASP.NET Core Web API | `ProjectTaskManagement.API` |
| EF Core + SQL Server | `ApplicationDbContext`, migrations, seed |
| JWT Authentication | `AuthenticationExtensions`, `JwtTokenService` |
| Clean Architecture | 4-layer solution (Domain → Application → Infrastructure → API) |

### Functional

| Feature | Endpoints |
|---------|-----------|
| Register / Login | `POST /api/v1/auth/register`, `POST /api/v1/auth/login` |
| Projects CRUD | `POST/GET/PUT/DELETE /api/v1/projects` |
| Tasks | `POST /api/v1/tasks`, `PATCH .../status`, `GET .../project/{id}`, `DELETE` |

### Architecture

- **Onion / Clean:** dependencies point inward; Domain has zero infrastructure references
- **SOLID:** single-purpose handlers, interface abstractions (`IUnitOfWork`, `ICacheService`, read repositories)
- **Patterns:** CQRS, MediatR pipeline behaviors, Repository + Unit of Work, DTOs, middleware exception handling

### Bonus features delivered

- CQRS + MediatR
- Docker + docker-compose
- Unit tests (10)
- Redis caching with correct invalidation
- Generic `ApiResponse<T>` wrapper
- Role-based authorization (User / Admin)
- API versioning (`/api/v1/...`)
- Serilog
- FluentValidation
- Health checks (`/health`, `/health/ready`)
- Pagination + search on projects
- Swagger JWT support

---

## How to Review (5 minutes)

```bash
git clone https://github.com/yasserelsayed7/ProjectTaskManagement-API.git
cd ProjectTaskManagement-API
docker-compose up --build
```

1. Open http://localhost:5000/swagger  
2. Login: `user@projecttask.com` / `User@123`  
3. Authorize with Bearer token  
4. Create a project → create a task → update task status  

For admin: login as `admin@projecttask.com` / `Admin@123` → `GET /api/v1/admin/stats`

### Postman

Import `postman/ProjectTaskManagement-API.postman_collection.json` and `postman/ProjectTaskManagement-Local.postman_environment.json`.  
Run **Auth → Login (Demo User)** first — JWT is saved automatically.

---

## Design Highlights for Interview

1. **Cache version tokens** — avoids stale paginated project lists without Redis SCAN
2. **Read repositories** — server-side pagination (`ProjectReadRepository`, `TaskReadRepository`)
3. **JWT in API layer** — Infrastructure does not depend on ASP.NET authentication packages
4. **`TaskItem` + `TaskItemStatus`** — avoids `System.Threading.Tasks.TaskStatus` ambiguity

---

## Contact

GitHub: [@yasserelsayed7](https://github.com/yasserelsayed7)
