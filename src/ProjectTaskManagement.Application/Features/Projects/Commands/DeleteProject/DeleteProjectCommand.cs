using MediatR;

namespace ProjectTaskManagement.Application.Features.Projects.Commands.DeleteProject;

public record DeleteProjectCommand(Guid Id) : IRequest<Unit>;
