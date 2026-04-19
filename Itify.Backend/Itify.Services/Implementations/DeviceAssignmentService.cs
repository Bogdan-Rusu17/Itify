using Itify.Database.Repository;
using Itify.Database.Repository.Entities;
using Itify.Database.Repository.Enums;
using Itify.Infrastructure.Configurations;
using Itify.Infrastructure.Errors;
using Itify.Infrastructure.Repositories.Interfaces;
using Itify.Infrastructure.Requests;
using Itify.Infrastructure.Responses;
using Itify.Services.Abstractions;
using Itify.Services.Constants;
using Itify.Services.DataTransferObjects;
using Itify.Services.Specifications;
using Microsoft.Extensions.Options;

namespace Itify.Services.Implementations;

public class DeviceAssignmentService(
    IRepository<WebAppDatabaseContext> repository,
    IMailService mailService,
    IOptions<MailConfiguration> mailConfiguration) : IDeviceAssignmentService
{
    public async Task<ServiceResponse<DeviceAssignmentRecord>> GetDeviceAssignment(Guid id, UserRecord requestingUser,
        CancellationToken cancellationToken = default)
    {
        DeviceAssignmentProjectionSpec spec;

        if (requestingUser.Role == UserRoleEnum.Employee)
        {
            spec = new DeviceAssignmentProjectionSpec(id, requestingUser.Id);
        }
        else
        {
            spec = new DeviceAssignmentProjectionSpec(id);
        }

        var result = await repository.GetAsync(spec, cancellationToken);

        if (result == null)
        {
            return ServiceResponse.FromError<DeviceAssignmentRecord>(CommonErrors.DeviceAssignmentNotFound);
        }

        return ServiceResponse.ForSuccess(result);
    }

    public async Task<ServiceResponse<PagedResponse<DeviceAssignmentRecord>>> GetDeviceAssignments(
        PaginationSearchQueryParams pagination, UserRecord requestingUser, CancellationToken cancellationToken = default)
    {
        DeviceAssignmentProjectionSpec spec;

        if (requestingUser.Role == UserRoleEnum.Employee)
        {
            spec = new DeviceAssignmentProjectionSpec(pagination.Search, requestingUser.Id);
        }
        else
        {
            spec = new DeviceAssignmentProjectionSpec(pagination.Search);
        }

        var result = await repository.PageAsync(pagination, spec, cancellationToken);

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

        var employee = await repository.GetAsync(new UserSpec(assignment.UserId), cancellationToken);
        if (employee != null)
        {
            await mailService.SendMail(
                employee.Email,
                "Device Assigned to You",
                MailTemplates.DeviceAssignedTemplate(employee.Name, device.Name, device.SerialNumber, mailConfiguration.Value.FrontendUrl),
                true, "Itify", cancellationToken);
        }

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