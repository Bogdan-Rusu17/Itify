using Itify.Database.Repository.Enums;

namespace Itify.Services.DataTransferObjects;

public class DeviceRecord
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string SerialNumber { get; set; } = null!;
    public DeviceStatusEnum Status { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
}