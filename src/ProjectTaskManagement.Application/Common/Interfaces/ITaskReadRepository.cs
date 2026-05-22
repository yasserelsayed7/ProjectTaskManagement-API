using ProjectTaskManagement.Application.Common.Models;
using ProjectTaskManagement.Application.DTOs.Tasks;
using ProjectTaskManagement.Domain.Enums;

namespace ProjectTaskManagement.Application.Common.Interfaces;

public interface ITaskReadRepository
{
    Task<PagedResult<TaskDto>> GetPagedByProjectAsync(
        Guid projectId,
        int pageNumber,
        int pageSize,
        TaskItemStatus? status,
        CancellationToken cancellationToken = default);
}
