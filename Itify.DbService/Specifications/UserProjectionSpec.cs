using Ardalis.Specification;
using Itify.DbService.DataTransferObjects;
using Itify.DbService.Entities;
using Itify.DbService.Enums;
using Microsoft.EntityFrameworkCore;

namespace Itify.DbService.Specifications;

public sealed class UserProjectionSpec : Specification<User, UserRecord>
{
    public UserProjectionSpec() =>
        Query.OrderByDescending(e => e.CreatedAt)
            .Select(e => new UserRecord { Id = e.Id, Name = e.Name, Email = e.Email, Role = e.Role });

    public UserProjectionSpec(Guid id) : this() => Query.Where(e => e.Id == id);

    public UserProjectionSpec(Guid groupId, bool inGroup) : this() =>
        Query.Where(e => e.UserGroups.Any(g => g.Id == groupId));

    public UserProjectionSpec(string? search, UserRoleEnum? role) : this()
    {
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = $"%{search.Replace(" ", "%")}%";
            Query.Where(e => EF.Functions.ILike(e.Name, s));
        }
        if (role.HasValue)
            Query.Where(e => e.Role == role.Value);
    }
}
