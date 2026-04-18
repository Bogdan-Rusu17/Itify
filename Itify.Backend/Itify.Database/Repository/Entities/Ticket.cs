using Itify.Database.Repository.Enums;
using Itify.Infrastructure.BaseObjects;

namespace Itify.Database.Repository.Entities;

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

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}