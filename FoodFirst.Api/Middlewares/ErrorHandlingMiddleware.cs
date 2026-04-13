using System.Net;
using System.Text.Json;

namespace FoodFirst.Api.Middlewares;

public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IHostEnvironment env)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogWarning(ex, "Unauthorized");
            await WriteError(context, HttpStatusCode.Unauthorized, "Unauthorized");
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogInformation(ex, "Not found");
            await WriteError(context, HttpStatusCode.NotFound, "Resource not found");
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Bad request");
            var message = env.IsDevelopment() ? ex.Message : "Bad request";
            await WriteError(context, HttpStatusCode.BadRequest, message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            await WriteError(context, HttpStatusCode.InternalServerError, "Internal server error");
        }
    }

    private static Task WriteError(HttpContext ctx, HttpStatusCode code, string message)
    {
        ctx.Response.StatusCode = (int)code;
        ctx.Response.ContentType = "application/problem+json";
        return ctx.Response.WriteAsync(JsonSerializer.Serialize(new
        {
            type = "about:blank",
            title = message,
            status = (int)code
        }));
    }
}
