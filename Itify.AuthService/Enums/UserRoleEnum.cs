using System.Text.Json.Serialization;

namespace Itify.AuthService.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserRoleEnum
{
    Admin,
    ItEngineer,
    Employee,
}
