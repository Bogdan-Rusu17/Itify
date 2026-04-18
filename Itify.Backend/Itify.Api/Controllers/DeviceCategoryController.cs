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
public class DeviceCategoryController(
    ILogger<DeviceCategoryController> logger,
    IUserService userService,
    IDeviceCategoryService deviceCategoryService)
    : AuthorizedController(logger, userService)
{
    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RequestResponse<DeviceCategoryRecord>>> GetById([FromRoute] Guid id)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Result == null) return ErrorMessageResult<DeviceCategoryRecord>(currentUser.Error);

        return FromServiceResponse(await deviceCategoryService.GetDeviceCategory(id));
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<RequestResponse<PagedResponse<DeviceCategoryRecord>>>> GetPage(
        [FromQuery] PaginationSearchQueryParams pagination)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Result == null)
            return ErrorMessageResult<PagedResponse<DeviceCategoryRecord>>(currentUser.Error);

        return FromServiceResponse(await deviceCategoryService.GetDeviceCategories(pagination));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<RequestResponse>> Add([FromBody] DeviceCategoryAddRecord category)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Result == null) return ErrorMessageResult(currentUser.Error);

        return FromServiceResponse(await deviceCategoryService.AddDeviceCategory(category, currentUser.Result));
    }

    [Authorize]
    [HttpPut]
    public async Task<ActionResult<RequestResponse>> Update([FromBody] DeviceCategoryUpdateRecord category)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Result == null) return ErrorMessageResult(currentUser.Error);

        return FromServiceResponse(await deviceCategoryService.UpdateDeviceCategory(category, currentUser.Result));
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<RequestResponse>> Delete([FromRoute] Guid id)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Result == null) return ErrorMessageResult(currentUser.Error);

        return FromServiceResponse(await deviceCategoryService.DeleteDeviceCategory(id, currentUser.Result));
    }
}