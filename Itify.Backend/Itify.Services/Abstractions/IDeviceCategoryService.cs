using Itify.Infrastructure.Requests;
using Itify.Infrastructure.Responses;
using Itify.Services.DataTransferObjects;

namespace Itify.Services.Abstractions;

public interface IDeviceCategoryService
{
    public Task<ServiceResponse<DeviceCategoryRecord>> GetDeviceCategory(Guid id,
        CancellationToken cancellationToken = default);

    public Task<ServiceResponse<PagedResponse<DeviceCategoryRecord>>> GetDeviceCategories(
        PaginationSearchQueryParams pagination,
        CancellationToken cancellationToken = default);

    public Task<ServiceResponse> AddDeviceCategory(DeviceCategoryAddRecord category,
        UserRecord requestingUser,
        CancellationToken cancellationToken = default);

    public Task<ServiceResponse> UpdateDeviceCategory(DeviceCategoryUpdateRecord category,
        UserRecord requestingUser,
        CancellationToken cancellationToken = default);

    public Task<ServiceResponse> DeleteDeviceCategory(Guid id,
        UserRecord requestingUser,
        CancellationToken cancellationToken = default);
}