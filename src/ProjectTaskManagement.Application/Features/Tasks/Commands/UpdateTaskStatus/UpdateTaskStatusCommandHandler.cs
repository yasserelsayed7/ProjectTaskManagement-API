using MediatR;
using ProjectTaskManagement.Application.Common.Extensions;
using ProjectTaskManagement.Application.Common.Interfaces;
using ProjectTaskManagement.Application.DTOs.Tasks;
using ProjectTaskManagement.Application.Features.Tasks.Commands.CreateTask;
using ProjectTaskManagement.Domain.Entities;
using ProjectTaskManagement.Domain.Exceptions;
using ProjectTaskManagement.Domain.Interfaces;

namespace ProjectTaskManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;

public class UpdateTaskStatusCommandHandler : IRequestHandler<UpdateTaskStatusCommand, TaskDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly ICacheService _cacheService;

    public UpdateTaskStatusCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _cacheService = cacheService;
    }

    public async Task<TaskDto> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException();
        var task = await _unitOfWork.Tasks.GetByIdAsync(request.Id, cancellationToken);

        if (task is null)
            throw new NotFoundException(nameof(TaskItem), request.Id);

        var project = await _unitOfWork.Projects.GetByIdAsync(task.ProjectId, cancellationToken);
        if (project is null || project.UserId != userId)
            throw new NotFoundException(nameof(TaskItem), request.Id);

        task.Status = request.Status;
        await _unitOfWork.Tasks.UpdateAsync(task, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _cacheService.InvalidateUserProjectsAsync(userId, cancellationToken);

        return CreateTaskCommandHandler.MapToDto(task);
    }
}
