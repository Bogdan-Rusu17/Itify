using Itify.BusinessService.DataTransferObjects;
using Itify.BusinessService.Enums;
using Itify.BusinessService.HttpClients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Itify.BusinessService.Controllers;

[ApiController]
[Route("device-categories")]
[Authorize]
public class DeviceCategoryController(IDbServiceClient db) : AuthorizedController(db)
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var category = await Db.GetDeviceCategoryAsync(id);
        return category is null ? NotFound() : Ok(category);
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
        => Ok(await Db.GetDeviceCategoriesAsync(page, pageSize, search));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DeviceCategoryAddRequest dto)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Role == UserRoleEnum.Employee) return Forbid();
        var id = await Db.CreateDeviceCategoryAsync(dto);
        return Ok(new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] DeviceCategoryUpdateRequest dto)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Role == UserRoleEnum.Employee) return Forbid();
        var category = await Db.GetDeviceCategoryAsync(id);
        if (category is null) return NotFound();
        await Db.UpdateDeviceCategoryAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Role == UserRoleEnum.Employee) return Forbid();
        var category = await Db.GetDeviceCategoryAsync(id);
        if (category is null) return NotFound();
        await Db.DeleteDeviceCategoryAsync(id);
        return NoContent();
    }
}
