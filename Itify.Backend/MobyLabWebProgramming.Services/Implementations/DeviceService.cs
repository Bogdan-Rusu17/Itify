using System.Net;
using MobyLabWebProgramming.Database.Repository;
using MobyLabWebProgramming.Database.Repository.Entities;
using MobyLabWebProgramming.Database.Repository.Enums;
using MobyLabWebProgramming.Infrastructure.Errors;
using MobyLabWebProgramming.Infrastructure.Repositories.Interfaces;
using MobyLabWebProgramming.Infrastructure.Requests;
using MobyLabWebProgramming.Infrastructure.Responses;
using MobyLabWebProgramming.Services.Abstractions;
using MobyLabWebProgramming.Services.DataTransferObjects;
using MobyLabWebProgramming.Services.Specifications;

namespace MobyLabWebProgramming.Services.Implementations;

public class DeviceService(IRepository<WebAppDatabaseContext> repository) : IDeviceService
{
        public async Task<ServiceResponse<DeviceRecord>> GetDevice(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await repository.GetAsync(new DeviceProjectionSpec(id), cancellationToken);

            if (result == null)
            {
                return ServiceResponse.FromError<DeviceRecord>(CommonErrors.DeviceNotFound);
            }
            
            return ServiceResponse.ForSuccess(result);
        }
    
        public async Task<ServiceResponse<PagedResponse<DeviceRecord>>> GetDevices(PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default)
        {
            var result = await repository.PageAsync(pagination, new DeviceProjectionSpec(pagination.Search), cancellationToken);

            return ServiceResponse.ForSuccess(result);
        }
    
        public async Task<ServiceResponse> AddDevice(DeviceAddRecord device, UserRecord requestingUser, CancellationToken cancellationToken = default)
        {
            if (!new List<UserRoleEnum> { UserRoleEnum.Admin, UserRoleEnum.ItEngineer }.Contains(requestingUser.Role))
            {
                return ServiceResponse.FromError(CommonErrors.UnauthorizedDeviceAddOrUpdate);
            }

            await repository.AddAsync(new Device
            {
                Name = device.Name,
                CategoryId = device.CategoryId,
                SerialNumber = device.SerialNumber,
                Status = device.Status,
                PurchaseDate = device.PurchaseDate,
            }, cancellationToken);
            
            return ServiceResponse.ForSuccess();
        }
    
        public async Task<ServiceResponse> UpdateDevice(DeviceUpdateRecord device, UserRecord requestingUser, CancellationToken cancellationToken = default)
        {
            if (!new List<UserRoleEnum> { UserRoleEnum.Admin, UserRoleEnum.ItEngineer }.Contains(requestingUser.Role))
            {
                return ServiceResponse.FromError(CommonErrors.UnauthorizedDeviceAddOrUpdate);
            }
            
            var entity = await repository.GetAsync(new DeviceSpec(device.Id), cancellationToken);
            if (entity == null)
            {
                return ServiceResponse.FromError(CommonErrors.DeviceNotFound);
            }
            
            entity.Name = device.Name ?? entity.Name;
            entity.Status = device.Status ?? entity.Status;
            entity.PurchaseDate = device.PurchaseDate ?? entity.PurchaseDate;

            await repository.UpdateAsync(entity, cancellationToken);
            
            return ServiceResponse.ForSuccess();
        }
    
        public async Task<ServiceResponse> DeleteDevice(Guid id, UserRecord requestingUser, CancellationToken cancellationToken = default)
        {
            if (!new List<UserRoleEnum> { UserRoleEnum.Admin, UserRoleEnum.ItEngineer }.Contains(requestingUser.Role))
            {
                return ServiceResponse.FromError(CommonErrors.UnauthorizedDeviceAddOrUpdate);
            }

            await repository.DeleteAsync<Device>(id, cancellationToken);
            
            return ServiceResponse.ForSuccess();
        }
}