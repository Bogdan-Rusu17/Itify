using Ardalis.Specification;
using Itify.Database.Repository.Entities;

namespace Itify.Services.Specifications;

public sealed class DeviceRequestSpec : Specification<DeviceRequest>
{
    public DeviceRequestSpec(Guid id)
    {
        Query.Where(e => e.Id == id);
    }

    public DeviceRequestSpec(Guid userId, Guid categoryId)
    {
        Query.Where(e => e.UserId == userId && e.CategoryId == categoryId);
    }
}