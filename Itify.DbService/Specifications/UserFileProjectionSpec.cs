using Ardalis.Specification;
using Itify.DbService.DataTransferObjects;
using Itify.DbService.Entities;
using Microsoft.EntityFrameworkCore;

namespace Itify.DbService.Specifications;

public sealed class UserFileProjectionSpec : Specification<UserFile, UserFileRecord>
{
    public UserFileProjectionSpec() =>
        Query.OrderByDescending(e => e.CreatedAt)
            .Select(e => new UserFileRecord
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                User = new UserRecord { Id = e.User.Id, Name = e.User.Name, Email = e.User.Email, Role = e.User.Role },
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            });

    public UserFileProjectionSpec(Guid userId) : this() => Query.Where(e => e.UserId == userId);

    public UserFileProjectionSpec(string? search) : this()
    {
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = $"%{search.Replace(" ", "%")}%";
            Query.Where(e => EF.Functions.ILike(e.Name, s));
        }
    }
}
