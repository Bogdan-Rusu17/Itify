namespace Itify.DbService.DataTransferObjects;

public class DeviceAssignmentRecord
{
    public Guid Id { get; set; }
    public Guid DeviceId { get; set; }
    public string DeviceName { get; set; } = null!;
    public string SerialNumber { get; set; } = null!;
    public Guid UserId { get; set; }
    public string UserName { get; set; } = null!;
    public DateTime AssignedAt { get; set; }
    public DateTime? ReturnedAt { get; set; }
}
