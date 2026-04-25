using Itify.DbService.Enums;
using Itify.DbService.Infrastructure;

namespace Itify.DbService.Entities;

public class Ticket : BaseEntity
{
    public string Description { get; set; } = null!;
    public TicketTypeEnum Type { get; set; }
    public TicketPriorityEnum Priority { get; set; }
    public bool IsUrgent { get; set; }
    public string? AdditionalNotes { get; set; }
    public TicketStatusEnum Status { get; set; }
    public DateTime? ResolvedAt { get; set; }

    public Guid DeviceAssignmentId { get; set; }
    public DeviceAssignment DeviceAssignment { get; set; } = null!;

    // UserId references the User in postgres-users — no navigation property across DBs
    public Guid UserId { get; set; }
}
