using Itify.BusinessService.DataTransferObjects;
using Itify.BusinessService.Enums;
using Itify.BusinessService.HttpClients;
using Itify.BusinessService.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Itify.BusinessService.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UserController(IDbServiceClient db, MailService mail) : AuthorizedController(db)
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Role == UserRoleEnum.Employee && id != currentUser.Id) return Forbid();
        var user = await Db.GetUserAsync(id);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] UserRoleEnum? role = null)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Role == UserRoleEnum.Employee) return Forbid();
        return Ok(await Db.GetUsersAsync(page, pageSize, search, role));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserAddRequest dto)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Role != UserRoleEnum.Admin) return Forbid();
        dto.Password = PasswordUtils.HashPassword(dto.Password);
        await Db.CreateUserAsync(dto);
        await mail.SendAsync(dto.Email, "Welcome to Itify!",
            $"<p>Dear {dto.Name},</p><p>Your account has been created. You can now log in to Itify.</p>");
        return Created(string.Empty, null);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateRequest dto)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Role == UserRoleEnum.Employee && id != currentUser.Id) return Forbid();
        if (currentUser.Role == UserRoleEnum.Employee && dto.Role is not null) return Forbid();

        var user = await Db.GetUserAsync(id);
        if (user is null) return NotFound();

        if (!string.IsNullOrWhiteSpace(dto.Password))
            dto.Password = PasswordUtils.HashPassword(dto.Password);
        else
            dto.Password = null;

        await Db.UpdateUserAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var currentUser = await GetCurrentUser();
        if (currentUser.Role == UserRoleEnum.Employee) return Forbid();
        var user = await Db.GetUserAsync(id);
        if (user is null) return NotFound();
        await Db.DeleteUserAsync(id);
        return NoContent();
    }
}
