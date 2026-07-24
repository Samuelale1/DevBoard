using DevBoard.Domain.Exceptions;

namespace DevBoard.Api.Middleware;

public sealed class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/json";

            var statusCode = ex switch
            {
                ValidationException => StatusCodes.Status422UnprocessableEntity,
                ConflictException => StatusCodes.Status409Conflict,
                NotFoundException => StatusCodes.Status404NotFound,
                UnauthorizedException => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError
            };

            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsJsonAsync(new
            {
                error = ex.Message
            });
        }
    }
}