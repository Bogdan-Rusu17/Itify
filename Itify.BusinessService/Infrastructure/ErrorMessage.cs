using System.Net;
using System.Text.Json.Serialization;

namespace Itify.BusinessService.Infrastructure;

public class ErrorMessage
{
    public string Message { get; }
    public ErrorCodes Code { get; }
    public HttpStatusCode Status { get; }
    [JsonIgnore] public string StackTrace { get; }

    public ErrorMessage(HttpStatusCode status, string message, ErrorCodes code = ErrorCodes.Unknown, string? stackTrace = null)
    {
        Message = message; Status = status; Code = code;
        StackTrace = stackTrace ?? new System.Diagnostics.StackTrace(true).ToString();
    }

    public static ErrorMessage FromException(ServerException ex) => new(ex.Status, ex.Message, ex.Code, ex.StackTrace);
    public static ErrorMessage FromException(Exception ex) => new(HttpStatusCode.InternalServerError, ex.Message, stackTrace: ex.StackTrace);
    public void LogError(ILogger? logger) =>
        logger?.LogError("Error {{ Status: {Status}, Code: {Code}, Message: {Message} }}\r\n{StackTrace}", Status, Code, Message, StackTrace);
}
