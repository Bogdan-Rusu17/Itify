using Itify.Infrastructure.Requests;
using Itify.Infrastructure.Responses;
using Itify.Services.DataTransferObjects;

namespace Itify.Services.Abstractions;

public interface ITicketService
{
    public Task<ServiceResponse<TicketRecord>> GetTicket(Guid id, UserRecord requestingUser,
        CancellationToken cancellationToken = default);

    public Task<ServiceResponse<PagedResponse<TicketRecord>>> GetTickets(
        PaginationSearchQueryParams pagination, UserRecord requestingUser, CancellationToken cancellationToken = default);

    public Task<ServiceResponse> AddTicket(TicketAddRecord ticket, UserRecord requestingUser,
        CancellationToken cancellationToken = default);

    public Task<ServiceResponse> UpdateTicket(TicketUpdateRecord ticket, UserRecord requestingUser,
        CancellationToken cancellationToken = default);

    public Task<ServiceResponse> DeleteTicket(Guid id, UserRecord requestingUser,
        CancellationToken cancellationToken = default);
}
