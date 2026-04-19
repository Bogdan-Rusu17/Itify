using Ardalis.Specification;
using Itify.Database.Repository.Entities;

namespace Itify.Services.Specifications;

public sealed class DeviceAssignmentSpec : Specification<DeviceAssignment>
{
    public DeviceAssignmentSpec(Guid id)
    {
        Query.Where(da => da.Id == id);
    }

    public DeviceAssignmentSpec(Guid deviceId, bool withDeviceId)
    {
        Query.Where(da => da.DeviceId == deviceId && da.ReturnedAt == null);
    }
}