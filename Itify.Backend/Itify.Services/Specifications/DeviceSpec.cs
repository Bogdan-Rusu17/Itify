using Ardalis.Specification;
using Itify.Database.Repository.Entities;

namespace Itify.Services.Specifications;

public sealed class DeviceSpec : Specification<Device>
{
    public DeviceSpec(Guid id) => Query.Where(device => device.Id == id);
}