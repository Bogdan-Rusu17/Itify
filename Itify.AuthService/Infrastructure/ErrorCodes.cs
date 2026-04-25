using System.Text.Json.Serialization;

namespace Itify.AuthService.Infrastructure;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ErrorCodes
{
    Unknown,
    TechnicalError,
    EntityNotFound,
    UserAlreadyExists,
    WrongPassword,
    CannotAdd,
}
