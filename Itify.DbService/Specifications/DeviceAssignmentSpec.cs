using Ardalis.Specification;
using Itify.DbService.Entities;
using Microsoft.EntityFrameworkCore;

namespace Itify.DbService.Specifications;

public sealed class DeviceAssignmentSpec : Specification<DeviceAssignment>
{
    public DeviceAssignmentSpec(Guid id) => Query.Where(e => e.Id == id).Include(e => e.Device);
    public DeviceAssignmentSpec(Guid deviceId, bool activeOnly) => Query.Where(e => e.DeviceId == deviceId && e.ReturnedAt == null);
    public DeviceAssignmentSpec(string? search, Guid? userId)
    {
        Query.Include(e => e.Device).OrderByDescending(e => e.CreatedAt);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = $"%{search.Replace(" ", "%")}%";
            Query.Where(e => EF.Functions.ILike(e.Device.Name, s));
        }
        if (userId.HasValue)
            Query.Where(e => e.UserId == userId.Value);
    }
}
