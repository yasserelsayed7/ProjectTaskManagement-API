using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProjectTaskManagement.Application.Common.Models;

namespace ProjectTaskManagement.API.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _mediator;
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected IActionResult OkResponse<T>(T data, string? message = null) =>
        Ok(ApiResponse<T>.Ok(data, message));

    protected IActionResult CreatedResponse<T>(string actionName, object routeValues, T data) =>
        CreatedAtAction(actionName, routeValues, ApiResponse<T>.Ok(data));
}
