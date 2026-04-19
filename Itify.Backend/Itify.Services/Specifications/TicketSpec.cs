using Ardalis.Specification;
using Itify.Database.Repository.Entities;

namespace Itify.Services.Specifications;

public sealed class TicketSpec : Specification<Ticket>
{
    public TicketSpec(Guid id)
    {
        Query.Where(t => t.Id == id);
    }
}