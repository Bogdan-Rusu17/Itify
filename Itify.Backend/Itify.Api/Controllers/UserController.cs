using Itify.Infrastructure.Authorization;
using Itify.Infrastructure.Responses;
using Itify.Services.Abstractions;
using Itify.Services.Authorization;
using Itify.Services.DataTransferObjects;
using Itify.Services.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Itify.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class UserController(ILogger<UserController> logger, IUserService userService)
    : AuthorizedController(logger, userService)
{
    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RequestResponse<UserRecord>>> GetById([FromRoute] Guid id)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Result == null) return ErrorMessageResult<UserRecord>(currentUser.Error);

        return FromServiceResponse(await UserService.GetUser(id, currentUser.Result));
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<RequestResponse<PagedResponse<UserRecord>>>> GetPage(
        [FromQuery] UserPaginationQueryParams pagination)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Result == null) return ErrorMessageResult<PagedResponse<UserRecord>>(currentUser.Error);

        return FromServiceResponse(await UserService.GetUsers(pagination));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<RequestResponse>> Add([FromBody] UserAddRecord user)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Result == null) return ErrorMessageResult(currentUser.Error);

        user.Password = PasswordUtils.HashPassword(user.Password);

        return FromServiceResponse(await UserService.AddUser(user, currentUser.Result));
    }

    [Authorize]
    [HttpPut]
    public async Task<ActionResult<RequestResponse>> Update([FromBody] UserUpdateRecord user)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Result == null) return ErrorMessageResult(currentUser.Error);

        return FromServiceResponse(await UserService.UpdateUser(user with
        {
            Password = !string.IsNullOrWhiteSpace(user.Password) ? PasswordUtils.HashPassword(user.Password) : null
        }, currentUser.Result));
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<RequestResponse>> Delete([FromRoute] Guid id)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Result == null) return ErrorMessageResult(currentUser.Error);

        return FromServiceResponse(await UserService.DeleteUser(id, currentUser.Result));
    }
}