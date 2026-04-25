using Itify.DbService.Enums;
using Itify.DbService.Infrastructure;

namespace Itify.DbService.Entities;

public class DeviceRequest : BaseEntity
{
    public string Reason { get; set; } = null!;
    public RequestStatusEnum Status { get; set; }

    // UserId references the User in postgres-users — no navigation property across DBs
    public Guid UserId { get; set; }

    public Guid CategoryId { get; set; }
    public DeviceCategory Category { get; set; } = null!;
}
