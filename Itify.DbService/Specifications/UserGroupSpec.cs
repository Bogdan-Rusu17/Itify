using Ardalis.Specification;
using Itify.DbService.Entities;

namespace Itify.DbService.Specifications;

public sealed class UserGroupSpec : Specification<UserGroup>
{
    public UserGroupSpec(Guid id) => Query.Where(e => e.Id == id);
    public UserGroupSpec(string name) => Query.Where(e => e.Name == name);
    public UserGroupSpec(Guid id, bool includeUsers) => Query.Where(e => e.Id == id).Include(e => e.Users);
}
