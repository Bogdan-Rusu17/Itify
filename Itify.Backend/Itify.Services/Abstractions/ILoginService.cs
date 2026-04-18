using Itify.Services.DataTransferObjects;

namespace Itify.Services.Abstractions;

/// <summary>
/// This service is used to emit a JWT token.
/// </summary>
public interface ILoginService
{
    /// <summary>
    /// GetToken returns a JWT token string for a user with an issue date and and expiration interval after issue.
    /// </summary>
    public string GetToken(UserRecord user, DateTime issuedAt, TimeSpan expiresIn);
}
