using Itify.DbService.Contexts;
using Itify.DbService.DataTransferObjects;
using Itify.DbService.Entities;
using Itify.DbService.Infrastructure;
using Itify.DbService.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace Itify.DbService.Controllers;

[ApiController]
[Route("db/device-categories")]
public class DeviceCategoryDbController(IRepository<EquipmentDbContext> repo) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var cat = await repo.GetAsync<DeviceCategory, DeviceCategoryRecord>(new DeviceCategoryProjectionSpec(id));
        return cat is null ? NotFound() : Ok(cat);
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
    {
        var pagination = new PaginationQueryParams { Page = page, PageSize = pageSize };
        var result = await repo.PageAsync<DeviceCategory, DeviceCategoryRecord>(pagination, new DeviceCategoryProjectionSpec(search));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DeviceCategoryAddRecord dto)
    {
        var cat = new DeviceCategory { Id = Guid.NewGuid(), Name = dto.Name, Description = dto.Description };
        await repo.AddAsync(cat);
        return Ok(new DeviceCategoryRecord { Id = cat.Id, Name = cat.Name, Description = cat.Description });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] DeviceCategoryUpdateRecord dto)
    {
        var cat = await repo.GetAsync<DeviceCategory>(id);
        if (cat is null) return NotFound();
        if (dto.Name is not null) cat.Name = dto.Name;
        if (dto.Description is not null) cat.Description = dto.Description;
        await repo.UpdateAsync(cat);
        return Ok(new DeviceCategoryRecord { Id = cat.Id, Name = cat.Name, Description = cat.Description });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await repo.DeleteAsync<DeviceCategory>(id);
        return deleted == 0 ? NotFound() : NoContent();
    }
}
