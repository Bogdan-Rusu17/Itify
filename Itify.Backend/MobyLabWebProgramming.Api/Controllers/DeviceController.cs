using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobyLabWebProgramming.Infrastructure.Requests;
using MobyLabWebProgramming.Infrastructure.Responses;
using MobyLabWebProgramming.Services.Abstractions;
using MobyLabWebProgramming.Services.Authorization;
using MobyLabWebProgramming.Services.DataTransferObjects;

namespace MobyLabWebProgramming.Api.Controllers;
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

        return FromServiceResponse(await deviceService.GetDevice(id));
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

        return FromServiceResponse(await deviceService.GetDevices(pagination));
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