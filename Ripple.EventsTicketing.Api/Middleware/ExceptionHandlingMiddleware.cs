using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Ripple.EventsTicketing.Application.Common.Exceptions;

namespace Ripple.EventsTicketing.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
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
        catch (NotFoundException ex)
        {
            await WriteProblemAsync(context, HttpStatusCode.NotFound, "Not Found", ex.Message);
        }
        catch (ConflictException ex)
        {
            await WriteProblemAsync(context, HttpStatusCode.Conflict, "Conflict", ex.Message);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray());

            var problem = new HttpValidationProblemDetails(errors)
            {
                Title = "Validation Failed",
                Status = (int)HttpStatusCode.BadRequest
            };

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await WriteProblemAsync(context, HttpStatusCode.InternalServerError, "Internal Server Error", "An unexpected error occurred.");
        }
    }

    private static async Task WriteProblemAsync(HttpContext context, HttpStatusCode statusCode, string title, string detail)
    {
        var problem = new ProblemDetails
        {
            Title = title,
            Detail = detail,
            Status = (int)statusCode
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }
}
