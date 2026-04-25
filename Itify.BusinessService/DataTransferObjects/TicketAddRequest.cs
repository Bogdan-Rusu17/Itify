using Itify.BusinessService.Enums;

namespace Itify.BusinessService.DataTransferObjects;

public class TicketAddRequest
{
    public string Description { get; set; } = null!;
    public TicketTypeEnum Type { get; set; }
    public TicketPriorityEnum Priority { get; set; }
    public bool IsUrgent { get; set; }
    public string? AdditionalNotes { get; set; }
    public Guid DeviceAssignmentId { get; set; }
}
