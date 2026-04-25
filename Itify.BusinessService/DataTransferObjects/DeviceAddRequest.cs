using Itify.BusinessService.Enums;

namespace Itify.BusinessService.DataTransferObjects;

public class DeviceAddRequest
{
    public string Name { get; set; } = null!;
    public string SerialNumber { get; set; } = null!;
    public DeviceStatusEnum Status { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public Guid CategoryId { get; set; }
}
