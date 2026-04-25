using Itify.AuthService.Enums;

namespace Itify.AuthService.DataTransferObjects;

public class DbUserRecord
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public UserRoleEnum Role { get; set; }
}
