using Itify.BusinessService.Enums;

namespace Itify.BusinessService.DataTransferObjects;

public class DeviceUpdateRequest
{
    public string? Name { get; set; }
    public DeviceStatusEnum? Status { get; set; }
    public DateTime? PurchaseDate { get; set; }
}
