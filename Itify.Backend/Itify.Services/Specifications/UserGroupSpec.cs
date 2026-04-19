using Ardalis.Specification;
using Itify.Database.Repository.Entities;

namespace Itify.Services.Specifications;

public sealed class UserGroupSpec : Specification<UserGroup>
{
    public UserGroupSpec(Guid id)
    {
        Query.Where(g => g.Id == id);
    }

    public UserGroupSpec(string name)
    {
        Query.Where(g => g.Name == name);
    }

    public UserGroupSpec(Guid id, bool includeUsers)
    {
        Query.Where(g => g.Id == id).Include(g => g.Users);
    }
}
