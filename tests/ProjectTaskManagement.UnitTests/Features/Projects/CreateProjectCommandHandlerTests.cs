using FluentAssertions;
using Moq;
using ProjectTaskManagement.Application.Common.Interfaces;
using ProjectTaskManagement.Application.Features.Projects.Commands.CreateProject;
using ProjectTaskManagement.Domain.Entities;
using ProjectTaskManagement.Domain.Interfaces;

namespace ProjectTaskManagement.UnitTests.Features.Projects;

public class CreateProjectCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Create_Project_And_Invalidate_Cache()
    {
        var userId = Guid.NewGuid();
        var unitOfWork = new Mock<IUnitOfWork>();
        var projectsRepo = new Mock<IRepository<Project>>();
        var currentUser = new Mock<ICurrentUserService>();
        var cacheService = new Mock<ICacheService>();

        currentUser.Setup(x => x.UserId).Returns(userId);
        unitOfWork.Setup(x => x.Projects).Returns(projectsRepo.Object);
        projectsRepo.Setup(x => x.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project p, CancellationToken _) => p);
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        cacheService.Setup(x => x.InvalidateVersionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new CreateProjectCommandHandler(
            unitOfWork.Object,
            currentUser.Object,
            cacheService.Object);

        var result = await handler.Handle(
            new CreateProjectCommand("Test Project", "Description"),
            CancellationToken.None);

        result.Name.Should().Be("Test Project");
        result.Description.Should().Be("Description");
        projectsRepo.Verify(
            x => x.AddAsync(It.Is<Project>(p => p.UserId == userId), It.IsAny<CancellationToken>()),
            Times.Once);
        cacheService.Verify(
            x => x.InvalidateVersionAsync(It.Is<string>(k => k.Contains(userId.ToString())), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
