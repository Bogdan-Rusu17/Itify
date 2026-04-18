using Microsoft.AspNetCore.Http;

namespace Itify.Services.DataTransferObjects;

/// <summary>
/// This DTO is used to add a user file, it contains a IFormFile that has the stream to the file in a form and an additional property.
/// </summary>
public class UserFileAddRecord
{
    public IFormFile File { get; set; } = null!;
    public string? Description { get; set; }
}
