using Itify.Database.Repository.Enums;

namespace Itify.Services.DataTransferObjects;

public class DeviceRequestUpdateRecord
{
    public Guid Id { get; set; }
    public RequestStatusEnum? Status { get; set; }
    public string? Reason { get; set; }
}