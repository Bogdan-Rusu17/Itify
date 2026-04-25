using System.Net.Mime;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Itify.DbService.Infrastructure;

public class GlobalExceptionHandlerMiddleware(ILogger<GlobalExceptionHandlerMiddleware> logger, RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var response = context.Response;
            response.ContentType = MediaTypeNames.Application.Json;
            var error = ex is ServerException se ? ErrorMessage.FromException(se) : ErrorMessage.FromException(ex);
            response.StatusCode = (int)error.Status;
            await response.WriteAsync(JsonSerializer.Serialize(new { error.Message, error.Code }));
            error.LogError(logger);
        }
    }
}
