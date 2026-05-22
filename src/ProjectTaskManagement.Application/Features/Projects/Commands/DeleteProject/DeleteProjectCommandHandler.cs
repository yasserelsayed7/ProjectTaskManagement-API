using MediatR;
using ProjectTaskManagement.Application.Common.Extensions;
using ProjectTaskManagement.Application.Common.Interfaces;
using ProjectTaskManagement.Domain.Exceptions;
using ProjectTaskManagement.Domain.Interfaces;

namespace ProjectTaskManagement.Application.Features.Projects.Commands.DeleteProject;

public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly ICacheService _cacheService;

    public DeleteProjectCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _cacheService = cacheService;
    }

    public async Task<Unit> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException();
        var project = await _unitOfWork.Projects.GetByIdAsync(request.Id, cancellationToken);

        if (project is null || project.UserId != userId)
            throw new NotFoundException(nameof(Domain.Entities.Project), request.Id);

        await _unitOfWork.Projects.DeleteAsync(project, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _cacheService.InvalidateUserProjectsAsync(userId, cancellationToken);

        return Unit.Value;
    }
}
