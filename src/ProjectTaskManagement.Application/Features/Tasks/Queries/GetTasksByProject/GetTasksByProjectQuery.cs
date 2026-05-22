using MediatR;
using ProjectTaskManagement.Application.DTOs.Tasks;
using ProjectTaskManagement.Domain.Enums;

namespace ProjectTaskManagement.Application.Features.Tasks.Queries.GetTasksByProject;

public record GetTasksByProjectQuery(
    Guid ProjectId,
    int PageNumber = 1,
    int PageSize = 20,
    TaskItemStatus? Status = null) : IRequest<Common.Models.PagedResult<TaskDto>>;
