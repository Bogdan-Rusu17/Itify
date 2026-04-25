using Itify.AuthService.DataTransferObjects;

namespace Itify.AuthService.HttpClients;

public interface IDbServiceClient
{
    Task<DbUserRecord?> GetUserWithPasswordAsync(string email);
    Task<bool> UserExistsAsync(string email);
    Task CreateUserAsync(CreateUserRequest dto);
}
