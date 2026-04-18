using System.Diagnostics;
using System.Net;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace MobyLabWebProgramming.Infrastructure.Errors;

/// <summary>
/// This is a simple class to transmit the error information to the client.
/// It includes the message, custom error code to identify te specific error and the HTTP status code to be set on the HTTP response.
/// </summary>
public class ErrorMessage
{
    public string Message { get; }
    public ErrorCodes Code { get; }
    public HttpStatusCode Status { get; }

    [JsonIgnore]
    public string StackTrace { get; }

    public ErrorMessage(HttpStatusCode status, string message, ErrorCodes code = ErrorCodes.Unknown, string? stackTrace = default)
    {
        Message = message;
        Status = status;
        Code = code;
        StackTrace = stackTrace ?? new StackTrace(true).ToString();
    }

    public static ErrorMessage FromException(ServerException exception) => new(exception.Status, exception.Message, stackTrace: exception.StackTrace);
    public static ErrorMessage FromException(Exception exception) => new(HttpStatusCode.InternalServerError, exception.Message, stackTrace: exception.StackTrace);

    public ErrorMessage LogError(ILogger? logger)
    {
        logger?.LogError("ErrorMessage {{ Status: {Status}, Code {Code}, Message: {Message} }}\r\n{StackTrace}", Status, Code, Message, StackTrace);

        return this;
    }
}
