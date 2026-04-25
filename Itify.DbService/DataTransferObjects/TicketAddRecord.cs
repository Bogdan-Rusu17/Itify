using Itify.DbService.Enums;

namespace Itify.DbService.DataTransferObjects;

public class TicketAddRecord
{
    public string Description { get; set; } = null!;
    public TicketTypeEnum Type { get; set; }
    public TicketPriorityEnum Priority { get; set; }
    public bool IsUrgent { get; set; }
    public string? AdditionalNotes { get; set; }
    public Guid DeviceAssignmentId { get; set; }
    public Guid UserId { get; set; }
}
