using Ardalis.Specification;
using Itify.Database.Repository.Entities;
using Itify.Services.DataTransferObjects;
using Microsoft.EntityFrameworkCore;

namespace Itify.Services.Specifications;

public sealed class TicketProjectionSpec : Specification<Ticket, TicketRecord>
{
    public TicketProjectionSpec(bool orderByCreatedAt = false)
    {
        Query.OrderByDescending(e => e.CreatedAt, orderByCreatedAt)
            .Select(t => new TicketRecord
            {
                Id = t.Id,
                Description = t.Description,
                Type = t.Type,
                Priority = t.Priority,
                IsUrgent = t.IsUrgent,
                AdditionalNotes = t.AdditionalNotes,
                Status = t.Status,
                ResolvedAt = t.ResolvedAt,
                DeviceAssignmentId = t.DeviceAssignmentId,
                UserId = t.UserId,
                UserName = t.User.Name
            });
    }

    public TicketProjectionSpec(Guid id) : this()
    {
        Query.Where(t => t.Id == id);
    }

    public TicketProjectionSpec(string? searchExpr) : this(true)
    {
        if (string.IsNullOrWhiteSpace(searchExpr)) return;
        var engineSearchExpr = $"%{searchExpr.Replace(" ", "%")}%";
        Query.Where(t => EF.Functions.ILike(t.Description,
            engineSearchExpr));
    }

    public TicketProjectionSpec(string? searchExpr, Guid userId) : this(searchExpr)
    {
        Query.Where(t => t.UserId == userId);
    }

    public TicketProjectionSpec(Guid id, Guid userId) : this(id)
    {
        Query.Where(t => t.UserId == userId);
    }
}