using MobyLabWebProgramming.Infrastructure.BaseObjects;

namespace MobyLabWebProgramming.Database.Repository.Entities;

public class DeviceAssignment : BaseEntity
{
    public Guid DeviceId { get; set; }
    public Device Device { get; set; } = null!;
    
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    public DateTime AssignedAt { get; set; }
    public DateTime? ReturnedAt { get; set; }
    
    public Ticket? Ticket { get; set; }
}