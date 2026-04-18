using System.Net;

namespace Itify.Infrastructure.Errors;

/// <summary>
/// Common error messages that may be reused in various places in the code.
/// </summary>
public static class CommonErrors
{
    public static ErrorMessage UserNotFound => new(HttpStatusCode.NotFound, "User doesn't exist!", ErrorCodes.EntityNotFound);
    public static ErrorMessage FileNotFound => new(HttpStatusCode.NotFound, "File not found on disk!", ErrorCodes.PhysicalFileNotFound);
    public static ErrorMessage TechnicalSupport => new(HttpStatusCode.InternalServerError, "An unknown error occurred, contact the technical support!", ErrorCodes.TechnicalError);
    public static ErrorMessage DeviceNotFound => new(HttpStatusCode.NotFound, "Device doesn't exist!", ErrorCodes.EntityNotFound);
    public static ErrorMessage UnauthorizedDeviceAddOrUpdate => new(HttpStatusCode.Forbidden, "You do not have the required role to add or update devices!", ErrorCodes.CannotAdd);
    public static ErrorMessage DeviceAlreadyExists => new(HttpStatusCode.Conflict, "Device with this serial number already exists!", ErrorCodes.UserAlreadyExists);
    public static ErrorMessage DeviceCategoryNotFound => new(HttpStatusCode.NotFound, "Device category doesn't exist!", ErrorCodes.EntityNotFound);
    public static ErrorMessage DeviceCategoryAlreadyExists => new(HttpStatusCode.Conflict, "A device category with this name already exists!", ErrorCodes.EntityNotFound);
    public static ErrorMessage DeviceCategoryUnauthorized => new(HttpStatusCode.Forbidden, "You do not have the required role to add or update device categories!", ErrorCodes.CannotAdd);
    
    }
