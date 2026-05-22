using System.Net;
using System.Text.Json;
using FluentValidation;
using ProjectTaskManagement.Application.Common.Models;
using ProjectTaskManagement.Domain.Exceptions;

namespace ProjectTaskManagement.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message, errors) = exception switch
        {
            ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                "Validation failed.",
                validationEx.Errors.Select(e => e.ErrorMessage)),

            NotFoundException notFoundEx => (
                HttpStatusCode.NotFound,
                notFoundEx.Message,
                null),

            UnauthorizedException unauthorizedEx => (
                HttpStatusCode.Unauthorized,
                unauthorizedEx.Message,
                null),

            ConflictException conflictEx => (
                HttpStatusCode.Conflict,
                conflictEx.Message,
                null),

            DomainException domainEx => (
                HttpStatusCode.BadRequest,
                domainEx.Message,
                null),

            _ => (
                HttpStatusCode.InternalServerError,
                "An unexpected error occurred.",
                null)
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse<object>.Fail(message, errors);
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
