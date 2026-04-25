using System.Net;

namespace Itify.BusinessService.Infrastructure;

public class ServerException(HttpStatusCode status, string message, ErrorCodes code = ErrorCodes.Unknown) : Exception(message)
{
    public HttpStatusCode Status { get; } = status;
    public ErrorCodes Code { get; } = code;
}

public class NotFoundException(string message) : ServerException(HttpStatusCode.NotFound, message, ErrorCodes.EntityNotFound);
public class ForbiddenException(string message) : ServerException(HttpStatusCode.Forbidden, message, ErrorCodes.CannotAdd);
public class ConflictException(string message) : ServerException(HttpStatusCode.Conflict, message, ErrorCodes.CannotAdd);
public class BadRequestException(string message) : ServerException(HttpStatusCode.BadRequest, message, ErrorCodes.Unknown);
