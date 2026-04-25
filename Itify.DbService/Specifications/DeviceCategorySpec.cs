using Ardalis.Specification;
using Itify.DbService.Entities;

namespace Itify.DbService.Specifications;

public sealed class DeviceCategorySpec : Specification<DeviceCategory>
{
    public DeviceCategorySpec(Guid id) => Query.Where(e => e.Id == id);
    public DeviceCategorySpec(string name) => Query.Where(e => e.Name == name);
}
