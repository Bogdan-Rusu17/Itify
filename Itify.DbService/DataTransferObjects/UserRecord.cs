using Itify.DbService.Enums;

namespace Itify.DbService.DataTransferObjects;

public class UserRecord
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public UserRoleEnum Role { get; set; }
}
