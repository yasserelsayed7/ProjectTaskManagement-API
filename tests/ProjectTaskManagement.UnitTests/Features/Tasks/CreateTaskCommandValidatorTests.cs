using FluentAssertions;
using ProjectTaskManagement.Application.Features.Tasks.Commands.CreateTask;
using ProjectTaskManagement.Domain.Enums;

namespace ProjectTaskManagement.UnitTests.Features.Tasks;

public class CreateTaskCommandValidatorTests
{
    private readonly CreateTaskCommandValidator _validator = new();

    [Fact]
    public void Should_Fail_When_Title_Is_Empty()
    {
        var command = new CreateTaskCommand(Guid.NewGuid(), "", "desc", null, TaskPriority.High);
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_Pass_When_Command_Is_Valid()
    {
        var command = new CreateTaskCommand(
            Guid.NewGuid(),
            "Implement API",
            "Build endpoints",
            DateTime.UtcNow.AddDays(3),
            TaskPriority.Medium);

        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }
}
