using Itify.DbService.Enums;

namespace Itify.DbService.DataTransferObjects;

public class UserUpdateRecord
{
    public string? Name { get; set; }
    public string? Password { get; set; }
    public UserRoleEnum? Role { get; set; }
}
