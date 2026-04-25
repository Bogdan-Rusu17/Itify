using Itify.AuthService.DataTransferObjects;
using Itify.AuthService.Enums;
using Itify.AuthService.HttpClients;
using Itify.AuthService.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Itify.AuthService.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(IDbServiceClient dbClient, JwtService jwtService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await dbClient.GetUserWithPasswordAsync(request.Email);
        if (user is null)
            return NotFound(new { Message = "User not found!", Code = ErrorCodes.EntityNotFound });
        if (user.Password != PasswordUtils.HashPassword(request.Password))
            return BadRequest(new { Message = "Wrong password!", Code = ErrorCodes.WrongPassword });
        return Ok(new LoginResponse(jwtService.GenerateToken(user), user.Id, user.Name, user.Email, user.Role));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (await dbClient.UserExistsAsync(request.Email))
            return Conflict(new { Message = "A user with this email already exists!", Code = ErrorCodes.UserAlreadyExists });
        await dbClient.CreateUserAsync(new CreateUserRequest(
            request.Name,
            request.Email,
            PasswordUtils.HashPassword(request.Password),
            UserRoleEnum.Employee));
        return Created(string.Empty, null);
    }
}
