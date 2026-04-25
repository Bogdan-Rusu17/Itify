namespace Itify.AuthService.DataTransferObjects;

public record RegisterRequest(string Name, string Email, string Password);
