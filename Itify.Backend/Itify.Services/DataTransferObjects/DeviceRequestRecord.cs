using Itify.Database.Repository.Enums;

namespace Itify.Services.DataTransferObjects;

public class DeviceRequestRecord
{
    public Guid Id { get; set; }
    public string Reason { get; set; } = null!;
    public RequestStatusEnum Status { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = null!;
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
}