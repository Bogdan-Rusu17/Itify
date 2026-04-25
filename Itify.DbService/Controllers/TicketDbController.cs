using Itify.DbService.Contexts;
using Itify.DbService.DataTransferObjects;
using Itify.DbService.Entities;
using Itify.DbService.Enums;
using Itify.DbService.Infrastructure;
using Itify.DbService.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace Itify.DbService.Controllers;

[ApiController]
[Route("db/tickets")]
public class TicketDbController(
    IRepository<EquipmentDbContext> equipmentRepo,
    IRepository<UsersDbContext> usersRepo) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var ticket = await equipmentRepo.GetAsync(new TicketSpec(id));
        if (ticket is null) return NotFound();
        return Ok((await Enrich([ticket])).First());
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] Guid? userId = null)
    {
        var spec = new TicketSpec(search, userId);
        var count = await equipmentRepo.GetCountAsync(spec);
        var all = await equipmentRepo.ListAsync(spec);
        var paged = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        var enriched = await Enrich(paged);
        return Ok(new PagedResponse<TicketRecord>(page, pageSize, count, enriched));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TicketAddRecord dto)
    {
        var ticket = new Ticket
        {
            Id = Guid.NewGuid(), Description = dto.Description,
            Type = dto.Type, Priority = dto.Priority, IsUrgent = dto.IsUrgent,
            AdditionalNotes = dto.AdditionalNotes, Status = TicketStatusEnum.Open,
            DeviceAssignmentId = dto.DeviceAssignmentId, UserId = dto.UserId
        };
        await equipmentRepo.AddAsync(ticket);
        return Ok(new { ticket.Id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] TicketUpdateRecord dto)
    {
        var ticket = await equipmentRepo.GetAsync<Ticket>(id);
        if (ticket is null) return NotFound();
        ticket.Status = dto.Status;
        if (dto.ResolvedAt.HasValue) ticket.ResolvedAt = dto.ResolvedAt;
        await equipmentRepo.UpdateAsync(ticket);
        return Ok(new { ticket.Id });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await equipmentRepo.DeleteAsync<Ticket>(id);
        return deleted == 0 ? NotFound() : NoContent();
    }

    private async Task<List<TicketRecord>> Enrich(List<Ticket> tickets)
    {
        var userIds = tickets.Select(t => t.UserId).Distinct().ToList();
        var users = await usersRepo.ListAsync(new UserSpec(userIds));
        var userMap = users.ToDictionary(u => u.Id, u => u.Name);

        return tickets.Select(t => new TicketRecord
        {
            Id = t.Id, Description = t.Description, Type = t.Type,
            Priority = t.Priority, IsUrgent = t.IsUrgent, AdditionalNotes = t.AdditionalNotes,
            Status = t.Status, ResolvedAt = t.ResolvedAt,
            DeviceAssignmentId = t.DeviceAssignmentId,
            UserId = t.UserId, UserName = userMap.GetValueOrDefault(t.UserId, "Unknown")
        }).ToList();
    }
}
