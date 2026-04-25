using Ardalis.Specification;
using Itify.DbService.Entities;
using Microsoft.EntityFrameworkCore;

namespace Itify.DbService.Specifications;

public sealed class TicketSpec : Specification<Ticket>
{
    public TicketSpec(Guid id) => Query.Include(e => e.DeviceAssignment).Where(e => e.Id == id);
    public TicketSpec(Guid deviceAssignmentId, bool withAssignmentId) => Query.Where(e => e.DeviceAssignmentId == deviceAssignmentId);
    public TicketSpec(string? search, Guid? userId)
    {
        Query.Include(e => e.DeviceAssignment).OrderByDescending(e => e.CreatedAt);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = $"%{search.Replace(" ", "%")}%";
            Query.Where(e => EF.Functions.ILike(e.Description, s));
        }
        if (userId.HasValue)
            Query.Where(e => e.UserId == userId.Value);
    }
}
