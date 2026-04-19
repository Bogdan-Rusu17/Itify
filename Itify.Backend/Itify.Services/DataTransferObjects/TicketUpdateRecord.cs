using Itify.Database.Repository.Enums;

namespace Itify.Services.DataTransferObjects;

public class TicketUpdateRecord
{
    public Guid Id { get; set; }
    public TicketStatusEnum Status { get; set; }
}
