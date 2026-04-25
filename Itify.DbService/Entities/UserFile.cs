using Itify.DbService.Infrastructure;

namespace Itify.DbService.Entities;

public class UserFile : BaseEntity
{
    public string Path { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
