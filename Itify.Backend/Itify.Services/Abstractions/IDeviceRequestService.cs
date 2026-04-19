using Itify.Infrastructure.Requests;
using Itify.Infrastructure.Responses;
using Itify.Services.DataTransferObjects;

namespace Itify.Services.Abstractions;

public interface IDeviceRequestService
{
    public Task<ServiceResponse<DeviceRequestRecord>> GetDeviceRequest(Guid id, UserRecord requestingUser,
        CancellationToken cancellationToken = default);

    public Task<ServiceResponse<PagedResponse<DeviceRequestRecord>>> GetDeviceRequests(
        PaginationSearchQueryParams pagination, UserRecord requestingUser, CancellationToken cancellationToken = default);

    public Task<ServiceResponse> AddDeviceRequest(DeviceRequestAddRecord request, UserRecord requestingUser,
        CancellationToken cancellationToken = default);

    public Task<ServiceResponse> UpdateDeviceRequest(DeviceRequestUpdateRecord request, UserRecord requestingUser,
        CancellationToken cancellationToken = default);

    public Task<ServiceResponse> DeleteDeviceRequest(Guid id, UserRecord requestingUser,
        CancellationToken cancellationToken = default);
}