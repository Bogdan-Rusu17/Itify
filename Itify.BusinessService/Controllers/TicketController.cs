using Itify.BusinessService.DataTransferObjects;
using Itify.BusinessService.Enums;
using Itify.BusinessService.HttpClients;
using Itify.BusinessService.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Itify.BusinessService.Controllers;

[ApiController]
[Route("tickets")]
[Authorize]
public class TicketController(IDbServiceClient db, MailService mail) : AuthorizedController(db)
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var currentUser = await GetCurrentUser();
        var ticket = await Db.GetTicketAsync(id);
        if (ticket is null) return NotFound();
        if (currentUser.Role == UserRoleEnum.Employee && ticket.UserId != currentUser.Id) return NotFound();
        return Ok(ticket);
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
    {
        var currentUser = await GetCurrentUser();
        var userId = currentUser.Role == UserRoleEnum.Employee ? currentUser.Id : (Guid?)null;
        return Ok(await Db.GetTicketsAsync(page, pageSize, search, userId));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TicketAddRequest dto)
    {
        var currentUser = await GetCurrentUser();

        var assignment = await Db.GetDeviceAssignmentAsync(dto.DeviceAssignmentId);
        if (assignment is null) return NotFound(new { Message = "Device assignment not found." });
        if (assignment.UserId != currentUser.Id) return Forbid();

        var existingTicket = await Db.GetTicketByAssignmentAsync(dto.DeviceAssignmentId);
        if (existingTicket is not null)
        {
            if (existingTicket.Status != TicketStatusEnum.Resolved)
                return Conflict(new { Message = "An unresolved ticket already exists for this device assignment." });
            await Db.DeleteTicketAsync(existingTicket.Id);
        }

        if (dto.Type == TicketTypeEnum.RepairRequest)
        {
            var device = await Db.GetDeviceAsync(assignment.DeviceId);
            if (device is null) return NotFound(new { Message = "Device not found." });
            if (device.Status == DeviceStatusEnum.InRepair)
                return Conflict(new { Message = "Device is already in repair." });
            await Db.UpdateDeviceAsync(assignment.DeviceId, new DeviceUpdateRequest { Status = DeviceStatusEnum.InRepair });
        }

        await Db.CreateTicketAsync(currentUser.Id, dto);

        var staff = await Db.GetStaffAsync();
        foreach (var member in staff)
            await mail.SendAsync(member.Email, "New Ticket Submitted",
                MailTemplates.TicketCreated(currentUser.Name, dto.Description, dto.Type.ToString(), string.Empty));

        return Created(string.Empty, null);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] TicketUpdateRequest dto)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Role == UserRoleEnum.Employee) return Forbid();

        var ticket = await Db.GetTicketAsync(id);
        if (ticket is null) return NotFound();
        if (ticket.Status == TicketStatusEnum.Resolved)
            return Conflict(new { Message = "Ticket is already resolved." });

        await Db.UpdateTicketAsync(id, dto);

        if (dto.Status == TicketStatusEnum.Resolved && ticket.Type == TicketTypeEnum.RepairRequest)
        {
            var assignment = await Db.GetDeviceAssignmentAsync(ticket.DeviceAssignmentId);
            if (assignment is not null)
                await Db.UpdateDeviceAsync(assignment.DeviceId, new DeviceUpdateRequest { Status = DeviceStatusEnum.Assigned });
        }

        var employee = await Db.GetUserAsync(ticket.UserId);
        if (employee is not null)
            await mail.SendAsync(employee.Email, "Ticket Status Updated",
                MailTemplates.TicketStatusUpdated(employee.Name, ticket.Description, dto.Status.ToString(), string.Empty));

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Role == UserRoleEnum.Employee) return Forbid();

        var ticket = await Db.GetTicketAsync(id);
        if (ticket is null) return NotFound();

        if (ticket.Status != TicketStatusEnum.Resolved && ticket.Type == TicketTypeEnum.RepairRequest)
        {
            var assignment = await Db.GetDeviceAssignmentAsync(ticket.DeviceAssignmentId);
            if (assignment is not null)
                await Db.UpdateDeviceAsync(assignment.DeviceId, new DeviceUpdateRequest { Status = DeviceStatusEnum.Assigned });
        }

        await Db.DeleteTicketAsync(id);
        return NoContent();
    }
}
