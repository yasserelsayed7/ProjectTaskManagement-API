using MediatR;
using ProjectTaskManagement.Application.DTOs.Auth;

namespace ProjectTaskManagement.Application.Features.Auth.Commands.Register;

public record RegisterCommand(string Email, string Password, string FullName) : IRequest<AuthResponseDto>;
