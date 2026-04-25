using Itify.DbService.Enums;

namespace Itify.DbService.DataTransferObjects;

public class DeviceUpdateRecord
{
    public string? Name { get; set; }
    public DeviceStatusEnum? Status { get; set; }
    public DateTime? PurchaseDate { get; set; }
}
