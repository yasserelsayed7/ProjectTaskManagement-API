using MediatR;
using ProjectTaskManagement.Application.DTOs.Admin;

namespace ProjectTaskManagement.Application.Features.Admin.Queries.GetAdminStats;

public record GetAdminStatsQuery : IRequest<AdminStatsDto>;
