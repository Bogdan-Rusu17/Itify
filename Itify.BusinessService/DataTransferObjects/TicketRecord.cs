using Itify.BusinessService.Enums;

namespace Itify.BusinessService.DataTransferObjects;

public class TicketRecord
{
    public Guid Id { get; set; }
    public string Description { get; set; } = null!;
    public TicketTypeEnum Type { get; set; }
    public TicketPriorityEnum Priority { get; set; }
    public bool IsUrgent { get; set; }
    public string? AdditionalNotes { get; set; }
    public TicketStatusEnum Status { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public Guid DeviceAssignmentId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = null!;
}
