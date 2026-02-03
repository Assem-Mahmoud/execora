using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Execora.Api.Filters;

/// <summary>
/// Global exception filter that handles all uncaught exceptions
/// </summary>
public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionFilter(
        ILogger<GlobalExceptionFilter> logger,
        IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "Unhandled exception occurred: {Message}", context.Exception.Message);

        var response = new ErrorResponse
        {
            Type = "Error",
            Message = GetErrorMessage(context.Exception),
            Code = GetErrorCode(context.Exception),
            StatusCode = (int)GetStatusCode(context.Exception)
        };

        // Include stack trace in development
        if (_environment.IsDevelopment())
        {
            response.StackTrace = context.Exception.StackTrace;
            response.InnerException = context.Exception.InnerException?.Message;
        }

        context.Result = new ObjectResult(response)
        {
            StatusCode = response.StatusCode
        };

        context.ExceptionHandled = true;
    }

    private static string GetErrorMessage(Exception exception)
    {
        return exception switch
        {
            ArgumentNullException => "A required parameter was missing.",
            UnauthorizedAccessException => "You do not have permission to access this resource.",
            ArgumentException or InvalidOperationException => exception.Message,
            // Add more specific exception types as needed
            _ => "An unexpected error occurred. Please try again later."
        };
    }

    private static string GetErrorCode(Exception exception)
    {
        return exception switch
        {
            ArgumentNullException => "MISSING_ARGUMENT",
            UnauthorizedAccessException => "UNAUTHORIZED",
            ArgumentException or InvalidOperationException => "INVALID_ARGUMENT",
            _ => "INTERNAL_ERROR"
        };
    }

    private static HttpStatusCode GetStatusCode(Exception exception)
    {
        return exception switch
        {
            UnauthorizedAccessException => HttpStatusCode.Forbidden,
            ArgumentNullException or ArgumentException or InvalidOperationException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError
        };
    }
}

/// <summary>
/// Standard error response structure
/// </summary>
public class ErrorResponse
{
    public string Type { get; set; } = "Error";
    public string Message { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public string? StackTrace { get; set; }
    public string? InnerException { get; set; }
}

/// <summary>
/// Extension methods for GlobalExceptionFilter
/// </summary>
public static class GlobalExceptionFilterExtensions
{
    public static IServiceCollection AddGlobalExceptionFilter(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add<GlobalExceptionFilter>();
        });

        return services;
    }
}
