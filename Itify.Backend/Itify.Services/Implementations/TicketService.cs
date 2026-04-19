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

public class TicketService(IRepository<WebAppDatabaseContext> repository) : ITicketService
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
        }

        await repository.UpdateAsync(entity, cancellationToken);

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

        await repository.DeleteAsync<Ticket>(id, cancellationToken);

        return ServiceResponse.ForSuccess();
    }
}
