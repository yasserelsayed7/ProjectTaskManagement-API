using MediatR;
using ProjectTaskManagement.Application.Common.Extensions;
using ProjectTaskManagement.Application.Common.Interfaces;
using ProjectTaskManagement.Application.DTOs.Projects;
using ProjectTaskManagement.Domain.Entities;
using ProjectTaskManagement.Domain.Exceptions;
using ProjectTaskManagement.Domain.Interfaces;

namespace ProjectTaskManagement.Application.Features.Projects.Commands.CreateProject;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly ICacheService _cacheService;

    public CreateProjectCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _cacheService = cacheService;
    }

    public async Task<ProjectDto> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException();

        var project = new Project
        {
            Name = request.Name,
            Description = request.Description,
            UserId = userId
        };

        await _unitOfWork.Projects.AddAsync(project, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _cacheService.InvalidateUserProjectsAsync(userId, cancellationToken);

        return MapToDto(project, 0);
    }

    internal static ProjectDto MapToDto(Project project, int taskCount) => new()
    {
        Id = project.Id,
        Name = project.Name,
        Description = project.Description,
        CreatedAt = project.CreatedAt,
        TaskCount = taskCount
    };
}
