namespace Itify.Services.DataTransferObjects;

public class UserGroupRecord
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
