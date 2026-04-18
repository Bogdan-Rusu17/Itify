using Itify.Infrastructure.Requests;
using Itify.Infrastructure.Responses;
using Itify.Services.DataTransferObjects;

namespace Itify.Services.Abstractions;

public interface IDeviceService
{
    public Task<ServiceResponse<DeviceRecord>> GetDevice(Guid id,
                                                         CancellationToken cancellationToken = default);
    public Task<ServiceResponse<PagedResponse<DeviceRecord>>> GetDevices(PaginationSearchQueryParams pagination,
                                                                         CancellationToken cancellationToken = default);
    public Task<ServiceResponse> AddDevice(DeviceAddRecord device,
                                           UserRecord requestingUser,
                                           CancellationToken cancellationToken = default);
    public Task<ServiceResponse> UpdateDevice(DeviceUpdateRecord device,
                                              UserRecord requestingUser,
                                              CancellationToken cancellationToken = default);
    public Task<ServiceResponse> DeleteDevice(Guid id,
                                              UserRecord requestingUser,
                                              CancellationToken cancellationToken = default);

}