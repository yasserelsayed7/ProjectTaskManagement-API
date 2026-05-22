using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTaskManagement.Application.Common.Models;
using ProjectTaskManagement.Application.DTOs.Admin;
using ProjectTaskManagement.Application.Features.Admin.Queries.GetAdminStats;

namespace ProjectTaskManagement.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class AdminController : ApiControllerBase
{
    [HttpGet("stats")]
    [ProducesResponseType(typeof(ApiResponse<AdminStatsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAdminStatsQuery(), cancellationToken);
        return OkResponse(result);
    }
}
