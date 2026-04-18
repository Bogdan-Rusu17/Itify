using Itify.Database.Repository.Enums;
using Itify.Infrastructure.BaseObjects;

namespace Itify.Database.Repository.Entities;

public class DeviceRequest : BaseEntity
{
    public string Reason { get; set; } = null!;
    public RequestStatusEnum Status { get; set; }
    
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    public Guid CategoryId { get; set; }
    public DeviceCategory Category { get; set; } = null!;
}