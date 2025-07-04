using JobApplicationTracker.Service.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;

namespace JobApplicationTracker.Api.GlobalExceptionHandler;

public class AppExceptionHandler : IExceptionHandler
{
    private readonly ILogger<AppExceptionHandler> _logger;

    public AppExceptionHandler(ILogger<AppExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, $"An unhandled exception of type {typeof(Exception)} occured: " +
                                    $"{exception.Message}");

        httpContext.Response.ContentType = "application/json";
        var statusCode = HttpStatusCode.InternalServerError;
        var message = exception.Message;
        var errors = new List<string>();

        switch (exception)
        {
            case NotFoundException notFoundException:
                statusCode = HttpStatusCode.NotFound;
                message = notFoundException.Message;
                break;

            case ValidationException validationException:
                statusCode = HttpStatusCode.BadRequest;
                message = "Validation failed.";
                errors.AddRange(validationException.Errors);
                break;

            case ArgumentException argEx:
                statusCode = HttpStatusCode.BadRequest;
                message = argEx.Message;
                break;

            case UnauthorizedAccessException _:
                statusCode = HttpStatusCode.Unauthorized;
                message = "Access denied. You are not authorized to perform this action.";
                break;

            default:
                if (httpContext.Response.HttpContext.RequestServices.GetService(
                    typeof(IHostEnvironment))
                    is IHostEnvironment env && env.IsDevelopment())
                {
                    message = exception.Message;
                }
                else
                {
                    message = "An unexpected error occurred. Please try again later.";
                }
                break;
        }

        httpContext.Response.StatusCode = (int) statusCode;

        var errorResponse = new
        {
            StatusCode = (int)statusCode,
            Message = message,
            Errors = errors.Any() ? errors : null
        };

        await httpContext.Response.WriteAsync(
            JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions 
            { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
            cancellationToken);

        // Return true to indicate the exception has been handled
        return true; 
    }
}
