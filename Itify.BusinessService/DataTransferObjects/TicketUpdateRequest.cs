using Itify.BusinessService.Enums;

namespace Itify.BusinessService.DataTransferObjects;

public class TicketUpdateRequest
{
    public TicketStatusEnum Status { get; set; }
}
