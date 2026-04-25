using Ardalis.Specification;
using Itify.DbService.DataTransferObjects;
using Itify.DbService.Entities;
using Microsoft.EntityFrameworkCore;

namespace Itify.DbService.Specifications;

public sealed class UserGroupProjectionSpec : Specification<UserGroup, UserGroupRecord>
{
    public UserGroupProjectionSpec() =>
        Query.OrderByDescending(e => e.CreatedAt)
            .Select(e => new UserGroupRecord { Id = e.Id, Name = e.Name, Description = e.Description });

    public UserGroupProjectionSpec(Guid id) : this() => Query.Where(e => e.Id == id);

    public UserGroupProjectionSpec(string? search) : this()
    {
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = $"%{search.Replace(" ", "%")}%";
            Query.Where(e => EF.Functions.ILike(e.Name, s));
        }
    }
}
