using Ardalis.Specification;
using Itify.Database.Repository.Entities;

namespace Itify.Services.Specifications;

public sealed class DeviceCategorySpec : Specification<DeviceCategory>
{
    public DeviceCategorySpec(Guid id)
    {
        Query.Where(dc => dc.Id == id);
    }

    public DeviceCategorySpec(string name)
    {
        Query.Where(dc => dc.Name == name);
    }
}