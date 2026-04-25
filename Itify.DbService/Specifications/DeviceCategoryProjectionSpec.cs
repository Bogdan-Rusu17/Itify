using Ardalis.Specification;
using Itify.DbService.DataTransferObjects;
using Itify.DbService.Entities;
using Microsoft.EntityFrameworkCore;

namespace Itify.DbService.Specifications;

public sealed class DeviceCategoryProjectionSpec : Specification<DeviceCategory, DeviceCategoryRecord>
{
    public DeviceCategoryProjectionSpec() =>
        Query.OrderByDescending(e => e.CreatedAt)
            .Select(e => new DeviceCategoryRecord { Id = e.Id, Name = e.Name, Description = e.Description });

    public DeviceCategoryProjectionSpec(Guid id) : this() => Query.Where(e => e.Id == id);

    public DeviceCategoryProjectionSpec(string? search) : this()
    {
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = $"%{search.Replace(" ", "%")}%";
            Query.Where(e => EF.Functions.ILike(e.Name, s));
        }
    }
}
