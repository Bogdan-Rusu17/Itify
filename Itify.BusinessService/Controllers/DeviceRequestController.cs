using Itify.BusinessService.DataTransferObjects;
using Itify.BusinessService.Enums;
using Itify.BusinessService.HttpClients;
using Itify.BusinessService.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Itify.BusinessService.Controllers;

[ApiController]
[Route("device-requests")]
[Authorize]
public class DeviceRequestController(IDbServiceClient db, MailService mail) : AuthorizedController(db)
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var currentUser = await GetCurrentUser();
        var request = await Db.GetDeviceRequestAsync(id);
        if (request is null) return NotFound();
        if (currentUser.Role == UserRoleEnum.Employee && request.UserId != currentUser.Id) return NotFound();
        return Ok(request);
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
    {
        var currentUser = await GetCurrentUser();
        var userId = currentUser.Role == UserRoleEnum.Employee ? currentUser.Id : (Guid?)null;
        return Ok(await Db.GetDeviceRequestsAsync(page, pageSize, search, userId));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DeviceRequestAddRequest dto)
    {
        var currentUser = await GetCurrentUser();

        var category = await Db.GetDeviceCategoryAsync(dto.CategoryId);
        if (category is null) return NotFound(new { Message = "Device category not found." });

        if (await Db.HasPendingDeviceRequestAsync(currentUser.Id, dto.CategoryId))
            return Conflict(new { Message = "You already have a pending request for this category." });

        var id = await Db.CreateDeviceRequestAsync(currentUser.Id, dto);

        var staff = await Db.GetStaffAsync();
        foreach (var member in staff)
            await mail.SendAsync(member.Email, "New Device Request",
                MailTemplates.DeviceRequestCreated(currentUser.Name, category.Name, dto.Reason, string.Empty));

        return Ok(new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] DeviceRequestUpdateRequest dto)
    {
        var currentUser = await GetCurrentUser();

        var request = await Db.GetDeviceRequestAsync(id);
        if (request is null) return NotFound();

        if (currentUser.Role == UserRoleEnum.Employee && request.UserId != currentUser.Id) return Forbid();
        if (currentUser.Role == UserRoleEnum.Employee && dto.Status is not null and not RequestStatusEnum.Pending) return Forbid();
        if (request.Status != RequestStatusEnum.Pending)
            return Conflict(new { Message = "Cannot update a request that has already been resolved." });

        await Db.UpdateDeviceRequestAsync(id, dto);

        if (dto.Status == RequestStatusEnum.Approved)
        {
            var available = await Db.GetFirstAvailableDeviceAsync(request.CategoryId);
            if (available is null)
                return Conflict(new { Message = "No available device in this category." });

            await Db.UpdateDeviceAsync(available.Value.Id, new DeviceUpdateRequest { Status = DeviceStatusEnum.Assigned });
            await Db.CreateDeviceAssignmentAsync(new DeviceAssignmentAddRequest { DeviceId = available.Value.Id, UserId = request.UserId });
        }

        if (dto.Status is RequestStatusEnum.Approved or RequestStatusEnum.Rejected)
        {
            var employee = await Db.GetUserAsync(request.UserId);
            var category = await Db.GetDeviceCategoryAsync(request.CategoryId);
            if (employee is not null && category is not null)
                await mail.SendAsync(employee.Email, $"Device Request {dto.Status}",
                    MailTemplates.DeviceRequestStatusUpdated(employee.Name, category.Name, dto.Status.ToString()!, string.Empty));
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var currentUser = await GetCurrentUser();

        var request = await Db.GetDeviceRequestAsync(id);
        if (request is null) return NotFound();

        if (currentUser.Role == UserRoleEnum.Employee && request.UserId != currentUser.Id) return Forbid();
        if (currentUser.Role == UserRoleEnum.Employee && request.Status != RequestStatusEnum.Pending)
            return Conflict(new { Message = "Cannot delete a request that has already been resolved." });

        await Db.DeleteDeviceRequestAsync(id);
        return NoContent();
    }
}
