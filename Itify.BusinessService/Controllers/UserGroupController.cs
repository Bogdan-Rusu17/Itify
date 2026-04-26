using Itify.BusinessService.DataTransferObjects;
using Itify.BusinessService.Enums;
using Itify.BusinessService.HttpClients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Itify.BusinessService.Controllers;

[ApiController]
[Route("user-groups")]
[Authorize]
public class UserGroupController(IDbServiceClient db) : AuthorizedController(db)
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var group = await Db.GetUserGroupAsync(id);
        return group is null ? NotFound() : Ok(group);
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
        => Ok(await Db.GetUserGroupsAsync(page, pageSize, search));

    [HttpGet("my")]
    public async Task<IActionResult> GetMyGroups([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var currentUser = await GetCurrentUser();
        return Ok(await Db.GetGroupsForUserAsync(currentUser.Id, page, pageSize));
    }

    [HttpGet("{groupId:guid}/users")]
    public async Task<IActionResult> GetUsers(Guid groupId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Role == UserRoleEnum.Employee) return Forbid();
        return Ok(await Db.GetUsersInGroupAsync(groupId, page, pageSize));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserGroupAddRequest dto)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Role != UserRoleEnum.Admin) return Forbid();
        var id = await Db.CreateUserGroupAsync(dto);
        return Ok(new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UserGroupUpdateRequest dto)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Role != UserRoleEnum.Admin) return Forbid();
        var group = await Db.GetUserGroupAsync(id);
        if (group is null) return NotFound();
        await Db.UpdateUserGroupAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Role != UserRoleEnum.Admin) return Forbid();
        var group = await Db.GetUserGroupAsync(id);
        if (group is null) return NotFound();
        await Db.DeleteUserGroupAsync(id);
        return NoContent();
    }

    [HttpPost("{groupId:guid}/users/{userId:guid}")]
    public async Task<IActionResult> AddUser(Guid groupId, Guid userId)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Role != UserRoleEnum.Admin) return Forbid();
        await Db.AddUserToGroupAsync(groupId, userId);
        return Ok();
    }

    [HttpDelete("{groupId:guid}/users/{userId:guid}")]
    public async Task<IActionResult> RemoveUser(Guid groupId, Guid userId)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Role != UserRoleEnum.Admin) return Forbid();
        await Db.RemoveUserFromGroupAsync(groupId, userId);
        return Ok();
    }
}
