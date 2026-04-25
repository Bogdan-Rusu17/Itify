using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Itify.BusinessService.DataTransferObjects;
using Itify.BusinessService.Enums;
using Itify.BusinessService.Infrastructure;

namespace Itify.BusinessService.HttpClients;

public class DbServiceClient(HttpClient http) : IDbServiceClient
{
    private static readonly JsonSerializerOptions Json = new()
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNameCaseInsensitive = true
    };

    private async Task<T?> GetOrNull<T>(string url) where T : class
    {
        var r = await http.GetAsync(url);
        if (r.StatusCode == HttpStatusCode.NotFound) return null;
        r.EnsureSuccessStatusCode();
        return await r.Content.ReadFromJsonAsync<T>(Json);
    }

    private async Task<PagedResponse<T>> GetPaged<T>(string url)
    {
        var r = await http.GetAsync(url);
        r.EnsureSuccessStatusCode();
        return (await r.Content.ReadFromJsonAsync<PagedResponse<T>>(Json))!;
    }

    private async Task<Guid> PostForId(string url, object dto)
    {
        var r = await http.PostAsJsonAsync(url, dto, Json);
        if (!r.IsSuccessStatusCode) throw new ServerException(r.StatusCode, $"DbService error: {r.StatusCode}");
        var result = await r.Content.ReadFromJsonAsync<JsonElement>(Json);
        return result.GetProperty("id").GetGuid();
    }

    private async Task Put(string url, object dto)
    {
        var r = await http.PutAsJsonAsync(url, dto, Json);
        if (!r.IsSuccessStatusCode) throw new ServerException(r.StatusCode, $"DbService error: {r.StatusCode}");
    }

    private async Task Delete(string url)
    {
        var r = await http.DeleteAsync(url);
        if (r.StatusCode == HttpStatusCode.NotFound) throw new NotFoundException("Resource not found.");
        r.EnsureSuccessStatusCode();
    }

    public Task<DeviceRecord?> GetDeviceAsync(Guid id) => GetOrNull<DeviceRecord>($"db/devices/{id}");

    public Task<PagedResponse<DeviceRecord>> GetDevicesAsync(int page, int pageSize, string? search = null, Guid? assignedToUserId = null)
    {
        var url = $"db/devices?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(search)) url += $"&search={Uri.EscapeDataString(search)}";
        if (assignedToUserId.HasValue) url += $"&assignedToUserId={assignedToUserId}";
        return GetPaged<DeviceRecord>(url);
    }

    public Task<Guid> CreateDeviceAsync(DeviceAddRequest dto) => PostForId("db/devices", dto);
    public Task UpdateDeviceAsync(Guid id, DeviceUpdateRequest dto) => Put($"db/devices/{id}", dto);
    public Task DeleteDeviceAsync(Guid id) => Delete($"db/devices/{id}");

    public async Task<(Guid Id, string Name, string SerialNumber)?> GetFirstAvailableDeviceAsync(Guid categoryId)
    {
        var r = await http.GetAsync($"db/devices/first-available/{categoryId}");
        if (r.StatusCode == HttpStatusCode.NotFound) return null;
        r.EnsureSuccessStatusCode();
        var el = await r.Content.ReadFromJsonAsync<JsonElement>(Json);
        return (el.GetProperty("id").GetGuid(), el.GetProperty("name").GetString()!, el.GetProperty("serialNumber").GetString()!);
    }

    public Task<DeviceCategoryRecord?> GetDeviceCategoryAsync(Guid id) => GetOrNull<DeviceCategoryRecord>($"db/device-categories/{id}");

    public Task<PagedResponse<DeviceCategoryRecord>> GetDeviceCategoriesAsync(int page, int pageSize, string? search = null)
    {
        var url = $"db/device-categories?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(search)) url += $"&search={Uri.EscapeDataString(search)}";
        return GetPaged<DeviceCategoryRecord>(url);
    }

    public Task<Guid> CreateDeviceCategoryAsync(DeviceCategoryAddRequest dto) => PostForId("db/device-categories", dto);
    public Task UpdateDeviceCategoryAsync(Guid id, DeviceCategoryUpdateRequest dto) => Put($"db/device-categories/{id}", dto);
    public Task DeleteDeviceCategoryAsync(Guid id) => Delete($"db/device-categories/{id}");

    public Task<DeviceAssignmentRecord?> GetDeviceAssignmentAsync(Guid id) => GetOrNull<DeviceAssignmentRecord>($"db/device-assignments/{id}");

    public Task<PagedResponse<DeviceAssignmentRecord>> GetDeviceAssignmentsAsync(int page, int pageSize, string? search = null, Guid? userId = null)
    {
        var url = $"db/device-assignments?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(search)) url += $"&search={Uri.EscapeDataString(search)}";
        if (userId.HasValue) url += $"&userId={userId}";
        return GetPaged<DeviceAssignmentRecord>(url);
    }

    public Task<Guid> CreateDeviceAssignmentAsync(DeviceAssignmentAddRequest dto) => PostForId("db/device-assignments", dto);

    public async Task ReturnDeviceAssignmentAsync(Guid id)
    {
        var r = await http.PutAsync($"db/device-assignments/{id}/return", null);
        if (!r.IsSuccessStatusCode) throw new ServerException(r.StatusCode, $"DbService error: {r.StatusCode}");
    }

    public Task DeleteDeviceAssignmentAsync(Guid id) => Delete($"db/device-assignments/{id}");

    public Task<DeviceRequestRecord?> GetDeviceRequestAsync(Guid id) => GetOrNull<DeviceRequestRecord>($"db/device-requests/{id}");

    public Task<PagedResponse<DeviceRequestRecord>> GetDeviceRequestsAsync(int page, int pageSize, string? search = null, Guid? userId = null)
    {
        var url = $"db/device-requests?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(search)) url += $"&search={Uri.EscapeDataString(search)}";
        if (userId.HasValue) url += $"&userId={userId}";
        return GetPaged<DeviceRequestRecord>(url);
    }

    public async Task<bool> HasPendingDeviceRequestAsync(Guid userId, Guid categoryId)
    {
        var r = await http.GetAsync($"db/device-requests/pending?userId={userId}&categoryId={categoryId}");
        r.EnsureSuccessStatusCode();
        var el = await r.Content.ReadFromJsonAsync<JsonElement>(Json);
        return el.GetProperty("hasPending").GetBoolean();
    }

    public async Task<Guid> CreateDeviceRequestAsync(Guid userId, DeviceRequestAddRequest dto) =>
        await PostForId("db/device-requests", new { dto.Reason, UserId = userId, dto.CategoryId });

    public Task UpdateDeviceRequestAsync(Guid id, DeviceRequestUpdateRequest dto) => Put($"db/device-requests/{id}", dto);
    public Task DeleteDeviceRequestAsync(Guid id) => Delete($"db/device-requests/{id}");

    public Task<TicketRecord?> GetTicketAsync(Guid id) => GetOrNull<TicketRecord>($"db/tickets/{id}");
    public Task<TicketRecord?> GetTicketByAssignmentAsync(Guid assignmentId) => GetOrNull<TicketRecord>($"db/tickets/by-assignment/{assignmentId}");

    public Task<PagedResponse<TicketRecord>> GetTicketsAsync(int page, int pageSize, string? search = null, Guid? userId = null)
    {
        var url = $"db/tickets?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(search)) url += $"&search={Uri.EscapeDataString(search)}";
        if (userId.HasValue) url += $"&userId={userId}";
        return GetPaged<TicketRecord>(url);
    }

    public async Task CreateTicketAsync(Guid userId, TicketAddRequest dto) =>
        await PostForId("db/tickets", new
        {
            dto.Description, dto.Type, dto.Priority, dto.IsUrgent,
            dto.AdditionalNotes, dto.DeviceAssignmentId, UserId = userId
        });

    public Task UpdateTicketAsync(Guid id, TicketUpdateRequest dto) => Put($"db/tickets/{id}", dto);
    public Task DeleteTicketAsync(Guid id) => Delete($"db/tickets/{id}");
    public Task<UserRecord?> GetUserAsync(Guid id) => GetOrNull<UserRecord>($"db/users/{id}");

    public Task<PagedResponse<UserRecord>> GetUsersAsync(int page, int pageSize, string? search = null, UserRoleEnum? role = null)
    {
        var url = $"db/users?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(search)) url += $"&search={Uri.EscapeDataString(search)}";
        if (role.HasValue) url += $"&role={role}";
        return GetPaged<UserRecord>(url);
    }

    public async Task<List<UserRecord>> GetStaffAsync()
    {
        var admins = await GetUsersAsync(1, 200, role: UserRoleEnum.Admin);
        var engineers = await GetUsersAsync(1, 200, role: UserRoleEnum.ItEngineer);
        return [.. admins.Data, .. engineers.Data];
    }

    public async Task CreateUserAsync(UserAddRequest dto) =>
        await PostForId("db/users", new { dto.Name, dto.Email, dto.Password, dto.Role });

    public Task UpdateUserAsync(Guid id, UserUpdateRequest dto) => Put($"db/users/{id}", dto);
    public Task DeleteUserAsync(Guid id) => Delete($"db/users/{id}");

    // User Groups
    public Task<UserGroupRecord?> GetUserGroupAsync(Guid id) => GetOrNull<UserGroupRecord>($"db/user-groups/{id}");

    public Task<PagedResponse<UserGroupRecord>> GetUserGroupsAsync(int page, int pageSize, string? search = null)
    {
        var url = $"db/user-groups?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(search)) url += $"&search={Uri.EscapeDataString(search)}";
        return GetPaged<UserGroupRecord>(url);
    }

    public Task<PagedResponse<UserGroupRecord>> GetGroupsForUserAsync(Guid userId, int page, int pageSize) =>
        GetPaged<UserGroupRecord>($"db/user-groups/by-user/{userId}?page={page}&pageSize={pageSize}");

    public Task<PagedResponse<UserRecord>> GetUsersInGroupAsync(Guid groupId, int page, int pageSize) =>
        GetPaged<UserRecord>($"db/user-groups/{groupId}/members?page={page}&pageSize={pageSize}");

    public Task<Guid> CreateUserGroupAsync(UserGroupAddRequest dto) => PostForId("db/user-groups", dto);
    public Task UpdateUserGroupAsync(Guid id, UserGroupUpdateRequest dto) => Put($"db/user-groups/{id}", dto);
    public Task DeleteUserGroupAsync(Guid id) => Delete($"db/user-groups/{id}");

    public async Task AddUserToGroupAsync(Guid groupId, Guid userId)
    {
        var r = await http.PostAsync($"db/user-groups/{groupId}/members/{userId}", null);
        if (!r.IsSuccessStatusCode) throw new ServerException(r.StatusCode, $"DbService error: {r.StatusCode}");
    }

    public async Task RemoveUserFromGroupAsync(Guid groupId, Guid userId)
    {
        var r = await http.DeleteAsync($"db/user-groups/{groupId}/members/{userId}");
        if (!r.IsSuccessStatusCode) throw new ServerException(r.StatusCode, $"DbService error: {r.StatusCode}");
    }
}
