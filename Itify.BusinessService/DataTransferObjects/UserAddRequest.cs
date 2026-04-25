using Itify.BusinessService.Enums;

namespace Itify.BusinessService.DataTransferObjects;

public class UserAddRequest
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public UserRoleEnum Role { get; set; }
}
