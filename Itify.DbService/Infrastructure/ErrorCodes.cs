using System.Text.Json.Serialization;

namespace Itify.DbService.Infrastructure;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ErrorCodes
{
    Unknown,
    TechnicalError,
    EntityNotFound,
    UserAlreadyExists,
    WrongPassword,
    CannotAdd,
    CannotUpdate,
    CannotDelete,
}
