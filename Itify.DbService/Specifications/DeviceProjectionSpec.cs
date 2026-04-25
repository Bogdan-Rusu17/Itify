using Ardalis.Specification;
using Itify.DbService.DataTransferObjects;
using Itify.DbService.Entities;
using Microsoft.EntityFrameworkCore;

namespace Itify.DbService.Specifications;

public sealed class DeviceProjectionSpec : Specification<Device, DeviceRecord>
{
    public DeviceProjectionSpec() =>
        Query.Include(e => e.Category)
            .OrderByDescending(e => e.CreatedAt)
            .Select(e => new DeviceRecord
            {
                Id = e.Id, Name = e.Name, SerialNumber = e.SerialNumber,
                Status = e.Status, PurchaseDate = e.PurchaseDate,
                CategoryId = e.CategoryId, CategoryName = e.Category.Name
            });

    public DeviceProjectionSpec(Guid id) : this() => Query.Where(e => e.Id == id);

    public DeviceProjectionSpec(string? search, Guid? assignedToUserId) : this()
    {
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = $"%{search.Replace(" ", "%")}%";
            Query.Where(e => EF.Functions.ILike(e.Name, s));
        }
        if (assignedToUserId.HasValue)
            Query.Where(e => e.Assignments.Any(a => a.UserId == assignedToUserId.Value && a.ReturnedAt == null));
    }
}
