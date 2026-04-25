namespace Itify.DbService.DataTransferObjects;

public class DeviceRequestAddRecord
{
    public string Reason { get; set; } = null!;
    public Guid UserId { get; set; }
    public Guid CategoryId { get; set; }
}
