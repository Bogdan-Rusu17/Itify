using Itify.AuthService.Enums;

namespace Itify.AuthService.DataTransferObjects;

public record CreateUserRequest(string Name, string Email, string Password, UserRoleEnum Role);
