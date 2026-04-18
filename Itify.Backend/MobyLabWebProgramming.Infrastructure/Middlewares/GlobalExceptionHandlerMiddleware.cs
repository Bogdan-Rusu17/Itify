using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MobyLabWebProgramming.Infrastructure.Errors;
using MobyLabWebProgramming.Infrastructure.Responses;

namespace MobyLabWebProgramming.Infrastructure.Middlewares;

/// <summary>
/// This is the global exception handler/middleware, when a HTTP request arrives it is invoked, if an uncaught exception is caught here it sends a error message back to the client.
/// </summary>
public class GlobalExceptionHandlerMiddleware(ILogger<GlobalExceptionHandlerMiddleware> logger, RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context); // Here the next middleware is invoked, the last middleware invoked calls the corresponding controller method for the specified route.
        }
        catch (Exception ex)
        {
            var response = context.Response;
            response.ContentType = MediaTypeNames.Application.Json;
            var responseError = ex is ServerException serverException ? ErrorMessage.FromException(serverException) : ErrorMessage.FromException(ex);
            response.StatusCode = (int) responseError.Status;
            await response.WriteAsync(JsonSerializer.Serialize(RequestResponse.FromError(responseError.LogError(logger))));
        }
    }
}
