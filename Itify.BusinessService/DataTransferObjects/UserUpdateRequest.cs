using Itify.BusinessService.Enums;

namespace Itify.BusinessService.DataTransferObjects;

public class UserUpdateRequest
{
    public string? Name { get; set; }
    public string? Password { get; set; }
    public UserRoleEnum? Role { get; set; }
}
