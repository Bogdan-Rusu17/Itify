using System.Net;

namespace Itify.Infrastructure.Errors;

/// <summary>
///     Common error messages that may be reused in various places in the code.
/// </summary>
public static class CommonErrors
{
    public static ErrorMessage DeviceRequestCannotUpdateNotOwnedRequestIfEmployee => new(HttpStatusCode.Forbidden,
        "You can only update your own requests!", ErrorCodes.CannotUpdate);

    public static ErrorMessage UnauthorizedDeviceRequestDelete => new(HttpStatusCode.Forbidden,
        "You can only delete your own requests!", ErrorCodes.CannotDelete);

    public static ErrorMessage UserNotFound =>
        new(HttpStatusCode.NotFound, "User doesn't exist!", ErrorCodes.EntityNotFound);

    public static ErrorMessage FileNotFound =>
        new(HttpStatusCode.NotFound, "File not found on disk!", ErrorCodes.PhysicalFileNotFound);

    public static ErrorMessage TechnicalSupport => new(HttpStatusCode.InternalServerError,
        "An unknown error occurred, contact the technical support!", ErrorCodes.TechnicalError);

    public static ErrorMessage DeviceNotFound =>
        new(HttpStatusCode.NotFound, "Device doesn't exist!", ErrorCodes.EntityNotFound);

    public static ErrorMessage UnauthorizedDeviceAddOrUpdate => new(HttpStatusCode.Forbidden,
        "You do not have the required role to add or update devices!", ErrorCodes.CannotAdd);

    public static ErrorMessage DeviceAlreadyExists => new(HttpStatusCode.Conflict,
        "Device with this serial number already exists!", ErrorCodes.UserAlreadyExists);

    public static ErrorMessage DeviceCategoryNotFound => new(HttpStatusCode.NotFound, "Device category doesn't exist!",
        ErrorCodes.EntityNotFound);

    public static ErrorMessage DeviceCategoryAlreadyExists => new(HttpStatusCode.Conflict,
        "A device category with this name already exists!", ErrorCodes.UserAlreadyExists);

    public static ErrorMessage DeviceCategoryUnauthorized => new(HttpStatusCode.Forbidden,
        "You do not have the required role to add or update device categories!", ErrorCodes.CannotAdd);

    public static ErrorMessage DeviceRequestNotFound => new(HttpStatusCode.NotFound, "Device request doesn't exist!",
        ErrorCodes.EntityNotFound);

    public static ErrorMessage UnauthorizedDeviceRequestStatusUpdate => new(HttpStatusCode.Forbidden,
        "You do not have the required role to update the device request's status to Approved or Rejected!",
        ErrorCodes.CannotUpdate);

    public static ErrorMessage UnauthorizedDeviceRequestAlreadyOnCategory => new(HttpStatusCode.Conflict,
        "You already have a pending request for this category!", ErrorCodes.CannotAdd);

    public static ErrorMessage UnauthorizedDeviceRequestDeleteIfResolutionExists => new(HttpStatusCode.BadRequest,
        "Cannot delete a request that has already been resolved!", ErrorCodes.CannotDelete);

    public static ErrorMessage UnauthorizedDeviceRequestUpdateIfResolutionExists => new(HttpStatusCode.BadRequest,
        "Cannot update a request that has already been resolved!", ErrorCodes.CannotUpdate);

    public static ErrorMessage DeviceCategoryHasDevices => new(HttpStatusCode.Conflict,
        "Cannot delete a category that has devices assigned to it!", ErrorCodes.CannotDelete);

    public static ErrorMessage DeviceAssignmentNotFound =>
        new(HttpStatusCode.NotFound, "Device assignment doesn't exist!",
            ErrorCodes.EntityNotFound);

    public static ErrorMessage DeviceNotAvailable => new(HttpStatusCode.Conflict,
        "Device is not available for assignment!", ErrorCodes.CannotAdd);

    public static ErrorMessage DeviceAlreadyAssignedToUser =>
        new(HttpStatusCode.Conflict, "This device is already assigned to this user!", ErrorCodes.CannotAdd);

    public static ErrorMessage DeviceAssignmentUnauthorized =>
        new(HttpStatusCode.Forbidden, "Only admins and IT engineers can manage device assignments!",
            ErrorCodes.CannotAdd);

    public static ErrorMessage DeviceAlreadyReturned =>
        new(HttpStatusCode.BadRequest, "Device already returned!", ErrorCodes.CannotUpdate);
}