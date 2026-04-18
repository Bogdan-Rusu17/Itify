namespace Itify.Services.DataTransferObjects;

public class DeviceRequestAddRecord
{
    public string Reason { get; set; } = null!;
    public Guid CategoryId { get; set; }
}