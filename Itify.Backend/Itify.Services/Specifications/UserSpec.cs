using Ardalis.Specification;
using Itify.Database.Repository.Entities;
using Itify.Database.Repository.Enums;

namespace Itify.Services.Specifications;

public sealed class UserSpec : Specification<User>
{
    public UserSpec(Guid id) => Query.Where(e => e.Id == id);

    public UserSpec(string email) => Query.Where(e => e.Email == email);

    public UserSpec(List<UserRoleEnum> roles) => Query.Where(e => roles.Contains(e.Role));
}
