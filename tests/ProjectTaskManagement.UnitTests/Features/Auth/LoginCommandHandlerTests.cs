using FluentAssertions;
using Moq;
using ProjectTaskManagement.Application.Common.Interfaces;
using ProjectTaskManagement.Application.Features.Auth.Commands.Login;
using ProjectTaskManagement.Domain.Entities;
using ProjectTaskManagement.Domain.Enums;
using ProjectTaskManagement.Domain.Exceptions;
using ProjectTaskManagement.Domain.Interfaces;

namespace ProjectTaskManagement.UnitTests.Features.Auth;

public class LoginCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_Token_When_Credentials_Are_Valid()
    {
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = "user@test.com",
            FullName = "Test User",
            PasswordHash = "hashed",
            Role = UserRoles.User
        };

        var unitOfWork = new Mock<IUnitOfWork>();
        var usersRepo = new Mock<IRepository<ApplicationUser>>();
        var passwordHasher = new Mock<IPasswordHasher>();
        var jwtTokenService = new Mock<IJwtTokenService>();

        unitOfWork.Setup(x => x.Users).Returns(usersRepo.Object);
        usersRepo.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ApplicationUser, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ApplicationUser> { user });
        passwordHasher.Setup(x => x.Verify("Password123", "hashed")).Returns(true);
        jwtTokenService.Setup(x => x.GenerateToken(user)).Returns("jwt-token");

        var handler = new LoginCommandHandler(unitOfWork.Object, passwordHasher.Object, jwtTokenService.Object);
        var result = await handler.Handle(new LoginCommand("user@test.com", "Password123"), CancellationToken.None);

        result.Token.Should().Be("jwt-token");
        result.Email.Should().Be("user@test.com");
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Credentials_Are_Invalid()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        var usersRepo = new Mock<IRepository<ApplicationUser>>();
        var passwordHasher = new Mock<IPasswordHasher>();
        var jwtTokenService = new Mock<IJwtTokenService>();

        unitOfWork.Setup(x => x.Users).Returns(usersRepo.Object);
        usersRepo.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ApplicationUser, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ApplicationUser>());

        var handler = new LoginCommandHandler(unitOfWork.Object, passwordHasher.Object, jwtTokenService.Object);

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            handler.Handle(new LoginCommand("user@test.com", "wrong"), CancellationToken.None));
    }
}
