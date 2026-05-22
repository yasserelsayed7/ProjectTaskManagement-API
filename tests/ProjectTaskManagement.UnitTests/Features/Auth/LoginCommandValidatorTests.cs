using FluentAssertions;
using ProjectTaskManagement.Application.Features.Auth.Commands.Login;

namespace ProjectTaskManagement.UnitTests.Features.Auth;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    [Fact]
    public void Should_Fail_When_Email_Is_Empty()
    {
        var result = _validator.Validate(new LoginCommand("", "password"));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_Pass_When_Command_Is_Valid()
    {
        var result = _validator.Validate(new LoginCommand("user@test.com", "password"));
        result.IsValid.Should().BeTrue();
    }
}
