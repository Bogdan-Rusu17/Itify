using MobyLabWebProgramming.Infrastructure.BaseObjects;

namespace MobyLabWebProgramming.Database.Repository.Entities;

public class UserGroup : BaseEntity
{
    public string Name { get; set; } = null!;
    public string?  Description { get; set; }

    public ICollection<User> Users { get; set; } = null!;
}