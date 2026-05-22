using ProjectTaskManagement.Domain.Entities;

namespace ProjectTaskManagement.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(ApplicationUser user);
}
