using MediatR;
using ProjectTaskManagement.Application.Common.Interfaces;
using ProjectTaskManagement.Application.DTOs.Auth;
using ProjectTaskManagement.Domain.Entities;
using ProjectTaskManagement.Domain.Enums;
using ProjectTaskManagement.Domain.Exceptions;
using ProjectTaskManagement.Domain.Interfaces;

namespace ProjectTaskManagement.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public RegisterCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var exists = await _unitOfWork.Users.ExistsAsync(
            u => u.Email == request.Email.ToLowerInvariant(), cancellationToken);

        if (exists)
            throw new ConflictException("Email is already registered.");

        var user = new ApplicationUser
        {
            Email = request.Email.ToLowerInvariant(),
            FullName = request.FullName,
            PasswordHash = _passwordHasher.Hash(request.Password),
            Role = UserRoles.User
        };

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return BuildAuthResponse(user);
    }

    private AuthResponseDto BuildAuthResponse(ApplicationUser user)
    {
        var token = _jwtTokenService.GenerateToken(user);
        return new AuthResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };
    }
}
