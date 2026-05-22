using ProjectTaskManagement.Application.Common.Models;
using ProjectTaskManagement.Application.DTOs.Projects;

namespace ProjectTaskManagement.Application.Common.Interfaces;

public interface IProjectReadRepository
{
    Task<PagedResult<ProjectDto>> GetPagedByUserAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        string? search,
        CancellationToken cancellationToken = default);
}
