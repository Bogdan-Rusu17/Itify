using Itify.Database.Repository;
using Itify.Database.Repository.Entities;
using Itify.Database.Repository.Enums;
using Itify.Infrastructure.Errors;
using Itify.Infrastructure.Repositories.Interfaces;
using Itify.Infrastructure.Requests;
using Itify.Infrastructure.Responses;
using Itify.Services.Abstractions;
using Itify.Services.DataTransferObjects;
using Itify.Services.Specifications;

namespace Itify.Services.Implementations;

public class UserGroupService(IRepository<WebAppDatabaseContext> repository) : IUserGroupService
{
    public async Task<ServiceResponse<UserGroupRecord>> GetUserGroup(Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await repository.GetAsync(new UserGroupProjectionSpec(id), cancellationToken);

        if (result == null)
        {
            return ServiceResponse.FromError<UserGroupRecord>(CommonErrors.UserGroupNotFound);
        }

        return ServiceResponse.ForSuccess(result);
    }

    public async Task<ServiceResponse<PagedResponse<UserGroupRecord>>> GetUserGroups(
        PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default)
    {
        var result = await repository.PageAsync(pagination, new UserGroupProjectionSpec(pagination.Search), cancellationToken);

        return ServiceResponse.ForSuccess(result);
    }

    public async Task<ServiceResponse> AddUserGroup(UserGroupAddRecord userGroup, UserRecord requestingUser,
        CancellationToken cancellationToken = default)
    {
        if (requestingUser.Role != UserRoleEnum.Admin)
        {
            return ServiceResponse.FromError(CommonErrors.UserGroupUnauthorized);
        }

        var existing = await repository.GetAsync(new UserGroupSpec(userGroup.Name), cancellationToken);

        if (existing != null)
        {
            return ServiceResponse.FromError(CommonErrors.UserGroupAlreadyExists);
        }

        await repository.AddAsync(new UserGroup
        {
            Name = userGroup.Name,
            Description = userGroup.Description
        }, cancellationToken);

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> UpdateUserGroup(UserGroupUpdateRecord userGroup, UserRecord requestingUser,
        CancellationToken cancellationToken = default)
    {
        if (requestingUser.Role != UserRoleEnum.Admin)
        {
            return ServiceResponse.FromError(CommonErrors.UserGroupUnauthorized);
        }

        var entity = await repository.GetAsync(new UserGroupSpec(userGroup.Id), cancellationToken);

        if (entity == null)
        {
            return ServiceResponse.FromError(CommonErrors.UserGroupNotFound);
        }

        entity.Name = userGroup.Name ?? entity.Name;
        entity.Description = userGroup.Description ?? entity.Description;

        await repository.UpdateAsync(entity, cancellationToken);

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> DeleteUserGroup(Guid id, UserRecord requestingUser,
        CancellationToken cancellationToken = default)
    {
        if (requestingUser.Role != UserRoleEnum.Admin)
        {
            return ServiceResponse.FromError(CommonErrors.UserGroupUnauthorized);
        }

        var entity = await repository.GetAsync(new UserGroupSpec(id), cancellationToken);

        if (entity == null)
        {
            return ServiceResponse.FromError(CommonErrors.UserGroupNotFound);
        }

        await repository.DeleteAsync<UserGroup>(id, cancellationToken);

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> AddUserToGroup(Guid groupId, Guid userId, UserRecord requestingUser,
        CancellationToken cancellationToken = default)
    {
        if (requestingUser.Role != UserRoleEnum.Admin)
            return ServiceResponse.FromError(CommonErrors.UserGroupUnauthorized);

        var group = await repository.GetAsync(new UserGroupSpec(groupId, true), cancellationToken);
        if (group == null)
            return ServiceResponse.FromError(CommonErrors.UserGroupNotFound);

        var user = await repository.GetAsync(new UserSpec(userId), cancellationToken);
        if (user == null)
            return ServiceResponse.FromError(CommonErrors.UserNotFound);

        if (group.Users.Any(u => u.Id == userId))
            return ServiceResponse.ForSuccess();

        group.Users.Add(user);
        await repository.UpdateAsync(group, cancellationToken);

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> RemoveUserFromGroup(Guid groupId, Guid userId, UserRecord requestingUser,
        CancellationToken cancellationToken = default)
    {
        if (requestingUser.Role != UserRoleEnum.Admin)
            return ServiceResponse.FromError(CommonErrors.UserGroupUnauthorized);

        var group = await repository.GetAsync(new UserGroupSpec(groupId, true), cancellationToken);
        if (group == null)
            return ServiceResponse.FromError(CommonErrors.UserGroupNotFound);

        var user = group.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
            return ServiceResponse.FromError(CommonErrors.UserNotFound);

        group.Users.Remove(user);
        await repository.UpdateAsync(group, cancellationToken);

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse<PagedResponse<UserRecord>>> GetUsersInGroup(Guid groupId,
        PaginationSearchQueryParams pagination, UserRecord requestingUser, CancellationToken cancellationToken = default)
    {
        var result = await repository.PageAsync(pagination, new UserProjectionSpec(groupId, true), cancellationToken);

        return ServiceResponse.ForSuccess(result);
    }

    public async Task<ServiceResponse<PagedResponse<UserGroupRecord>>> GetGroupsForUser(Guid userId,
        PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default)
    {
        var result = await repository.PageAsync(pagination, new UserGroupProjectionSpec(userId, true), cancellationToken);

        return ServiceResponse.ForSuccess(result);
    }
}
