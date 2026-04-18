namespace MobyLabWebProgramming.Services.DataTransferObjects;

/// <summary>
/// This DTO is used to respond to a login with the JWT token and user information.
/// </summary>
public class LoginResponseRecord
{
    public string Token { get; set; } = null!;
    public UserRecord User { get; set; } = null!;
}
