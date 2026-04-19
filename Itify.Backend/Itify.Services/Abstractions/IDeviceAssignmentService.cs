using Itify.Infrastructure.Requests;
using Itify.Infrastructure.Responses;
using Itify.Services.DataTransferObjects;

namespace Itify.Services.Abstractions;

public interface IDeviceAssignmentService
{
    public Task<ServiceResponse<DeviceAssignmentRecord>> GetDeviceAssignment(Guid id,
        CancellationToken cancellationToken = default);

    public Task<ServiceResponse<PagedResponse<DeviceAssignmentRecord>>> GetDeviceAssignments(
        PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default);

    public Task<ServiceResponse<PagedResponse<DeviceAssignmentRecord>>> GetMyDeviceAssignments(Guid userId,
        PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default);

    public Task<ServiceResponse> AddDeviceAssignment(DeviceAssignmentAddRecord assignment, UserRecord requestingUser,
        CancellationToken cancellationToken = default);

    public Task<ServiceResponse> ReturnDeviceAssignment(Guid id, UserRecord requestingUser,
        CancellationToken cancellationToken = default);

    public Task<ServiceResponse> DeleteDeviceAssignment(Guid id, UserRecord requestingUser,
        CancellationToken cancellationToken = default);
}