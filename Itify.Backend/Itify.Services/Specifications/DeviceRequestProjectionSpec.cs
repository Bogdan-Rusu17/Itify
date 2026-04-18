using Ardalis.Specification;
using Itify.Database.Repository.Entities;
using Itify.Services.DataTransferObjects;
using Microsoft.EntityFrameworkCore;

namespace Itify.Services.Specifications;

public class DeviceRequestProjectionSpec : Specification<DeviceRequest, DeviceRequestRecord>
{
    public DeviceRequestProjectionSpec(bool orderByCreatedAt = false)
    {
        Query.OrderByDescending(dr => dr.CreatedAt, orderByCreatedAt)
            .Select(dr => new DeviceRequestRecord
            {
                Id = dr.Id,
                Reason = dr.Reason,
                Status = dr.Status,
                UserId = dr.UserId,
                UserName = dr.User.Name,
                CategoryId = dr.CategoryId,
                CategoryName = dr.Category.Name
            });
    }

    public DeviceRequestProjectionSpec(Guid id) : this()
    {
        Query.Where(dr => dr.Id == id);
    }

    public DeviceRequestProjectionSpec(string? searchExpr) : this(true)
    {
        if (string.IsNullOrWhiteSpace(searchExpr)) return;
        var engineSearchExpr = $"%{searchExpr.Replace(" ", "%")}%";
        Query.Where(dr => EF.Functions.ILike(dr.Reason, engineSearchExpr));
    }
}