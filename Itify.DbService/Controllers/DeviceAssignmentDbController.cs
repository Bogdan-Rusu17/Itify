using Itify.DbService.Contexts;
using Itify.DbService.DataTransferObjects;
using Itify.DbService.Entities;
using Itify.DbService.Infrastructure;
using Itify.DbService.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace Itify.DbService.Controllers;

[ApiController]
[Route("db/device-assignments")]
public class DeviceAssignmentDbController(
    IRepository<EquipmentDbContext> equipmentRepo,
    IRepository<UsersDbContext> usersRepo) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var assignment = await equipmentRepo.GetAsync(new DeviceAssignmentSpec(id));
        if (assignment is null) return NotFound();
        var enriched = await Enrich([assignment]);
        return Ok(enriched.First());
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] Guid? userId = null)
    {
        var spec = new DeviceAssignmentSpec(search, userId);
        var count = await equipmentRepo.GetCountAsync(spec);
        var all = await equipmentRepo.ListAsync(spec);
        var paged = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        var enriched = await Enrich(paged);
        return Ok(new PagedResponse<DeviceAssignmentRecord>(page, pageSize, count, enriched));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DeviceAssignmentAddRecord dto)
    {
        var assignment = new DeviceAssignment
        {
            Id = Guid.NewGuid(), DeviceId = dto.DeviceId,
            UserId = dto.UserId, AssignedAt = DateTime.UtcNow
        };
        await equipmentRepo.AddAsync(assignment);
        return Ok(new { assignment.Id });
    }

    [HttpPut("{id:guid}/return")]
    public async Task<IActionResult> Return(Guid id)
    {
        var assignment = await equipmentRepo.GetAsync<DeviceAssignment>(id);
        if (assignment is null) return NotFound();
        assignment.ReturnedAt = DateTime.UtcNow;
        await equipmentRepo.UpdateAsync(assignment);
        return Ok(new { assignment.Id });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await equipmentRepo.DeleteAsync<DeviceAssignment>(id);
        return deleted == 0 ? NotFound() : NoContent();
    }

    private async Task<List<DeviceAssignmentRecord>> Enrich(List<DeviceAssignment> assignments)
    {
        var userIds = assignments.Select(a => a.UserId).Distinct().ToList();
        var users = await usersRepo.ListAsync(new UserSpec(userIds));
        var userMap = users.ToDictionary(u => u.Id, u => u.Name);

        return assignments.Select(a => new DeviceAssignmentRecord
        {
            Id = a.Id, DeviceId = a.DeviceId,
            DeviceName = a.Device?.Name ?? string.Empty,
            SerialNumber = a.Device?.SerialNumber ?? string.Empty,
            UserId = a.UserId,
            UserName = userMap.GetValueOrDefault(a.UserId, "Unknown"),
            AssignedAt = a.AssignedAt, ReturnedAt = a.ReturnedAt
        }).ToList();
    }
}
