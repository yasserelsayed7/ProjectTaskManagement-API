using MediatR;
using ProjectTaskManagement.Application.DTOs.Admin;
using ProjectTaskManagement.Domain.Enums;
using ProjectTaskManagement.Domain.Interfaces;

namespace ProjectTaskManagement.Application.Features.Admin.Queries.GetAdminStats;

public class GetAdminStatsQueryHandler : IRequestHandler<GetAdminStatsQuery, AdminStatsDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAdminStatsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AdminStatsDto> Handle(GetAdminStatsQuery request, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);
        var projects = await _unitOfWork.Projects.GetAllAsync(cancellationToken);
        var tasks = await _unitOfWork.Tasks.GetAllAsync(cancellationToken);

        return new AdminStatsDto
        {
            TotalUsers = users.Count,
            TotalProjects = projects.Count,
            TotalTasks = tasks.Count,
            AdminUsers = users.Count(u => u.Role == UserRoles.Admin)
        };
    }
}
