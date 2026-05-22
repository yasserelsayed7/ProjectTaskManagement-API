using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTaskManagement.Application.Common.Models;
using ProjectTaskManagement.Application.DTOs.Projects;
using ProjectTaskManagement.Application.Features.Projects.Commands.CreateProject;
using ProjectTaskManagement.Application.Features.Projects.Commands.DeleteProject;
using ProjectTaskManagement.Application.Features.Projects.Commands.UpdateProject;
using ProjectTaskManagement.Application.Features.Projects.Queries.GetAllProjects;
using ProjectTaskManagement.Application.Features.Projects.Queries.GetProjectById;

namespace ProjectTaskManagement.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class ProjectsController : ApiControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProjectDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(
        [FromBody] CreateProjectCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return CreatedResponse(nameof(GetById), new { id = result.Id, version = "1.0" }, result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ProjectDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(new GetAllProjectsQuery(pageNumber, pageSize, search), cancellationToken);
        return OkResponse(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProjectDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetProjectByIdQuery(id), cancellationToken);
        return OkResponse(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProjectDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new UpdateProjectCommand(id, request.Name, request.Description),
            cancellationToken);
        return OkResponse(result, "Project updated successfully.");
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new DeleteProjectCommand(id), cancellationToken);
        return Ok(ApiResponse<object>.Ok(null!, "Project deleted successfully."));
    }
}

public record UpdateProjectRequest(string Name, string Description);
