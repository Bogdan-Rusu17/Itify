using Itify.AuthService.DataTransferObjects;
using Itify.AuthService.Enums;
using Itify.AuthService.HttpClients;
using Itify.AuthService.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Itify.AuthService.Controllers;

[ApiController]
[Route("")]
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
        return Ok(new LoginResponse { Token = jwtService.GenerateToken(user), UserId = user.Id, Name = user.Name, Email = user.Email, Role = user.Role });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (await dbClient.UserExistsAsync(request.Email))
            return Conflict(new { Message = "A user with this email already exists!", Code = ErrorCodes.UserAlreadyExists });
        await dbClient.CreateUserAsync(new CreateUserRequest
        {
            Name = request.Name,
            Email = request.Email,
            Password = PasswordUtils.HashPassword(request.Password),
            Role = UserRoleEnum.Employee
        });
        return Created(string.Empty, null);
    }
}
