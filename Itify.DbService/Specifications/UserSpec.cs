using Ardalis.Specification;
using Itify.DbService.Entities;
using Itify.DbService.Enums;
using Microsoft.EntityFrameworkCore;

namespace Itify.DbService.Specifications;

public sealed class UserSpec : Specification<User>
{
    public UserSpec(Guid id) => Query.Where(e => e.Id == id);
    public UserSpec(string email) => Query.Where(e => e.Email == email);
    public UserSpec(List<Guid> ids) => Query.Where(e => ids.Contains(e.Id));
    public UserSpec(string? search, UserRoleEnum? role)
    {
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = $"%{search.Replace(" ", "%")}%";
            Query.Where(e => EF.Functions.ILike(e.Name, s));
        }
        if (role.HasValue)
            Query.Where(e => e.Role == role.Value);
        Query.OrderByDescending(e => e.CreatedAt);
    }
}
