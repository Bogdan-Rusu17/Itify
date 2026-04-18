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

public class DeviceCategoryService(IRepository<WebAppDatabaseContext> repository) : IDeviceCategoryService
{
    public async Task<ServiceResponse<DeviceCategoryRecord>> GetDeviceCategory(Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await repository.GetAsync(new DeviceCategoryProjectionSpec(id), cancellationToken);

        if (result == null) return ServiceResponse.FromError<DeviceCategoryRecord>(CommonErrors.DeviceCategoryNotFound);

        return ServiceResponse.ForSuccess(result);
    }

    public async Task<ServiceResponse<PagedResponse<DeviceCategoryRecord>>> GetDeviceCategories(
        PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default)
    {
        var result = await repository.PageAsync(pagination, new DeviceCategoryProjectionSpec(pagination.Search),
            cancellationToken);

        return ServiceResponse.ForSuccess(result);
    }

    public async Task<ServiceResponse> AddDeviceCategory(DeviceCategoryAddRecord category, UserRecord requestingUser,
        CancellationToken cancellationToken = default)
    {
        if (!new List<UserRoleEnum> { UserRoleEnum.Admin, UserRoleEnum.ItEngineer }.Contains(requestingUser.Role))
            return ServiceResponse.FromError(CommonErrors.DeviceCategoryUnauthorized);

        var entity = await repository.GetAsync(new DeviceCategorySpec(category.Name), cancellationToken);
        if (entity != null) return ServiceResponse.FromError(CommonErrors.DeviceCategoryAlreadyExists);

        await repository.AddAsync(new DeviceCategory
        {
            Name = category.Name,
            Description = category.Description
        }, cancellationToken);

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> UpdateDeviceCategory(DeviceCategoryUpdateRecord category,
        UserRecord requestingUser,
        CancellationToken cancellationToken = default)
    {
        if (!new List<UserRoleEnum> { UserRoleEnum.Admin, UserRoleEnum.ItEngineer }.Contains(requestingUser.Role))
            return ServiceResponse.FromError(CommonErrors.DeviceCategoryUnauthorized);

        var entity = await repository.GetAsync(new DeviceCategorySpec(category.Id), cancellationToken);
        if (entity == null) return ServiceResponse.FromError(CommonErrors.DeviceCategoryNotFound);

        entity.Description = category.Description ?? entity.Description;
        entity.Name = category.Name ?? entity.Name;

        await repository.UpdateAsync(entity, cancellationToken);

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> DeleteDeviceCategory(Guid id, UserRecord requestingUser,
        CancellationToken cancellationToken = default)
    {
        if (!new List<UserRoleEnum> { UserRoleEnum.Admin, UserRoleEnum.ItEngineer }.Contains(requestingUser.Role))
            return ServiceResponse.FromError(CommonErrors.DeviceCategoryUnauthorized);

        var deviceCount = await repository.GetCountAsync(new DeviceSpec(id, true), cancellationToken);
        if (deviceCount > 0)
            return ServiceResponse.FromError(CommonErrors.DeviceCategoryHasDevices);

        await repository.DeleteAsync<DeviceCategory>(id, cancellationToken);

        return ServiceResponse.ForSuccess();
    }
}