using Itify.DbService.Contexts;
using Itify.DbService.DataTransferObjects;
using Itify.DbService.Entities;
using Itify.DbService.Infrastructure;
using Itify.DbService.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace Itify.DbService.Controllers;

[ApiController]
[Route("db/user-groups")]
public class UserGroupDbController(IRepository<UsersDbContext> repo) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var group = await repo.GetAsync<UserGroup, UserGroupRecord>(new UserGroupProjectionSpec(id));
        return group is null ? NotFound() : Ok(group);
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
    {
        var pagination = new PaginationQueryParams { Page = page, PageSize = pageSize };
        var result = await repo.PageAsync<UserGroup, UserGroupRecord>(pagination, new UserGroupProjectionSpec(search));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserGroupAddRecord dto)
    {
        var group = new UserGroup { Id = Guid.NewGuid(), Name = dto.Name, Description = dto.Description };
        await repo.AddAsync(group);
        return Ok(new UserGroupRecord { Id = group.Id, Name = group.Name, Description = group.Description });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UserGroupUpdateRecord dto)
    {
        var group = await repo.GetAsync<UserGroup>(id);
        if (group is null) return NotFound();
        if (dto.Name is not null) group.Name = dto.Name;
        if (dto.Description is not null) group.Description = dto.Description;
        await repo.UpdateAsync(group);
        return Ok(new UserGroupRecord { Id = group.Id, Name = group.Name, Description = group.Description });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await repo.DeleteAsync<UserGroup>(id);
        return deleted == 0 ? NotFound() : NoContent();
    }

    [HttpPost("{groupId:guid}/members/{userId:guid}")]
    public async Task<IActionResult> AddMember(Guid groupId, Guid userId)
    {
        var group = await repo.GetAsync(new UserGroupSpec(groupId, includeUsers: true));
        if (group is null) return NotFound("Group not found");
        var user = await repo.GetAsync<User>(userId);
        if (user is null) return NotFound("User not found");
        group.Users.Add(user);
        await repo.UpdateAsync(group);
        return Ok();
    }

    [HttpDelete("{groupId:guid}/members/{userId:guid}")]
    public async Task<IActionResult> RemoveMember(Guid groupId, Guid userId)
    {
        var group = await repo.GetAsync(new UserGroupSpec(groupId, includeUsers: true));
        if (group is null) return NotFound("Group not found");
        var member = group.Users.FirstOrDefault(u => u.Id == userId);
        if (member is null) return NotFound("User not in group");
        group.Users.Remove(member);
        await repo.UpdateAsync(group);
        return Ok();
    }
}
