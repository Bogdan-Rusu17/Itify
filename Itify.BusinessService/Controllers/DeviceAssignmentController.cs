using Itify.BusinessService.DataTransferObjects;
using Itify.BusinessService.Enums;
using Itify.BusinessService.HttpClients;
using Itify.BusinessService.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Itify.BusinessService.Controllers;

[ApiController]
[Route("api/device-assignments")]
[Authorize]
public class DeviceAssignmentController(IDbServiceClient db, MailService mail) : AuthorizedController(db)
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var assignment = await Db.GetDeviceAssignmentAsync(id);
        return assignment is null ? NotFound() : Ok(assignment);
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
    {
        var currentUser = await GetCurrentUser();
        var userId = currentUser.Role == UserRoleEnum.Employee ? currentUser.Id : (Guid?)null;
        return Ok(await Db.GetDeviceAssignmentsAsync(page, pageSize, search, userId));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DeviceAssignmentAddRequest dto)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Role == UserRoleEnum.Employee) return Forbid();

        var device = await Db.GetDeviceAsync(dto.DeviceId);
        if (device is null) return NotFound(new { Message = "Device not found." });
        if (device.Status != DeviceStatusEnum.Available)
            return Conflict(new { Message = "Device is not available for assignment." });

        await Db.UpdateDeviceAsync(dto.DeviceId, new DeviceUpdateRequest { Status = DeviceStatusEnum.Assigned });
        var id = await Db.CreateDeviceAssignmentAsync(dto);

        var user = await Db.GetUserAsync(dto.UserId);
        if (user is not null)
            await mail.SendAsync(user.Email, "Device Assigned to You",
                MailTemplates.DeviceAssigned(user.Name, device.Name, device.SerialNumber, string.Empty));

        return Ok(new { id });
    }

    [HttpPut("{id:guid}/return")]
    public async Task<IActionResult> Return(Guid id)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Role == UserRoleEnum.Employee) return Forbid();

        var assignment = await Db.GetDeviceAssignmentAsync(id);
        if (assignment is null) return NotFound();
        if (assignment.ReturnedAt is not null) return Conflict(new { Message = "Device already returned." });

        await Db.ReturnDeviceAssignmentAsync(id);
        await Db.UpdateDeviceAsync(assignment.DeviceId, new DeviceUpdateRequest { Status = DeviceStatusEnum.Available });
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Role == UserRoleEnum.Employee) return Forbid();

        var assignment = await Db.GetDeviceAssignmentAsync(id);
        if (assignment is null) return NotFound();

        if (assignment.ReturnedAt is null)
            await Db.UpdateDeviceAsync(assignment.DeviceId, new DeviceUpdateRequest { Status = DeviceStatusEnum.Available });

        await Db.DeleteDeviceAssignmentAsync(id);
        return NoContent();
    }
}
