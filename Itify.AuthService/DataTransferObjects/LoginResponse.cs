using Itify.AuthService.Enums;

namespace Itify.AuthService.DataTransferObjects;

public record LoginResponse(string Token, Guid UserId, string Name, string Email, UserRoleEnum Role);
