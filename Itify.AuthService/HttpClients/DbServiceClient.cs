using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Itify.AuthService.DataTransferObjects;
using Itify.AuthService.Infrastructure;

namespace Itify.AuthService.HttpClients;

public class DbServiceClient(HttpClient http) : IDbServiceClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNameCaseInsensitive = true
    };

    public async Task<DbUserRecord?> GetUserWithPasswordAsync(string email)
    {
        var response = await http.GetAsync($"db/users/with-password/{Uri.EscapeDataString(email)}");
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DbUserRecord>(JsonOptions);
    }

    public async Task<bool> UserExistsAsync(string email)
    {
        var response = await http.GetAsync($"db/users/by-email/{Uri.EscapeDataString(email)}");
        if (response.StatusCode == HttpStatusCode.NotFound) return false;
        response.EnsureSuccessStatusCode();
        return true;
    }

    public async Task CreateUserAsync(CreateUserRequest dto)
    {
        var response = await http.PostAsJsonAsync("db/users", dto, JsonOptions);
        if (!response.IsSuccessStatusCode)
            throw new ServerException(response.StatusCode, "Failed to create user in DbService");
    }
}
