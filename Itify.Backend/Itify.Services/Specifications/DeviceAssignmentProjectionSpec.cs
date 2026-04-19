using Ardalis.Specification;
using Itify.Database.Repository.Entities;
using Itify.Services.DataTransferObjects;
using Microsoft.EntityFrameworkCore;

namespace Itify.Services.Specifications;

public sealed class DeviceAssignmentProjectionSpec : Specification<DeviceAssignment, DeviceAssignmentRecord>
{
    public DeviceAssignmentProjectionSpec(bool orderByCreatedAt = false)
    {
        Query.OrderByDescending(e => e.CreatedAt, orderByCreatedAt)
            .Select(da => new DeviceAssignmentRecord
            {
                Id = da.Id,
                DeviceId = da.DeviceId,
                DeviceName = da.Device.Name,
                SerialNumber = da.Device.SerialNumber,
                UserId = da.UserId,
                UserName = da.User.Name,
                AssignedAt = da.AssignedAt,
                ReturnedAt = da.ReturnedAt
            });
    }

    public DeviceAssignmentProjectionSpec(Guid id) : this()
    {
        Query.Where(da => da.Id == id);
    }

    public DeviceAssignmentProjectionSpec(string? searchExpr) : this(true)
    {
        if (string.IsNullOrWhiteSpace(searchExpr)) return;
        var engineSearchExpr = $"%{searchExpr.Replace(" ", "%")}%";
        Query.Where(e => EF.Functions.ILike(e.Device.Name, engineSearchExpr));
    }

    public DeviceAssignmentProjectionSpec(Guid userId, bool withUserId) : this(true)
    {
        Query.Where(e => e.UserId == userId);
    }
}