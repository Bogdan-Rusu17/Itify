using Itify.DbService.Contexts;
using Itify.DbService.DataTransferObjects;
using Itify.DbService.Entities;
using Itify.DbService.Enums;
using Itify.DbService.Infrastructure;
using Itify.DbService.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace Itify.DbService.Controllers;

[ApiController]
[Route("db/users")]
public class UserDbController(IRepository<UsersDbContext> repo) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await repo.GetAsync<User, UserRecord>(new UserProjectionSpec(id));
        return user is null ? NotFound() : Ok(user);
    }

    [HttpGet("by-email/{email}")]
    public async Task<IActionResult> GetByEmail(string email)
    {
        var user = await repo.GetAsync(new UserSpec(email));
        if (user is null) return NotFound();
        return Ok(new UserRecord { Id = user.Id, Name = user.Name, Email = user.Email, Role = user.Role });
    }

    [HttpGet("with-password/{email}")]
    public async Task<IActionResult> GetWithPassword(string email)
    {
        var user = await repo.GetAsync(new UserSpec(email));
        return user is null ? NotFound() : Ok(user);
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] UserRoleEnum? role = null)
    {
        var pagination = new PaginationQueryParams { Page = page, PageSize = pageSize };
        var result = await repo.PageAsync<User, UserRecord>(pagination, new UserProjectionSpec(search, role));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserAddRecord dto)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email,
            Password = dto.Password,
            Role = dto.Role
        };
        await repo.AddAsync(user);
        return Ok(new UserRecord { Id = user.Id, Name = user.Name, Email = user.Email, Role = user.Role });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateRecord dto)
    {
        var user = await repo.GetAsync<User>(id);
        if (user is null) return NotFound();
        if (dto.Name is not null) user.Name = dto.Name;
        if (dto.Password is not null) user.Password = dto.Password;
        if (dto.Role.HasValue) user.Role = dto.Role.Value;
        await repo.UpdateAsync(user);
        return Ok(new UserRecord { Id = user.Id, Name = user.Name, Email = user.Email, Role = user.Role });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await repo.DeleteAsync<User>(id);
        return deleted == 0 ? NotFound() : NoContent();
    }
}
