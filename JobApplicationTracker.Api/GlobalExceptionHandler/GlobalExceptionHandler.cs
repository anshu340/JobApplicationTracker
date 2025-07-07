using System.Diagnostics;
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
        
        var traceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;

        logger.LogError(context.Exception, $"An exception occured {context.Exception.Message ?? ""} with traceId ", traceId);

        // Prevent further exception handling by other filters/middleware 
        context.ExceptionHandled = true;

        HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
        string message = "An unexpected error occurred.";
        
        // provide full exception details in development environement
        string? details = hostEnvironment.IsDevelopment() ? context.Exception.ToString() : null; 
        var errors = new List<string>();

        switch (context.Exception)
        {
            case NotFoundException notFoundException:
                statusCode = HttpStatusCode.NotFound;
                message = notFoundException.Message;
                details = hostEnvironment.IsDevelopment() ? notFoundException.ToString() : notFoundException.Message;
                break;

            case ValidationException validationException:
                statusCode = HttpStatusCode.BadRequest;
                message = validationException.Message;
                errors.AddRange(validationException.Errors);
                details = hostEnvironment.IsDevelopment() ? validationException.ToString() : validationException.Message;
                break;

            case ArgumentException argEx:
                statusCode = HttpStatusCode.BadRequest;
                message = argEx.Message;
                details = hostEnvironment.IsDevelopment() ? argEx.ToString() : argEx.Message;
                break;

            case UnauthorizedAccessException _:
                statusCode = HttpStatusCode.Unauthorized;
                message = "Access denied. You are not authorized to perform this action.";
                details = hostEnvironment.IsDevelopment() ? context.Exception.ToString() : message;
                break;
        }

        // Using ProblemDetails for standard error response 
        var response = new ProblemDetails()
        {
            Status = (int)statusCode,
            Title = message,
            Detail = details, 
            Instance = context.HttpContext.Request.Path 
        };
        
        if (errors.Any())
        {
            response.Extensions["errors"] = errors;
        }

        context.Result = new JsonResult(response)
        {
            StatusCode = (int)statusCode 
        };
    }


    // private (string title, HttpStatusCode statusCode, string details) GetProblemDetails(Exception exception)
    // {
    //     return exception switch
    //     {
    //         NotFoundException => ($"Sorry. We could not find what you are looking for. Try checking again later. {exception.Message}",
    //             HttpStatusCode.NotFound,hostEnvironment.IsDevelopment() ? exception.ToString() : exception.Message),
    //         ValidationException => ($"Please check all the details again. Try again later. {exception.Message}", 
    //             HttpStatusCode.BadRequest, hostEnvironment.IsDevelopment() ? exception.ToString() : exception.Message )
    //         
    //     };
    // }
}