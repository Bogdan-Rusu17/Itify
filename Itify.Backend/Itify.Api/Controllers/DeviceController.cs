using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Itify.Infrastructure.Requests;
using Itify.Infrastructure.Responses;
using Itify.Services.Abstractions;
using Itify.Services.Authorization;
using Itify.Services.DataTransferObjects;

namespace Itify.Api.Controllers;
[ApiController]
[Route("api/[controller]/[action]")]
public class DeviceController(ILogger<DeviceController> logger,
                              IUserService userService,
                              IDeviceService deviceService)
: AuthorizedController(logger, userService)
{
    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RequestResponse<DeviceRecord>>> GetById([FromRoute] Guid id)
    {
        var currentUser = await GetCurrentUser();
        
        if (currentUser.Result == null)
        {
            return ErrorMessageResult<DeviceRecord>(currentUser.Error);
        }

        return FromServiceResponse(await deviceService.GetDevice(id, currentUser.Result));
    }
    
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<RequestResponse<PagedResponse<DeviceRecord>>>> GetPage([FromQuery] PaginationSearchQueryParams pagination)
    {
        var currentUser = await GetCurrentUser();

        if (currentUser.Result == null)
        {
            return ErrorMessageResult<PagedResponse<DeviceRecord>>(currentUser.Error);
        }

        return FromServiceResponse(await deviceService.GetDevices(pagination, currentUser.Result));
    }
    
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<RequestResponse>> Add([FromBody] DeviceAddRecord device)
    {
        var currentUser = await GetCurrentUser();

        if (currentUser.Result == null)
        {
            return ErrorMessageResult(currentUser.Error);
        }

        return FromServiceResponse(await deviceService.AddDevice(device, currentUser.Result));
    }
    
    [Authorize]
    [HttpPut]
    public async Task<ActionResult<RequestResponse>> Update([FromBody] DeviceUpdateRecord device)
    {
        var currentUser = await GetCurrentUser();

        if (currentUser.Result == null)
        {
            return ErrorMessageResult(currentUser.Error);
        }

        return FromServiceResponse(await deviceService.UpdateDevice(device, currentUser.Result));
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<RequestResponse>> Delete([FromRoute] Guid id)
    {
        var currentUser = await GetCurrentUser();

        if (currentUser.Result == null)
        {
            return ErrorMessageResult(currentUser.Error);
        }

        return FromServiceResponse(await deviceService.DeleteDevice(id, currentUser.Result));
    }
}