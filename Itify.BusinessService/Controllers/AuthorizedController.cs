using System.Security.Claims;
using Itify.BusinessService.DataTransferObjects;
using Itify.BusinessService.HttpClients;
using Itify.BusinessService.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Itify.BusinessService.Controllers;

public abstract class AuthorizedController(IDbServiceClient db) : ControllerBase
{
    protected readonly IDbServiceClient Db = db;
    private UserClaims? _userClaims;
    private UserRecord? _currentUser;

    protected UserClaims ExtractClaims()
    {
        if (_userClaims is not null)
            return _userClaims;

        var claims = User.Claims.ToList();
        _userClaims = new UserClaims
        {
            Id = claims.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(x => Guid.Parse(x.Value)).FirstOrDefault(),
            Name = claims.Where(x => x.Type == ClaimTypes.Name).Select(x => x.Value).FirstOrDefault()!,
            Email = claims.Where(x => x.Type == ClaimTypes.Email).Select(x => x.Value).FirstOrDefault()!
        };
        return _userClaims;
    }

    protected async Task<UserRecord> GetCurrentUser()
    {
        if (_currentUser is not null)
            return _currentUser;

        _currentUser = await Db.GetUserAsync(ExtractClaims().Id)
            ?? throw new NotFoundException("Authenticated user not found.");

        return _currentUser;
    }
}
