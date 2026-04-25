using Itify.BusinessService.Enums;

namespace Itify.BusinessService.DataTransferObjects;

public class DeviceRequestUpdateRequest
{
    public string? Reason { get; set; }
    public RequestStatusEnum? Status { get; set; }
}
