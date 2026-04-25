namespace Itify.BusinessService.DataTransferObjects;

public class DeviceRequestAddRequest
{
    public string Reason { get; set; } = null!;
    public Guid CategoryId { get; set; }
}
