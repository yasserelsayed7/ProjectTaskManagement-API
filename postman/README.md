# Postman Collection

## Files

| File | Description |
|------|-------------|
| `ProjectTaskManagement-API.postman_collection.json` | Full API collection (all endpoints) |
| `ProjectTaskManagement-Local.postman_environment.json` | Local environment variables |

## Import into Postman

1. Open **Postman** → **Import**
2. Drag both JSON files (or select them)
3. Select environment **ProjectTaskManagement - Local**
4. Start the API (`dotnet run` or `docker-compose up`)

## Recommended test flow

1. **Health** → Health Check  
2. **Auth** → Login (Demo User) — saves `token` automatically  
3. **Projects** → Create Project — saves `projectId`  
4. **Tasks** → Create Task — saves `taskId`  
5. **Tasks** → Update Task Status  
6. **Auth** → Login (Admin) → **Admin** → Get Admin Stats  

## Variables (auto-managed)

| Variable | Set by |
|----------|--------|
| `token` | Login (Demo User) test script |
| `adminToken` | Login (Admin) test script |
| `projectId` | Create Project test script |
| `taskId` | Create Task test script |

## Swagger alternative

Swagger UI: http://localhost:5000/swagger
