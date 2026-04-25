using Itify.DbService.Contexts;
using Itify.DbService.DataTransferObjects;
using Itify.DbService.Entities;
using Itify.DbService.Infrastructure;
using Itify.DbService.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace Itify.DbService.Controllers;

[ApiController]
[Route("db/devices")]
public class DeviceDbController(IRepository<EquipmentDbContext> repo) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var device = await repo.GetAsync<Device, DeviceRecord>(new DeviceProjectionSpec(id));
        return device is null ? NotFound() : Ok(device);
    }

    [HttpGet("by-serial/{serial}")]
    public async Task<IActionResult> GetBySerial(string serial)
    {
        var device = await repo.GetAsync(new DeviceSpec(serial));
        return device is null ? NotFound() : Ok(device);
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] Guid? assignedToUserId = null)
    {
        var pagination = new PaginationQueryParams { Page = page, PageSize = pageSize };
        var result = await repo.PageAsync<Device, DeviceRecord>(pagination, new DeviceProjectionSpec(search, assignedToUserId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DeviceAddRecord dto)
    {
        var device = new Device
        {
            Id = Guid.NewGuid(), Name = dto.Name, SerialNumber = dto.SerialNumber,
            Status = dto.Status, PurchaseDate = dto.PurchaseDate, CategoryId = dto.CategoryId
        };
        await repo.AddAsync(device);
        return Ok(new { device.Id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] DeviceUpdateRecord dto)
    {
        var device = await repo.GetAsync<Device>(id);
        if (device is null) return NotFound();
        if (dto.Name is not null) device.Name = dto.Name;
        if (dto.Status.HasValue) device.Status = dto.Status.Value;
        if (dto.PurchaseDate.HasValue) device.PurchaseDate = dto.PurchaseDate;
        await repo.UpdateAsync(device);
        return Ok(new { device.Id });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await repo.DeleteAsync<Device>(id);
        return deleted == 0 ? NotFound() : NoContent();
    }
}
