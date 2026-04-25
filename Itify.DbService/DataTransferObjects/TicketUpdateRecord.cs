using Itify.DbService.Enums;

namespace Itify.DbService.DataTransferObjects;

public class TicketUpdateRecord
{
    public TicketStatusEnum Status { get; set; }
    public DateTime? ResolvedAt { get; set; }
}
