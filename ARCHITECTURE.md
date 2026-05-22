# Architecture Overview

> Part of [ProjectTaskManagement-API](https://github.com/yasserelsayed7/ProjectTaskManagement-API)

## Clean Architecture Layers

```
┌─────────────────────────────────────────────┐
│              API (Presentation)              │
│  Controllers, Middleware, Swagger, Auth    │
└─────────────────────┬───────────────────────┘
                      │
┌─────────────────────▼───────────────────────┐
│           Infrastructure                     │
│  EF Core, SQL Server, Redis, JWT, Repos     │
└─────────────────────┬───────────────────────┘
                      │
┌─────────────────────▼───────────────────────┐
│              Application                     │
│  CQRS, MediatR, Validation, DTOs, Behaviors │
└─────────────────────┬───────────────────────┘
                      │
┌─────────────────────▼───────────────────────┐
│                Domain                        │
│  Entities, Enums, Interfaces, Exceptions    │
└─────────────────────────────────────────────┘
```

## Request Flow

1. HTTP request hits **Controller**
2. Controller sends **MediatR** command/query
3. **ValidationBehavior** runs FluentValidation rules
4. **Handler** executes business logic via **IUnitOfWork** / read repositories
5. Response wrapped in **ApiResponse&lt;T&gt;**
6. Exceptions caught by **ExceptionHandlingMiddleware**

## Key Design Decisions

| Decision | Rationale |
|----------|-----------|
| CQRS + MediatR | Separates reads/writes; scales to complex domains |
| Generic Repository + UoW | Simple CRUD; specialized read repos for pagination |
| Cache version tokens | Correct invalidation without Redis SCAN |
| JWT in API layer | Infrastructure stays free of ASP.NET auth packages |
| `TaskItem` entity name | Avoids conflict with `System.Threading.Tasks.Task` |
| Cascade delete on Project→Tasks | DB integrity; simpler delete handler |

## Security Model

- Each user owns projects (`UserId` FK)
- All project/task handlers verify ownership
- JWT contains `NameIdentifier`, `Email`, `Role` claims
- `AdminOnly` policy for `/api/v1/admin/*`

## Caching Strategy

- Project list cached per user + version + page + search
- Version key bumped on any project/task mutation affecting counts
- Redis in production; in-memory fallback for local dev without Redis
