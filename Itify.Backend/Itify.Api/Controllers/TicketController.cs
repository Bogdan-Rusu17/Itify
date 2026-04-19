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
public class TicketController(
    ILogger<TicketController> logger,
    IUserService userService,
    ITicketService ticketService)
    : AuthorizedController(logger, userService)
{
    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RequestResponse<TicketRecord>>> GetById([FromRoute] Guid id)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Result == null) return ErrorMessageResult<TicketRecord>(currentUser.Error);

        return FromServiceResponse(await ticketService.GetTicket(id, currentUser.Result));
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<RequestResponse<PagedResponse<TicketRecord>>>> GetPage(
        [FromQuery] PaginationSearchQueryParams pagination)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Result == null) return ErrorMessageResult<PagedResponse<TicketRecord>>(currentUser.Error);

        return FromServiceResponse(await ticketService.GetTickets(pagination, currentUser.Result));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<RequestResponse>> Add([FromBody] TicketAddRecord ticket)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Result == null) return ErrorMessageResult(currentUser.Error);

        return FromServiceResponse(await ticketService.AddTicket(ticket, currentUser.Result));
    }

    [Authorize]
    [HttpPut]
    public async Task<ActionResult<RequestResponse>> Update([FromBody] TicketUpdateRecord ticket)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Result == null) return ErrorMessageResult(currentUser.Error);

        return FromServiceResponse(await ticketService.UpdateTicket(ticket, currentUser.Result));
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<RequestResponse>> Delete([FromRoute] Guid id)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Result == null) return ErrorMessageResult(currentUser.Error);

        return FromServiceResponse(await ticketService.DeleteTicket(id, currentUser.Result));
    }
}
