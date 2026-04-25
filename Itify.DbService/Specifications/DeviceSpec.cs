using Ardalis.Specification;
using Itify.DbService.Entities;
using Itify.DbService.Enums;

namespace Itify.DbService.Specifications;

public sealed class DeviceSpec : Specification<Device>
{
    public DeviceSpec(Guid id) => Query.Where(e => e.Id == id);
    public DeviceSpec(string serialNumber) => Query.Where(e => e.SerialNumber == serialNumber);
    public DeviceSpec(Guid categoryId, DeviceStatusEnum status) => Query.Where(e => e.CategoryId == categoryId && e.Status == status);
}
