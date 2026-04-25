using Itify.DbService.Infrastructure;

namespace Itify.DbService.Entities;

public class DeviceAssignment : BaseEntity
{
    public Guid DeviceId { get; set; }
    public Device Device { get; set; } = null!;

    // UserId references the User in postgres-users — no navigation property across DBs
    public Guid UserId { get; set; }

    public DateTime AssignedAt { get; set; }
    public DateTime? ReturnedAt { get; set; }

    public Ticket? Ticket { get; set; }
}
