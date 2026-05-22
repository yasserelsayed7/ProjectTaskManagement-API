using MediatR;
using ProjectTaskManagement.Application.Common.Models;

namespace ProjectTaskManagement.Application.Features.Projects.Queries.GetAllProjects;

public record GetAllProjectsQuery(int PageNumber = 1, int PageSize = 10, string? Search = null)
    : IRequest<PagedResult<DTOs.Projects.ProjectDto>>;
