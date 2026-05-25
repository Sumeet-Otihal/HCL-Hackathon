using HotelBooking.Core.Helpers;

namespace HotelBooking.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception on {Method} {Path}",
                context.Request.Method, context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        var response = ex switch
        {
            Core.Exceptions.NotFoundException => ApiResponse<object>.Fail(ex.Message, 404),
            Core.Exceptions.UnauthorizedException => ApiResponse<object>.Fail(ex.Message, 401),
            Core.Exceptions.ForbiddenException => ApiResponse<object>.Fail(ex.Message, 403),
            Core.Exceptions.ValidationException validationEx => ApiResponse<object>.Fail(
                ex.Message, 400, validationEx.Errors),
            _ => ApiResponse<object>.Fail(
                _env.IsProduction() ? "An unexpected error occurred." : ex.Message, 500)
        };

        context.Response.StatusCode = response.StatusCode;
        await context.Response.WriteAsJsonAsync(response);
    }
}
