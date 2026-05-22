using ProjectTaskManagement.Application.Common.Constants;
using ProjectTaskManagement.Application.Common.Interfaces;

namespace ProjectTaskManagement.Application.Common.Extensions;

public static class ProjectCacheInvalidation
{
    public static Task InvalidateUserProjectsAsync(
        this ICacheService cacheService,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        cacheService.InvalidateVersionAsync(CacheKeys.ProjectsVersion(userId), cancellationToken);
}
