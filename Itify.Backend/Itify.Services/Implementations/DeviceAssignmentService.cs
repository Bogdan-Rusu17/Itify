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

public class DeviceAssignmentService(IRepository<WebAppDatabaseContext> repository) : IDeviceAssignmentService
{
    public async Task<ServiceResponse<DeviceAssignmentRecord>> GetDeviceAssignment(Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await repository.GetAsync(new DeviceAssignmentProjectionSpec(id), cancellationToken);

        if (result == null)
            return ServiceResponse.FromError<DeviceAssignmentRecord>(CommonErrors.DeviceAssignmentNotFound);

        return ServiceResponse.ForSuccess(result);
    }

    public async Task<ServiceResponse<PagedResponse<DeviceAssignmentRecord>>> GetDeviceAssignments(
        PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default)
    {
        var result = await repository.PageAsync(pagination, new DeviceAssignmentProjectionSpec(pagination.Search),
            cancellationToken);

        return ServiceResponse.ForSuccess(result);
    }

    public async Task<ServiceResponse<PagedResponse<DeviceAssignmentRecord>>> GetMyDeviceAssignments(Guid userId,
        PaginationSearchQueryParams pagination,
        CancellationToken cancellationToken = default)
    {
        var result = await repository.PageAsync(pagination, new DeviceAssignmentProjectionSpec(userId, true),
            cancellationToken);

        return ServiceResponse.ForSuccess(result);
    }

    public async Task<ServiceResponse> AddDeviceAssignment(DeviceAssignmentAddRecord assignment,
        UserRecord requestingUser,
        CancellationToken cancellationToken = default)
    {
        if (!new List<UserRoleEnum> { UserRoleEnum.Admin, UserRoleEnum.ItEngineer }.Contains(requestingUser.Role))
            return ServiceResponse.FromError(CommonErrors.DeviceAssignmentUnauthorized);

        var device = await repository.GetAsync(new DeviceSpec(assignment.DeviceId), cancellationToken);
        if (device == null) return ServiceResponse.FromError(CommonErrors.DeviceNotFound);

        if (device.Status != DeviceStatusEnum.Available)
            return ServiceResponse.FromError(CommonErrors.DeviceNotAvailable);

        var existingAssignment =
            await repository.GetAsync(new DeviceAssignmentSpec(assignment.DeviceId, true), cancellationToken);
        if (existingAssignment != null) return ServiceResponse.FromError(CommonErrors.DeviceAlreadyAssignedToUser);

        device.Status = DeviceStatusEnum.Assigned;
        await repository.UpdateAsync(device, cancellationToken);

        await repository.AddAsync(new DeviceAssignment
        {
            DeviceId = assignment.DeviceId,
            UserId = assignment.UserId,
            AssignedAt = DateTime.UtcNow
        }, cancellationToken);

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> ReturnDeviceAssignment(Guid id, UserRecord requestingUser,
        CancellationToken cancellationToken = default)
    {
        if (!new List<UserRoleEnum> { UserRoleEnum.Admin, UserRoleEnum.ItEngineer }.Contains(requestingUser.Role))
            return ServiceResponse.FromError(CommonErrors.DeviceAssignmentUnauthorized);

        var entity = await repository.GetAsync(new DeviceAssignmentSpec(id), cancellationToken);
        if (entity == null) return ServiceResponse.FromError(CommonErrors.DeviceAssignmentNotFound);

        if (entity.ReturnedAt != null) return ServiceResponse.FromError(CommonErrors.DeviceAlreadyReturned);

        entity.ReturnedAt = DateTime.UtcNow;
        await repository.UpdateAsync(entity, cancellationToken);

        var device = await repository.GetAsync(new DeviceSpec(entity.DeviceId), cancellationToken);
        if (device != null)
        {
            device.Status = DeviceStatusEnum.Available;
            await repository.UpdateAsync(device, cancellationToken);
        }

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> DeleteDeviceAssignment(Guid id, UserRecord requestingUser,
        CancellationToken cancellationToken = default)
    {
        if (!new List<UserRoleEnum> { UserRoleEnum.Admin, UserRoleEnum.ItEngineer }.Contains(requestingUser.Role))
            return ServiceResponse.FromError(CommonErrors.DeviceAssignmentUnauthorized);

        var entity = await repository.GetAsync(new DeviceAssignmentSpec(id), cancellationToken);
        if (entity == null) return ServiceResponse.FromError(CommonErrors.DeviceAssignmentNotFound);

        if (entity.ReturnedAt == null)
        {
            var device = await repository.GetAsync(new DeviceSpec(entity.DeviceId), cancellationToken);
            if (device != null)
            {
                device.Status = DeviceStatusEnum.Available;
                await repository.UpdateAsync(device, cancellationToken);
            }
        }

        await repository.DeleteAsync<DeviceAssignment>(id, cancellationToken);

        return ServiceResponse.ForSuccess();
    }
}