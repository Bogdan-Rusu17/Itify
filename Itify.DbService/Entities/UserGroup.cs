using Itify.DbService.Infrastructure;

namespace Itify.DbService.Entities;

public class UserGroup : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public ICollection<User> Users { get; set; } = null!;
}
