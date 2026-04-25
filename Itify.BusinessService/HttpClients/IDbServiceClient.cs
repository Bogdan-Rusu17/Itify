using Itify.BusinessService.DataTransferObjects;
using Itify.BusinessService.Enums;
using Itify.BusinessService.Infrastructure;

namespace Itify.BusinessService.HttpClients;

public interface IDbServiceClient
{
    Task<DeviceRecord?> GetDeviceAsync(Guid id);
    Task<PagedResponse<DeviceRecord>> GetDevicesAsync(int page, int pageSize, string? search = null, Guid? assignedToUserId = null);
    Task<Guid> CreateDeviceAsync(DeviceAddRequest dto);
    Task UpdateDeviceAsync(Guid id, DeviceUpdateRequest dto);
    Task DeleteDeviceAsync(Guid id);
    Task<(Guid Id, string Name, string SerialNumber)?> GetFirstAvailableDeviceAsync(Guid categoryId);
    Task<DeviceCategoryRecord?> GetDeviceCategoryAsync(Guid id);
    Task<PagedResponse<DeviceCategoryRecord>> GetDeviceCategoriesAsync(int page, int pageSize, string? search = null);
    Task<Guid> CreateDeviceCategoryAsync(DeviceCategoryAddRequest dto);
    Task UpdateDeviceCategoryAsync(Guid id, DeviceCategoryUpdateRequest dto);
    Task DeleteDeviceCategoryAsync(Guid id);
    Task<DeviceAssignmentRecord?> GetDeviceAssignmentAsync(Guid id);
    Task<PagedResponse<DeviceAssignmentRecord>> GetDeviceAssignmentsAsync(int page, int pageSize, string? search = null, Guid? userId = null);
    Task<Guid> CreateDeviceAssignmentAsync(DeviceAssignmentAddRequest dto);
    Task ReturnDeviceAssignmentAsync(Guid id);
    Task DeleteDeviceAssignmentAsync(Guid id);
    Task<DeviceRequestRecord?> GetDeviceRequestAsync(Guid id);
    Task<PagedResponse<DeviceRequestRecord>> GetDeviceRequestsAsync(int page, int pageSize, string? search = null, Guid? userId = null);
    Task<bool> HasPendingDeviceRequestAsync(Guid userId, Guid categoryId);
    Task<Guid> CreateDeviceRequestAsync(Guid userId, DeviceRequestAddRequest dto);
    Task UpdateDeviceRequestAsync(Guid id, DeviceRequestUpdateRequest dto);
    Task DeleteDeviceRequestAsync(Guid id);
    Task<TicketRecord?> GetTicketAsync(Guid id);
    Task<TicketRecord?> GetTicketByAssignmentAsync(Guid assignmentId);
    Task<PagedResponse<TicketRecord>> GetTicketsAsync(int page, int pageSize, string? search = null, Guid? userId = null);
    Task CreateTicketAsync(Guid userId, TicketAddRequest dto);
    Task UpdateTicketAsync(Guid id, TicketUpdateRequest dto);
    Task DeleteTicketAsync(Guid id);
    Task<UserRecord?> GetUserAsync(Guid id);
    Task<PagedResponse<UserRecord>> GetUsersAsync(int page, int pageSize, string? search = null, UserRoleEnum? role = null);
    Task<List<UserRecord>> GetStaffAsync();
    Task CreateUserAsync(UserAddRequest dto);
    Task UpdateUserAsync(Guid id, UserUpdateRequest dto);
    Task DeleteUserAsync(Guid id);

    // User Groups
    Task<UserGroupRecord?> GetUserGroupAsync(Guid id);
    Task<PagedResponse<UserGroupRecord>> GetUserGroupsAsync(int page, int pageSize, string? search = null);
    Task<PagedResponse<UserGroupRecord>> GetGroupsForUserAsync(Guid userId, int page, int pageSize);
    Task<PagedResponse<UserRecord>> GetUsersInGroupAsync(Guid groupId, int page, int pageSize);
    Task<Guid> CreateUserGroupAsync(UserGroupAddRequest dto);
    Task UpdateUserGroupAsync(Guid id, UserGroupUpdateRequest dto);
    Task DeleteUserGroupAsync(Guid id);
    Task AddUserToGroupAsync(Guid groupId, Guid userId);
    Task RemoveUserFromGroupAsync(Guid groupId, Guid userId);
}
