namespace ProjectTaskManagement.Application.Common.Constants;

public static class CacheKeys
{
    public static string ProjectsVersion(Guid userId) => $"projects:version:{userId}";

    public static string ProjectsList(
        Guid userId,
        string version,
        int pageNumber,
        int pageSize,
        string? search) =>
        $"projects:{userId}:{version}:{pageNumber}:{pageSize}:{search ?? string.Empty}";
}
