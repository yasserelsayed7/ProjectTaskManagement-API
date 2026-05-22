using MediatR;
using ProjectTaskManagement.Application.Common.Constants;
using ProjectTaskManagement.Application.Common.Interfaces;
using ProjectTaskManagement.Application.Common.Models;
using ProjectTaskManagement.Application.DTOs.Projects;
using ProjectTaskManagement.Domain.Exceptions;

namespace ProjectTaskManagement.Application.Features.Projects.Queries.GetAllProjects;

public class GetAllProjectsQueryHandler : IRequestHandler<GetAllProjectsQuery, PagedResult<ProjectDto>>
{
    private readonly IProjectReadRepository _projectReadRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly ICacheService _cacheService;

    public GetAllProjectsQueryHandler(
        IProjectReadRepository projectReadRepository,
        ICurrentUserService currentUser,
        ICacheService cacheService)
    {
        _projectReadRepository = projectReadRepository;
        _currentUser = currentUser;
        _cacheService = cacheService;
    }

    public async Task<PagedResult<ProjectDto>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException();
        var versionKey = CacheKeys.ProjectsVersion(userId);
        var version = await _cacheService.GetOrCreateVersionAsync(versionKey, cancellationToken);
        var cacheKey = CacheKeys.ProjectsList(userId, version, request.PageNumber, request.PageSize, request.Search);

        var cached = await _cacheService.GetAsync<PagedResult<ProjectDto>>(cacheKey, cancellationToken);
        if (cached is not null)
            return cached;

        var result = await _projectReadRepository.GetPagedByUserAsync(
            userId,
            request.PageNumber,
            request.PageSize,
            request.Search,
            cancellationToken);

        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5), cancellationToken);
        return result;
    }
}
