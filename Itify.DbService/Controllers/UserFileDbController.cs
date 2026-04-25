using Itify.DbService.Contexts;
using Itify.DbService.DataTransferObjects;
using Itify.DbService.Entities;
using Itify.DbService.Infrastructure;
using Itify.DbService.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace Itify.DbService.Controllers;

[ApiController]
[Route("db/user-files")]
public class UserFileDbController(IRepository<UsersDbContext> repo) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var file = await repo.GetAsync<UserFile, UserFileRecord>(new UserFileProjectionSpec(id));
        return file is null ? NotFound() : Ok(file);
    }

    [HttpGet("by-user/{userId:guid}")]
    public async Task<IActionResult> GetByUser(Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var pagination = new PaginationQueryParams { Page = page, PageSize = pageSize };
        var result = await repo.PageAsync<UserFile, UserFileRecord>(pagination, new UserFileProjectionSpec(userId));
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
    {
        var pagination = new PaginationQueryParams { Page = page, PageSize = pageSize };
        var result = await repo.PageAsync<UserFile, UserFileRecord>(pagination, new UserFileProjectionSpec(search));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserFileCreatePayload dto)
    {
        var file = new UserFile { Id = Guid.NewGuid(), Path = dto.Path, Name = dto.Name, Description = dto.Description, UserId = dto.UserId };
        await repo.AddAsync(file);
        return Ok(new { file.Id });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await repo.DeleteAsync<UserFile>(id);
        return deleted == 0 ? NotFound() : NoContent();
    }
}

public class UserFileCreatePayload
{
    public string Path { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public Guid UserId { get; set; }
}
