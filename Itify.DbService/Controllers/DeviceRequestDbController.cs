using Itify.DbService.Contexts;
using Itify.DbService.DataTransferObjects;
using Itify.DbService.Entities;
using Itify.DbService.Infrastructure;
using Itify.DbService.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace Itify.DbService.Controllers;

[ApiController]
[Route("db/device-requests")]
public class DeviceRequestDbController(
    IRepository<EquipmentDbContext> equipmentRepo,
    IRepository<UsersDbContext> usersRepo) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var req = await equipmentRepo.GetAsync(new DeviceRequestSpec(id));
        if (req is null) return NotFound();
        return Ok((await Enrich([req])).First());
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] Guid? userId = null)
    {
        var spec = new DeviceRequestSpec(search, userId);
        var count = await equipmentRepo.GetCountAsync(spec);
        var all = await equipmentRepo.ListAsync(spec);
        var paged = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        var enriched = await Enrich(paged);
        return Ok(new PagedResponse<DeviceRequestRecord>(page, pageSize, count, enriched));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DeviceRequestAddRecord dto)
    {
        var req = new DeviceRequest
        {
            Id = Guid.NewGuid(), Reason = dto.Reason,
            UserId = dto.UserId, CategoryId = dto.CategoryId,
            Status = Enums.RequestStatusEnum.Pending
        };
        await equipmentRepo.AddAsync(req);
        return Ok(new { req.Id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] DeviceRequestUpdateRecord dto)
    {
        var req = await equipmentRepo.GetAsync<DeviceRequest>(id);
        if (req is null) return NotFound();
        if (dto.Status.HasValue) req.Status = dto.Status.Value;
        if (dto.Reason is not null) req.Reason = dto.Reason;
        await equipmentRepo.UpdateAsync(req);
        return Ok(new { req.Id });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await equipmentRepo.DeleteAsync<DeviceRequest>(id);
        return deleted == 0 ? NotFound() : NoContent();
    }

    private async Task<List<DeviceRequestRecord>> Enrich(List<DeviceRequest> requests)
    {
        var userIds = requests.Select(r => r.UserId).Distinct().ToList();
        var users = await usersRepo.ListAsync(new UserSpec(userIds));
        var userMap = users.ToDictionary(u => u.Id, u => u.Name);

        return requests.Select(r => new DeviceRequestRecord
        {
            Id = r.Id, Reason = r.Reason, Status = r.Status,
            UserId = r.UserId, UserName = userMap.GetValueOrDefault(r.UserId, "Unknown"),
            CategoryId = r.CategoryId, CategoryName = r.Category?.Name ?? string.Empty
        }).ToList();
    }
}
