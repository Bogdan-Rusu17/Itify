using Itify.Infrastructure.Requests;
using Itify.Infrastructure.Responses;
using Itify.Services.Abstractions;
using Itify.Services.Authorization;
using Itify.Services.DataTransferObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Itify.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class UserGroupController(
    ILogger<UserGroupController> logger,
    IUserService userService,
    IUserGroupService userGroupService)
    : AuthorizedController(logger, userService)
{
    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RequestResponse<UserGroupRecord>>> GetById([FromRoute] Guid id)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Result == null) return ErrorMessageResult<UserGroupRecord>(currentUser.Error);

        return FromServiceResponse(await userGroupService.GetUserGroup(id));
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<RequestResponse<PagedResponse<UserGroupRecord>>>> GetPage(
        [FromQuery] PaginationSearchQueryParams pagination)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Result == null) return ErrorMessageResult<PagedResponse<UserGroupRecord>>(currentUser.Error);

        return FromServiceResponse(await userGroupService.GetUserGroups(pagination));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<RequestResponse>> Add([FromBody] UserGroupAddRecord userGroup)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Result == null) return ErrorMessageResult(currentUser.Error);

        return FromServiceResponse(await userGroupService.AddUserGroup(userGroup, currentUser.Result));
    }

    [Authorize]
    [HttpPut]
    public async Task<ActionResult<RequestResponse>> Update([FromBody] UserGroupUpdateRecord userGroup)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Result == null) return ErrorMessageResult(currentUser.Error);

        return FromServiceResponse(await userGroupService.UpdateUserGroup(userGroup, currentUser.Result));
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<RequestResponse>> Delete([FromRoute] Guid id)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Result == null) return ErrorMessageResult(currentUser.Error);

        return FromServiceResponse(await userGroupService.DeleteUserGroup(id, currentUser.Result));
    }

    [Authorize]
    [HttpGet("{groupId:guid}/users")]
    public async Task<ActionResult<RequestResponse<PagedResponse<UserRecord>>>> GetUsers(
        [FromRoute] Guid groupId, [FromQuery] PaginationSearchQueryParams pagination)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Result == null) return ErrorMessageResult<PagedResponse<UserRecord>>(currentUser.Error);

        return FromServiceResponse(await userGroupService.GetUsersInGroup(groupId, pagination, currentUser.Result));
    }

    [Authorize]
    [HttpGet("my")]
    public async Task<ActionResult<RequestResponse<PagedResponse<UserGroupRecord>>>> GetMyGroups(
        [FromQuery] PaginationSearchQueryParams pagination)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Result == null) return ErrorMessageResult<PagedResponse<UserGroupRecord>>(currentUser.Error);

        return FromServiceResponse(await userGroupService.GetGroupsForUser(currentUser.Result.Id, pagination));
    }

    [Authorize]
    [HttpPost("{groupId:guid}/users/{userId:guid}")]
    public async Task<ActionResult<RequestResponse>> AddUser([FromRoute] Guid groupId, [FromRoute] Guid userId)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Result == null) return ErrorMessageResult(currentUser.Error);

        return FromServiceResponse(await userGroupService.AddUserToGroup(groupId, userId, currentUser.Result));
    }

    [Authorize]
    [HttpDelete("{groupId:guid}/users/{userId:guid}")]
    public async Task<ActionResult<RequestResponse>> RemoveUser([FromRoute] Guid groupId, [FromRoute] Guid userId)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Result == null) return ErrorMessageResult(currentUser.Error);

        return FromServiceResponse(await userGroupService.RemoveUserFromGroup(groupId, userId, currentUser.Result));
    }
}
