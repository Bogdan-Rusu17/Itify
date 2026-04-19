using Ardalis.Specification;
using Itify.Database.Repository.Entities;
using Itify.Database.Repository.Enums;

namespace Itify.Services.Specifications;

public sealed class DeviceSpec : Specification<Device>
{
    public DeviceSpec(Guid id)
    {
        Query.Where(device => device.Id == id);
    }

    public DeviceSpec(string serialNumber)
    {
        Query.Where(device => device.SerialNumber == serialNumber);
    }

    public DeviceSpec(Guid categoryId, bool byCategoryId)
    {
        Query.Where(device => device.CategoryId == categoryId);
    }

    public DeviceSpec(Guid categoryId, DeviceStatusEnum status)
    {
        Query.Where(device => device.CategoryId == categoryId && device.Status == status);
    }
}