using Itify.BusinessService.DataTransferObjects;
using Itify.BusinessService.Enums;
using Itify.BusinessService.HttpClients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Itify.BusinessService.Controllers;

[ApiController]
[Route("devices")]
[Authorize]
public class DeviceController(IDbServiceClient db) : AuthorizedController(db)
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var currentUser = await GetCurrentUser();
        var device = await Db.GetDeviceAsync(id);
        if (device is null) return NotFound();
        if (currentUser.Role == UserRoleEnum.Employee)
        {
            var assignments = await Db.GetDeviceAssignmentsAsync(1, 1000, userId: currentUser.Id);
            if (!assignments.Data.Any(a => a.DeviceId == id && a.ReturnedAt == null))
                return NotFound();
        }
        return Ok(device);
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
    {
        var currentUser = await GetCurrentUser();
        var userId = currentUser.Role == UserRoleEnum.Employee ? currentUser.Id : (Guid?)null;
        return Ok(await Db.GetDevicesAsync(page, pageSize, search, userId));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DeviceAddRequest dto)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Role == UserRoleEnum.Employee) return Forbid();
        var id = await Db.CreateDeviceAsync(dto);
        return Ok(new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] DeviceUpdateRequest dto)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Role == UserRoleEnum.Employee) return Forbid();
        var device = await Db.GetDeviceAsync(id);
        if (device is null) return NotFound();
        await Db.UpdateDeviceAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Role == UserRoleEnum.Employee) return Forbid();
        var device = await Db.GetDeviceAsync(id);
        if (device is null) return NotFound();
        await Db.DeleteDeviceAsync(id);
        return NoContent();
    }
}
