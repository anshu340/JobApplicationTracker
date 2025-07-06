using System.Net;
using JobApplicationTracker.Service.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace JobApplicationTracker.Api.GlobalExceptionHandler;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment hostEnvironment)
    : IExceptionFilter
{

    public void OnException(ExceptionContext context)
    {
        // Log the exception
        logger.LogError(context.Exception, "An unhandled exception occurred: {Message}", context.Exception.Message);
        
        // Prevent further exception handling by other filters
        context.ExceptionHandled = true;
        
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
        string message = "An unexpected error occurred.";
        string details = hostEnvironment.IsDevelopment() ? context.Exception.Message : "";
        var errors = new List<string>();

        switch (context.Exception)
        {
            case NotFoundException notFoundException:
                statusCode = HttpStatusCode.NotFound;
                message = notFoundException.Message;
                break;

            case ValidationException validationException:
                statusCode = HttpStatusCode.BadRequest;
                message = validationException.Message;
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
        }

        // using ProblemDetails for standard error response
        var response = new ProblemDetails()
        {
            Status = (int)statusCode,
            Title = message,
            Detail = details,
            Instance = context.HttpContext.Request.Path
        };

        context.Result = new JsonResult(response);
    }
}