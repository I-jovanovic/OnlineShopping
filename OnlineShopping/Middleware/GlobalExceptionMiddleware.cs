using System.Net;
using System.Text.Json;
using OnlineShopping.Core.Exceptions;

namespace OnlineShopping.Middleware;

/// <summary>
/// Global exception handling middleware
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse
        {
            TraceId = context.TraceIdentifier
        };

        switch (exception)
        {
            case NotFoundException notFoundEx:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Message = notFoundEx.Message;
                response.Type = "NotFound";
                break;

            case BusinessRuleViolationException businessEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = businessEx.Message;
                response.Type = "BusinessRuleViolation";
                break;

            case InsufficientStockException stockEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = stockEx.Message;
                response.Type = "InsufficientStock";
                break;

            case EntityNotFoundException entityEx:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Message = entityEx.Message;
                response.Type = "EntityNotFound";
                break;

            case DuplicateEntityException duplicateEx:
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                response.Message = duplicateEx.Message;
                response.Type = "DuplicateEntity";
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = "An error occurred while processing your request";
                response.Type = "InternalServerError";
                
                if (_environment.IsDevelopment())
                {
                    response.Message = exception.Message;
                    response.Details = exception.StackTrace;
                }
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}

public class ErrorResponse
{
    public string Type { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string TraceId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}