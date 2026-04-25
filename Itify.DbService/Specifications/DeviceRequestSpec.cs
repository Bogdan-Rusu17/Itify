using Ardalis.Specification;
using Itify.DbService.Entities;
using Microsoft.EntityFrameworkCore;

namespace Itify.DbService.Specifications;

public sealed class DeviceRequestSpec : Specification<DeviceRequest>
{
    public DeviceRequestSpec(Guid id) => Query.Include(e => e.Category).Where(e => e.Id == id);
    public DeviceRequestSpec(Guid userId, Guid categoryId) => Query.Where(e => e.UserId == userId && e.CategoryId == categoryId);
    public DeviceRequestSpec(string? search, Guid? userId)
    {
        Query.Include(e => e.Category).OrderByDescending(e => e.CreatedAt);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = $"%{search.Replace(" ", "%")}%";
            Query.Where(e => EF.Functions.ILike(e.Reason, s));
        }
        if (userId.HasValue)
            Query.Where(e => e.UserId == userId.Value);
    }
}
