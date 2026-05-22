using MediatR;
using ProjectTaskManagement.Application.Common.Interfaces;
using ProjectTaskManagement.Application.Common.Models;
using ProjectTaskManagement.Application.DTOs.Tasks;
using ProjectTaskManagement.Domain.Exceptions;
using ProjectTaskManagement.Domain.Interfaces;

namespace ProjectTaskManagement.Application.Features.Tasks.Queries.GetTasksByProject;

public class GetTasksByProjectQueryHandler : IRequestHandler<GetTasksByProjectQuery, PagedResult<TaskDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly ITaskReadRepository _taskReadRepository;

    public GetTasksByProjectQueryHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        ITaskReadRepository taskReadRepository)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _taskReadRepository = taskReadRepository;
    }

    public async Task<PagedResult<TaskDto>> Handle(GetTasksByProjectQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException();
        var project = await _unitOfWork.Projects.GetByIdAsync(request.ProjectId, cancellationToken);

        if (project is null || project.UserId != userId)
            throw new NotFoundException(nameof(Domain.Entities.Project), request.ProjectId);

        return await _taskReadRepository.GetPagedByProjectAsync(
            request.ProjectId,
            request.PageNumber,
            request.PageSize,
            request.Status,
            cancellationToken);
    }
}
