using MediatR;
using ProjectTaskManagement.Application.Common.Interfaces;
using ProjectTaskManagement.Application.DTOs.Projects;
using ProjectTaskManagement.Application.Features.Projects.Commands.CreateProject;
using ProjectTaskManagement.Domain.Exceptions;
using ProjectTaskManagement.Domain.Interfaces;

namespace ProjectTaskManagement.Application.Features.Projects.Queries.GetProjectById;

public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public GetProjectByIdQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ProjectDto> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException();
        var project = await _unitOfWork.Projects.GetByIdAsync(request.Id, cancellationToken);

        if (project is null || project.UserId != userId)
            throw new NotFoundException(nameof(Domain.Entities.Project), request.Id);

        var tasks = await _unitOfWork.Tasks.FindAsync(t => t.ProjectId == project.Id, cancellationToken);
        return CreateProjectCommandHandler.MapToDto(project, tasks.Count);
    }
}
