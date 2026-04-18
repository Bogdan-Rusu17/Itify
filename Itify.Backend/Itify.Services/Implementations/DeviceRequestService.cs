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

public class DeviceRequestService(IRepository<WebAppDatabaseContext> repository) : IDeviceRequestService
{
    public async Task<ServiceResponse<DeviceRequestRecord>> GetDeviceRequest(Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await repository.GetAsync(new DeviceRequestProjectionSpec(id), cancellationToken);

        if (result == null) return ServiceResponse.FromError<DeviceRequestRecord>(CommonErrors.DeviceRequestNotFound);

        return ServiceResponse.ForSuccess(result);
    }

    public async Task<ServiceResponse<PagedResponse<DeviceRequestRecord>>> GetDeviceRequests(
        PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default)
    {
        var result = await repository.PageAsync(pagination, new DeviceRequestProjectionSpec(pagination.Search),
            cancellationToken);

        return ServiceResponse.ForSuccess(result);
    }

    public async Task<ServiceResponse> AddDeviceRequest(DeviceRequestAddRecord request, UserRecord requestingUser,
        CancellationToken cancellationToken = default)
    {
        var category = await repository.GetAsync(new DeviceCategorySpec(request.CategoryId), cancellationToken);
        if (category == null) return ServiceResponse.FromError(CommonErrors.DeviceCategoryNotFound);
        
        var existing = await repository.GetAsync(new DeviceRequestSpec(requestingUser.Id, request.CategoryId), cancellationToken);
        if (existing?.Status == RequestStatusEnum.Pending)
            return ServiceResponse.FromError(CommonErrors.UnauthorizedDeviceRequestAlreadyOnCategory);
        
        await repository.AddAsync(new DeviceRequest
        {
            Reason = request.Reason,
            CategoryId = request.CategoryId,
            UserId = requestingUser.Id,
            Status = RequestStatusEnum.Pending
        }, cancellationToken);

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> UpdateDeviceRequest(DeviceRequestUpdateRecord request, UserRecord requestingUser,
        CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetAsync(new DeviceRequestSpec(request.Id), cancellationToken);
        if (entity == null) return ServiceResponse.FromError(CommonErrors.DeviceRequestNotFound);

        if (requestingUser.Role == UserRoleEnum.Employee && entity.UserId != requestingUser.Id)
            return ServiceResponse.FromError(CommonErrors.DeviceRequestCannotUpdateNotOwnedRequestIfEmployee);

        if (requestingUser.Role == UserRoleEnum.Employee && request.Status !=
            null && request.Status != RequestStatusEnum.Pending)
            return ServiceResponse.FromError(CommonErrors.UnauthorizedDeviceRequestStatusUpdate);
        
        if (entity.Status != RequestStatusEnum.Pending)
            return ServiceResponse.FromError(CommonErrors.UnauthorizedDeviceRequestUpdateIfResolutionExists);

        
        entity.Reason = request.Reason ?? entity.Reason;
        entity.Status = request.Status ?? entity.Status;

        await repository.UpdateAsync(entity, cancellationToken);

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> DeleteDeviceRequest(Guid id, UserRecord requestingUser,
        CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetAsync(new DeviceRequestSpec(id),
            cancellationToken);
        if (entity == null)
            return
                ServiceResponse.FromError(CommonErrors.DeviceRequestNotFound);

        if (requestingUser.Role == UserRoleEnum.Employee && entity.UserId !=
            requestingUser.Id)
            return ServiceResponse.FromError(CommonErrors.UnauthorizedDeviceRequestDelete);

        if (requestingUser.Role == UserRoleEnum.Employee && entity.Status !=
            RequestStatusEnum.Pending)
            return ServiceResponse.FromError(CommonErrors.UnauthorizedDeviceRequestDeleteIfResolutionExists);

        await repository.DeleteAsync<DeviceRequest>(id, cancellationToken);

        return ServiceResponse.ForSuccess();
    }
}