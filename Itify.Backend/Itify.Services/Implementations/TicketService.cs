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

public class TicketService(
    IRepository<WebAppDatabaseContext> repository,
    IMailService mailService,
    IOptions<MailConfiguration> mailConfiguration) : ITicketService
{
    public async Task<ServiceResponse<TicketRecord>> GetTicket(Guid id, UserRecord requestingUser,
        CancellationToken cancellationToken = default)
    {
        TicketProjectionSpec spec;

        if (requestingUser.Role == UserRoleEnum.Employee)
        {
            spec = new TicketProjectionSpec(id, requestingUser.Id);
        }
        else
        {
            spec = new TicketProjectionSpec(id);
        }

        var result = await repository.GetAsync(spec, cancellationToken);

        if (result == null)
        {
            return ServiceResponse.FromError<TicketRecord>(CommonErrors.TicketNotFound);
        }

        return ServiceResponse.ForSuccess(result);
    }

    public async Task<ServiceResponse<PagedResponse<TicketRecord>>> GetTickets(
        PaginationSearchQueryParams pagination, UserRecord requestingUser, CancellationToken cancellationToken = default)
    {
        TicketProjectionSpec spec;

        if (requestingUser.Role == UserRoleEnum.Employee)
        {
            spec = new TicketProjectionSpec(pagination.Search, requestingUser.Id);
        }
        else
        {
            spec = new TicketProjectionSpec(pagination.Search);
        }

        var result = await repository.PageAsync(pagination, spec, cancellationToken);

        return ServiceResponse.ForSuccess(result);
    }

    public async Task<ServiceResponse> AddTicket(TicketAddRecord ticket, UserRecord requestingUser,
        CancellationToken cancellationToken = default)
    {
        var assignment = await repository.GetAsync(new DeviceAssignmentSpec(ticket.DeviceAssignmentId), cancellationToken);

        if (assignment == null)
        {
            return ServiceResponse.FromError(CommonErrors.DeviceAssignmentNotFound);
        }

        if (assignment.UserId != requestingUser.Id)
        {
            return ServiceResponse.FromError(CommonErrors.DeviceAssignmentUnauthorized);
        }

        var existingTicket = await repository.GetAsync(new TicketSpec(ticket.DeviceAssignmentId, true), cancellationToken);

        if (existingTicket != null)
        {
            if (existingTicket.Status != TicketStatusEnum.Resolved)
            {
                return ServiceResponse.FromError(CommonErrors.TicketAlreadyExists);
            }

            await repository.DeleteAsync<Ticket>(existingTicket.Id, cancellationToken);
        }

        if (ticket.Type == TicketTypeEnum.RepairRequest)
        {
            var device = await repository.GetAsync(new DeviceSpec(assignment.DeviceId), cancellationToken);

            if (device == null)
            {
                return ServiceResponse.FromError(CommonErrors.DeviceNotFound);
            }

            if (device.Status == DeviceStatusEnum.InRepair)
            {
                return ServiceResponse.FromError(CommonErrors.DeviceAlreadyInRepair);
            }

            device.Status = DeviceStatusEnum.InRepair;
            await repository.UpdateAsync(device, cancellationToken);
        }

        await repository.AddAsync(new Ticket
        {
            Description = ticket.Description,
            Type = ticket.Type,
            Priority = ticket.Priority,
            IsUrgent = ticket.IsUrgent,
            AdditionalNotes = ticket.AdditionalNotes,
            Status = TicketStatusEnum.Open,
            DeviceAssignmentId = ticket.DeviceAssignmentId,
            UserId = requestingUser.Id
        }, cancellationToken);

        var staff = await repository.ListAsync(
            new UserSpec(new List<UserRoleEnum> { UserRoleEnum.Admin, UserRoleEnum.ItEngineer }), cancellationToken);

        foreach (var member in staff)
        {
            await mailService.SendMail(
                member.Email,
                "New Ticket Submitted",
                MailTemplates.TicketCreatedTemplate(requestingUser.Name, ticket.Description, ticket.Type.ToString(), mailConfiguration.Value.FrontendUrl),
                true, "Itify", cancellationToken);
        }

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> UpdateTicket(TicketUpdateRecord ticket, UserRecord requestingUser,
        CancellationToken cancellationToken = default)
    {
        if (!new List<UserRoleEnum> { UserRoleEnum.Admin, UserRoleEnum.ItEngineer }.Contains(requestingUser.Role))
        {
            return ServiceResponse.FromError(CommonErrors.TicketUnauthorizedUpdate);
        }

        var entity = await repository.GetAsync(new TicketSpec(ticket.Id), cancellationToken);

        if (entity == null)
        {
            return ServiceResponse.FromError(CommonErrors.TicketNotFound);
        }

        if (entity.Status == TicketStatusEnum.Resolved)
        {
            return ServiceResponse.FromError(CommonErrors.TicketAlreadyResolved);
        }

        entity.Status = ticket.Status;

        if (ticket.Status == TicketStatusEnum.Resolved)
        {
            entity.ResolvedAt = DateTime.UtcNow;

            if (entity.Type == TicketTypeEnum.RepairRequest)
            {
                var assignment = await repository.GetAsync(new DeviceAssignmentSpec(entity.DeviceAssignmentId), cancellationToken);

                if (assignment != null)
                {
                    var device = await repository.GetAsync(new DeviceSpec(assignment.DeviceId), cancellationToken);

                    if (device != null)
                    {
                        device.Status = DeviceStatusEnum.Assigned;
                        await repository.UpdateAsync(device, cancellationToken);
                    }
                }
            }
        }

        await repository.UpdateAsync(entity, cancellationToken);

        var employee = await repository.GetAsync(new UserSpec(entity.UserId), cancellationToken);
        if (employee != null)
        {
            await mailService.SendMail(
                employee.Email,
                "Ticket Status Updated",
                MailTemplates.TicketStatusUpdatedTemplate(employee.Name, entity.Description, ticket.Status.ToString(), mailConfiguration.Value.FrontendUrl),
                true, "Itify", cancellationToken);
        }

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> DeleteTicket(Guid id, UserRecord requestingUser,
        CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetAsync(new TicketSpec(id), cancellationToken);

        if (entity == null)
        {
            return ServiceResponse.FromError(CommonErrors.TicketNotFound);
        }

        if (!new List<UserRoleEnum> { UserRoleEnum.Admin, UserRoleEnum.ItEngineer }.Contains(requestingUser.Role))
        {
            return ServiceResponse.FromError(CommonErrors.TicketUnauthorizedDelete);
        }

        if (entity.Status != TicketStatusEnum.Resolved && entity.Type == TicketTypeEnum.RepairRequest)
        {
            var assignment = await repository.GetAsync(new DeviceAssignmentSpec(entity.DeviceAssignmentId), cancellationToken);

            if (assignment != null)
            {
                var device = await repository.GetAsync(new DeviceSpec(assignment.DeviceId), cancellationToken);

                if (device != null)
                {
                    device.Status = DeviceStatusEnum.Assigned;
                    await repository.UpdateAsync(device, cancellationToken);
                }
            }
        }

        await repository.DeleteAsync<Ticket>(id, cancellationToken);

        return ServiceResponse.ForSuccess();
    }
}
