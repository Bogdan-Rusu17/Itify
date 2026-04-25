using System.Text.Json.Serialization;

namespace Itify.BusinessService.Infrastructure;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ErrorCodes
{
    Unknown, TechnicalError, EntityNotFound, UserAlreadyExists,
    WrongPassword, CannotAdd, CannotUpdate, CannotDelete,
}
