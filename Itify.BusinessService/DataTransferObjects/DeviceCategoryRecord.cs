namespace Itify.BusinessService.DataTransferObjects;

public class DeviceCategoryRecord
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
