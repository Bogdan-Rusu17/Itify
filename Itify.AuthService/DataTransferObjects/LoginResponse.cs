using Itify.AuthService.Enums;

namespace Itify.AuthService.DataTransferObjects;

public class LoginResponse
{
    public string Token { get; set; } = null!;
    public Guid UserId { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public UserRoleEnum Role { get; set; }
}
