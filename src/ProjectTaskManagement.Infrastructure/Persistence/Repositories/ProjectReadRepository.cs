using Microsoft.EntityFrameworkCore;
using ProjectTaskManagement.Application.Common.Interfaces;
using ProjectTaskManagement.Application.Common.Models;
using ProjectTaskManagement.Application.DTOs.Projects;

namespace ProjectTaskManagement.Infrastructure.Persistence.Repositories;

public class ProjectReadRepository : IProjectReadRepository
{
    private readonly ApplicationDbContext _context;

    public ProjectReadRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ProjectDto>> GetPagedByUserAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Projects
            .AsNoTracking()
            .Where(p => p.UserId == userId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p =>
                p.Name.Contains(search) ||
                p.Description.Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                TaskCount = p.Tasks.Count
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<ProjectDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
