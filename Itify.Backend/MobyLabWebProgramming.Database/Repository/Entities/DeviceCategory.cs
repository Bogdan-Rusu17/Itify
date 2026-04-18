using MobyLabWebProgramming.Infrastructure.BaseObjects;

namespace MobyLabWebProgramming.Database.Repository.Entities;

public class DeviceCategory : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public ICollection<Device> Devices { get; set; } = null!;
    
    public ICollection<DeviceRequest> DeviceRequests { get; set; } = null!;
}