using Itify.DbService.Enums;
using Itify.DbService.Infrastructure;

namespace Itify.DbService.Entities;

public class Device : BaseEntity
{
    public string Name { get; set; } = null!;
    public string SerialNumber { get; set; } = null!;
    public DeviceStatusEnum Status { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public Guid CategoryId { get; set; }
    public DeviceCategory Category { get; set; } = null!;
    public ICollection<DeviceAssignment> Assignments { get; set; } = null!;
}
