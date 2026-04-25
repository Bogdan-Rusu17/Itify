using Itify.DbService.Enums;
using Itify.DbService.Infrastructure;

namespace Itify.DbService.Entities;

public class User : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public UserRoleEnum Role { get; set; }

    public ICollection<UserFile> UserFiles { get; set; } = null!;
    public ICollection<UserGroup> UserGroups { get; set; } = null!;
}
