using System.Security.Claims;
using Itify.BusinessService.Enums;

namespace Itify.BusinessService.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user) =>
        Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public static UserRoleEnum GetRole(this ClaimsPrincipal user) =>
        Enum.Parse<UserRoleEnum>(user.FindFirstValue(ClaimTypes.Role)!);

    public static string GetName(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.Name)!;
}
