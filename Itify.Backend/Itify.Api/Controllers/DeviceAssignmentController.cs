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
public class DeviceAssignmentController(
    ILogger<DeviceAssignmentController> logger,
    IUserService userService,
    IDeviceAssignmentService deviceAssignmentService)
    : AuthorizedController(logger, userService)
{
    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RequestResponse<DeviceAssignmentRecord>>> GetById([FromRoute] Guid id)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Result == null) return ErrorMessageResult<DeviceAssignmentRecord>(currentUser.Error);

        return FromServiceResponse(await deviceAssignmentService.GetDeviceAssignment(id));
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<RequestResponse<PagedResponse<DeviceAssignmentRecord>>>> GetPage(
        [FromQuery] PaginationSearchQueryParams pagination)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Result == null)
            return ErrorMessageResult<PagedResponse<DeviceAssignmentRecord>>(currentUser.Error);

        return FromServiceResponse(await deviceAssignmentService.GetDeviceAssignments(pagination));
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<RequestResponse<PagedResponse<DeviceAssignmentRecord>>>> GetMyAssignments(
        [FromQuery] PaginationSearchQueryParams pagination)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Result == null)
            return ErrorMessageResult<PagedResponse<DeviceAssignmentRecord>>(currentUser.Error);

        return FromServiceResponse(
            await deviceAssignmentService.GetMyDeviceAssignments(currentUser.Result.Id, pagination));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<RequestResponse>> Add([FromBody] DeviceAssignmentAddRecord assignment)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Result == null) return ErrorMessageResult(currentUser.Error);

        return FromServiceResponse(await deviceAssignmentService.AddDeviceAssignment(assignment, currentUser.Result));
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<RequestResponse>> Return([FromRoute] Guid id)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Result == null) return ErrorMessageResult(currentUser.Error);

        return FromServiceResponse(
            await deviceAssignmentService.ReturnDeviceAssignment(id, currentUser.Result));
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<RequestResponse>> Delete([FromRoute] Guid id)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Result == null) return ErrorMessageResult(currentUser.Error);

        return FromServiceResponse(
            await deviceAssignmentService.DeleteDeviceAssignment(id, currentUser.Result));
    }
}