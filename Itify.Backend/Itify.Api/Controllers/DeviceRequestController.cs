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
public class DeviceRequestController(
    ILogger<DeviceRequestController> logger,
    IUserService userService,
    IDeviceRequestService deviceRequestService)
    : AuthorizedController(logger, userService)
{
    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RequestResponse<DeviceRequestRecord>>>
        GetById([FromRoute] Guid id)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Result == null) return ErrorMessageResult<DeviceRequestRecord>(currentUser.Error);
        return FromServiceResponse(await deviceRequestService.GetDeviceRequest(id));
    }

    [Authorize]
    [HttpGet]
    public async
        Task<ActionResult<RequestResponse<PagedResponse<DeviceRequestRecord>>>>
        GetPage([FromQuery] PaginationSearchQueryParams pagination)
    {
        var currentUser = await GetCurrentUser();

        if (currentUser.Result == null)
            return ErrorMessageResult<PagedResponse<DeviceRequestRecord>>(currentUser.Error);

        return FromServiceResponse(await deviceRequestService.GetDeviceRequests(pagination));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<RequestResponse>> Add([FromBody] DeviceRequestAddRecord request)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Result == null) return ErrorMessageResult(currentUser.Error);

        return FromServiceResponse(await deviceRequestService.AddDeviceRequest(request, currentUser.Result));
    }

    [Authorize]
    [HttpPut]
    public async Task<ActionResult<RequestResponse>> Update([FromBody] DeviceRequestUpdateRecord request)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Result == null) return ErrorMessageResult(currentUser.Error);

        return FromServiceResponse(await deviceRequestService.UpdateDeviceRequest(request, currentUser.Result));
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<RequestResponse>> Delete([FromRoute] Guid
        id)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Result == null) return ErrorMessageResult(currentUser.Error);

        return FromServiceResponse(await deviceRequestService.DeleteDeviceRequest(id, currentUser.Result));
    }
}