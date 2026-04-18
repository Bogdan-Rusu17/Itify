using Itify.Database.Repository.Enums;

namespace Itify.Services.DataTransferObjects;

public class DeviceUpdateRecord
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public DeviceStatusEnum? Status { get; set; }
    public DateTime? PurchaseDate { get; set; }
}