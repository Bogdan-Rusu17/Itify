using MobyLabWebProgramming.Database.Repository.Enums;
using MobyLabWebProgramming.Infrastructure.BaseObjects;

namespace MobyLabWebProgramming.Database.Repository.Entities;

public class Device : BaseEntity
{
    public string Name { get; set; } = null!;
    public string SerialNumber { get; set; } = null!;
    public DeviceStatusEnum  Status { get; set; }
    public DateTime? PurchaseDate { get; set; }
    
    public Guid CategoryId { get; set; }
    public DeviceCategory Category { get; set; } = null!;
    
    public ICollection<DeviceAssignment> Assignments { get; set; } = null!;
}