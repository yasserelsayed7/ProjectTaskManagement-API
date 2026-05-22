using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTaskManagement.Application.Common.Models;
using ProjectTaskManagement.Application.DTOs.Tasks;
using ProjectTaskManagement.Application.Features.Tasks.Commands.CreateTask;
using ProjectTaskManagement.Application.Features.Tasks.Commands.DeleteTask;
using ProjectTaskManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;
using ProjectTaskManagement.Application.Features.Tasks.Queries.GetTasksByProject;
using ProjectTaskManagement.Domain.Enums;

namespace ProjectTaskManagement.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class TasksController : ApiControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TaskDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(
        [FromBody] CreateTaskCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return CreatedResponse(nameof(GetByProject), new { projectId = result.ProjectId, version = "1.0" }, result);
    }

    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(ApiResponse<TaskDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] UpdateTaskStatusRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new UpdateTaskStatusCommand(id, request.Status), cancellationToken);
        return OkResponse(result, "Task status updated successfully.");
    }

    [HttpGet("project/{projectId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<TaskDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByProject(
        Guid projectId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] TaskItemStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(
            new GetTasksByProjectQuery(projectId, pageNumber, pageSize, status),
            cancellationToken);
        return OkResponse(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new DeleteTaskCommand(id), cancellationToken);
        return Ok(ApiResponse<object>.Ok(null!, "Task deleted successfully."));
    }
}

public record UpdateTaskStatusRequest(TaskItemStatus Status);
