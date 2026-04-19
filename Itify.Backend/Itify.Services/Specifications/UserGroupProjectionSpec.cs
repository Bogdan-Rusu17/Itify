using Ardalis.Specification;
using Itify.Database.Repository.Entities;
using Itify.Services.DataTransferObjects;
using Microsoft.EntityFrameworkCore;

namespace Itify.Services.Specifications;

public sealed class UserGroupProjectionSpec : Specification<UserGroup, UserGroupRecord>
{
    public UserGroupProjectionSpec(bool orderByCreatedAt = false)
    {
        Query.OrderByDescending(e => e.CreatedAt, orderByCreatedAt)
            .Select(g => new UserGroupRecord
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description
            });
    }

    public UserGroupProjectionSpec(Guid id) : this()
    {
        Query.Where(g => g.Id == id);
    }

    public UserGroupProjectionSpec(string? searchExpr) : this(true)
    {
        if (string.IsNullOrWhiteSpace(searchExpr)) return;
        var engineSearchExpr = $"%{searchExpr.Replace(" ", "%")}%";
        Query.Where(g => EF.Functions.ILike(g.Name, engineSearchExpr));
    }
}
