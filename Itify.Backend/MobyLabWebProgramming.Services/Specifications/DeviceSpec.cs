using Ardalis.Specification;
using MobyLabWebProgramming.Database.Repository.Entities;

namespace MobyLabWebProgramming.Services.Specifications;

public sealed class DeviceSpec : Specification<Device>
{
    public DeviceSpec(Guid id) => Query.Where(device => device.Id == id);
}