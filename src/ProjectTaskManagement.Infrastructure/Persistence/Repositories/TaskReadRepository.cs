using Microsoft.EntityFrameworkCore;
using ProjectTaskManagement.Application.Common.Interfaces;
using ProjectTaskManagement.Application.Common.Models;
using ProjectTaskManagement.Application.DTOs.Tasks;
using ProjectTaskManagement.Domain.Enums;

namespace ProjectTaskManagement.Infrastructure.Persistence.Repositories;

public class TaskReadRepository : ITaskReadRepository
{
    private readonly ApplicationDbContext _context;

    public TaskReadRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<TaskDto>> GetPagedByProjectAsync(
        Guid projectId,
        int pageNumber,
        int pageSize,
        TaskItemStatus? status,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Tasks
            .AsNoTracking()
            .Where(t => t.ProjectId == projectId);

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                DueDate = t.DueDate,
                Priority = t.Priority,
                ProjectId = t.ProjectId,
                CreatedAt = t.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<TaskDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
