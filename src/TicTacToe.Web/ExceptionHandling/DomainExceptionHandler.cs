using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TicTacToe.Domain.Exceptions;

namespace TicTacToe.Web.ExceptionHandling;

public sealed class DomainExceptionHandler(ILogger<DomainExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, title) = exception switch
        {
            GameNotFoundException => (StatusCodes.Status404NotFound, "Game Not Found"),
            GameValidationException => (StatusCodes.Status400BadRequest, "Invalid game state"),
            InvalidMoveException => (StatusCodes.Status400BadRequest, "Invalid move"),
            UserAlreadyExistsException => (StatusCodes.Status409Conflict, "Conflict"),
            UnauthorizedException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            _ => (0, null as string)
        };

        if (status == 0)
        {
            logger.LogError(exception, "Unhandled Exception");
            return false;
        }

        if (status >= 400 && status < 500)
            logger.LogWarning("Client error: {Title}", title);
        
        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails()
        {
            Status = status,
            Title = title,
            Detail = exception.Message
        }, cancellationToken);

        return true;
    }
}