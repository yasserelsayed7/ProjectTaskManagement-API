using MediatR;

namespace ProjectTaskManagement.Application.Features.Tasks.Commands.DeleteTask;

public record DeleteTaskCommand(Guid Id) : IRequest<Unit>;
