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

public class DeviceService(IRepository<WebAppDatabaseContext> repository) : IDeviceService
{
    public async Task<ServiceResponse<DeviceRecord>> GetDevice(Guid id, UserRecord requestingUser,
        CancellationToken cancellationToken = default)
    {
        DeviceProjectionSpec spec;

        if (requestingUser.Role == UserRoleEnum.Employee)
        {
            spec = new DeviceProjectionSpec(id, requestingUser.Id);
        }
        else
        {
            spec = new DeviceProjectionSpec(id);
        }

        var result = await repository.GetAsync(spec, cancellationToken);

        if (result == null)
        {
            return ServiceResponse.FromError<DeviceRecord>(CommonErrors.DeviceNotFound);
        }

        return ServiceResponse.ForSuccess(result);
    }

    public async Task<ServiceResponse<PagedResponse<DeviceRecord>>> GetDevices(PaginationSearchQueryParams pagination,
        UserRecord requestingUser, CancellationToken cancellationToken = default)
    {
        DeviceProjectionSpec spec;

        if (requestingUser.Role == UserRoleEnum.Employee)
        {
            spec = new DeviceProjectionSpec(pagination.Search, requestingUser.Id);
        }
        else
        {
            spec = new DeviceProjectionSpec(pagination.Search);
        }

        var result = await repository.PageAsync(pagination, spec, cancellationToken);

        return ServiceResponse.ForSuccess(result);
    }

    public async Task<ServiceResponse> AddDevice(DeviceAddRecord device, UserRecord requestingUser,
        CancellationToken cancellationToken = default)
    {
        if (!new List<UserRoleEnum> { UserRoleEnum.Admin, UserRoleEnum.ItEngineer }.Contains(requestingUser.Role))
            return ServiceResponse.FromError(CommonErrors.UnauthorizedDeviceAddOrUpdate);

        var category = await repository.GetAsync(new DeviceCategorySpec(device.CategoryId), cancellationToken);
        if (category == null) return ServiceResponse.FromError(CommonErrors.DeviceCategoryNotFound);

        var entity = await repository.GetAsync(new DeviceSpec(device.SerialNumber), cancellationToken);
        if (entity != null) return ServiceResponse.FromError(CommonErrors.DeviceAlreadyExists);

        await repository.AddAsync(new Device
        {
            Name = device.Name,
            CategoryId = device.CategoryId,
            SerialNumber = device.SerialNumber,
            Status = device.Status,
            PurchaseDate = device.PurchaseDate
        }, cancellationToken);

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> UpdateDevice(DeviceUpdateRecord device, UserRecord requestingUser,
        CancellationToken cancellationToken = default)
    {
        if (!new List<UserRoleEnum> { UserRoleEnum.Admin, UserRoleEnum.ItEngineer }.Contains(requestingUser.Role))
            return ServiceResponse.FromError(CommonErrors.UnauthorizedDeviceAddOrUpdate);

        var entity = await repository.GetAsync(new DeviceSpec(device.Id), cancellationToken);
        if (entity == null) return ServiceResponse.FromError(CommonErrors.DeviceNotFound);

        entity.Name = device.Name ?? entity.Name;
        entity.Status = device.Status ?? entity.Status;
        entity.PurchaseDate = device.PurchaseDate ?? entity.PurchaseDate;

        await repository.UpdateAsync(entity, cancellationToken);

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> DeleteDevice(Guid id, UserRecord requestingUser,
        CancellationToken cancellationToken = default)
    {
        if (!new List<UserRoleEnum> { UserRoleEnum.Admin, UserRoleEnum.ItEngineer }.Contains(requestingUser.Role))
            return ServiceResponse.FromError(CommonErrors.UnauthorizedDeviceAddOrUpdate);

        await repository.DeleteAsync<Device>(id, cancellationToken);

        return ServiceResponse.ForSuccess();
    }
}