using System.Net;
using System.Text.Json;
using BaseTemplate.Application.Common.Exceptions;
using BaseTemplate.Domain.Exceptions;
using FluentValidation;

namespace BaseTemplate.API.Middleware;

/// <summary>
/// Central place that turns exceptions into consistent ProblemDetails responses,
/// so controllers never need try/catch for these well-known failure kinds.
/// </summary>
public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title, errors) = exception switch
        {
            ValidationException validationException => (
                HttpStatusCode.BadRequest,
                "One or more validation errors occurred.",
                validationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())),

            NotFoundException notFoundException => (
                HttpStatusCode.NotFound,
                notFoundException.Message,
                null),

            DomainException domainException => (
                HttpStatusCode.BadRequest,
                domainException.Message,
                null),

            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.", null),
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception processing {Method} {Path}", context.Request.Method, context.Request.Path);
        }
        else
        {
            logger.LogWarning(exception, "Handled exception processing {Method} {Path}: {Message}", context.Request.Method, context.Request.Path, exception.Message);
        }

        var problemDetails = new
        {
            type = $"https://httpstatuses.com/{(int)statusCode}",
            title,
            status = (int)statusCode,
            traceId = context.TraceIdentifier,
            errors,
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        }));
    }
}
