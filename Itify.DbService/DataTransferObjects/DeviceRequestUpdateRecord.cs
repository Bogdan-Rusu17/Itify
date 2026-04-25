using Itify.DbService.Enums;

namespace Itify.DbService.DataTransferObjects;

public class DeviceRequestUpdateRecord
{
    public RequestStatusEnum? Status { get; set; }
    public string? Reason { get; set; }
}
