using Ardalis.Specification;
using Itify.Database.Repository.Entities;
using Itify.Services.DataTransferObjects;
using Microsoft.EntityFrameworkCore;

namespace Itify.Services.Specifications;

public class DeviceCategoryProjectionSpec : Specification<DeviceCategory, DeviceCategoryRecord>
{
    public DeviceCategoryProjectionSpec(bool orderByCreatedAt = false)
    {
        Query.OrderByDescending(dc => dc.CreatedAt, orderByCreatedAt)
            .Select(dc => new DeviceCategoryRecord
            {
                Id = dc.Id,
                Name = dc.Name,
                Description = dc.Description
            });
    }

    public DeviceCategoryProjectionSpec(Guid id) : this()
    {
        Query.Where(dc => dc.Id == id);
    }

    public DeviceCategoryProjectionSpec(string? searchExpr) : this()
    {
        if (string.IsNullOrWhiteSpace(searchExpr)) return;

        var engineSearchExpr = $"%{searchExpr.Replace(" ", "%")}%";
        Query.Where(dc => EF.Functions.ILike(dc.Name, engineSearchExpr));
    }
}