using Itify.Infrastructure.Requests;
using Itify.Infrastructure.Responses;
using Itify.Services.DataTransferObjects;

namespace Itify.Services.Abstractions;

public interface IUserGroupService
{
    public Task<ServiceResponse<UserGroupRecord>> GetUserGroup(Guid id,
        CancellationToken cancellationToken = default);

    public Task<ServiceResponse<PagedResponse<UserGroupRecord>>> GetUserGroups(
        PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default);

    public Task<ServiceResponse> AddUserGroup(UserGroupAddRecord userGroup, UserRecord requestingUser,
        CancellationToken cancellationToken = default);

    public Task<ServiceResponse> UpdateUserGroup(UserGroupUpdateRecord userGroup, UserRecord requestingUser,
        CancellationToken cancellationToken = default);

    public Task<ServiceResponse> DeleteUserGroup(Guid id, UserRecord requestingUser,
        CancellationToken cancellationToken = default);

    public Task<ServiceResponse> AddUserToGroup(Guid groupId, Guid userId, UserRecord requestingUser,
        CancellationToken cancellationToken = default);

    public Task<ServiceResponse> RemoveUserFromGroup(Guid groupId, Guid userId, UserRecord requestingUser,
        CancellationToken cancellationToken = default);

    public Task<ServiceResponse<PagedResponse<UserRecord>>> GetUsersInGroup(Guid groupId,
        PaginationSearchQueryParams pagination, UserRecord requestingUser, CancellationToken cancellationToken = default);

    public Task<ServiceResponse<PagedResponse<UserGroupRecord>>> GetGroupsForUser(Guid userId,
        PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default);
}
