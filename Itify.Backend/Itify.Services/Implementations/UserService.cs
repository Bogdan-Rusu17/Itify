using Itify.Database.Repository;
using Itify.Database.Repository.Entities;
using Itify.Database.Repository.Enums;
using Itify.Infrastructure.Errors;
using Itify.Infrastructure.Repositories.Interfaces;
using Itify.Infrastructure.Responses;
using Itify.Services.Abstractions;
using Itify.Services.Constants;
using Itify.Services.DataTransferObjects;
using Itify.Services.Requests;
using Itify.Services.Specifications;

namespace Itify.Services.Implementations;

/// <summary>
///     Inject the required services through the constructor.
/// </summary>
public class UserService(
    IRepository<WebAppDatabaseContext> repository,
    ILoginService loginService,
    IMailService mailService)
    : IUserService
{
    public async Task<ServiceResponse<UserRecord>> GetUser(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await repository.GetAsync(new UserProjectionSpec(id), cancellationToken);

        if (result == null) return ServiceResponse.FromError<UserRecord>(CommonErrors.UserNotFound);

        return ServiceResponse.ForSuccess(result);
    }

    public async Task<ServiceResponse<UserRecord>> GetUser(Guid id, UserRecord requestingUser,
        CancellationToken cancellationToken = default)
    {
        if (requestingUser.Role == UserRoleEnum.Employee && requestingUser.Id != id)
            return ServiceResponse.FromError<UserRecord>(CommonErrors.UserNotFound);

        var result = await repository.GetAsync(new UserProjectionSpec(id), cancellationToken);

        if (result == null) return ServiceResponse.FromError<UserRecord>(CommonErrors.UserNotFound);

        return ServiceResponse.ForSuccess(result);
    }

    public async Task<ServiceResponse> Register(UserAddRecord user, CancellationToken cancellationToken = default)
    {
        if (user.Role != UserRoleEnum.Employee) return ServiceResponse.FromError(CommonErrors.RegisterRoleNotAllowed);

        var existing = await repository.GetAsync(new UserSpec(user.Email), cancellationToken);

        if (existing != null) return ServiceResponse.FromError(CommonErrors.UserAlreadyExists);

        await repository.AddAsync(new User
        {
            Email = user.Email,
            Name = user.Name,
            Role = UserRoleEnum.Employee,
            Password = user.Password
        }, cancellationToken);

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse<PagedResponse<UserRecord>>> GetUsers(UserPaginationQueryParams pagination,
        CancellationToken cancellationToken = default)
    {
        UserProjectionSpec spec;

        if (pagination.Role.HasValue)
            spec = new UserProjectionSpec(pagination.Search, pagination.Role.Value);
        else
            spec = new UserProjectionSpec(pagination.Search);

        var result = await repository.PageAsync(pagination, spec, cancellationToken);

        return ServiceResponse.ForSuccess(result);
    }

    public async Task<ServiceResponse<LoginResponseRecord>> Login(LoginRecord login,
        CancellationToken cancellationToken = default)
    {
        var result = await repository.GetAsync(new UserSpec(login.Email), cancellationToken);

        if (result == null) // Verify if the user is found in the database.
            return ServiceResponse.FromError<LoginResponseRecord>(CommonErrors
                .UserNotFound); // Pack the proper error as the response.

        if (result.Password != login.Password)
            return ServiceResponse.FromError<LoginResponseRecord>(CommonErrors.WrongPassword);

        var user = new UserRecord
        {
            Id = result.Id,
            Email = result.Email,
            Name = result.Name,
            Role = result.Role
        };

        return ServiceResponse.ForSuccess(new LoginResponseRecord
        {
            User = user,
            Token = loginService.GetToken(user, DateTime.UtcNow,
                new TimeSpan(7, 0, 0, 0)) // Get a JWT for the user issued now and that expires in 7 days.
        });
    }

    public async Task<ServiceResponse<int>> GetUserCount(CancellationToken cancellationToken = default)
    {
        return ServiceResponse.ForSuccess(await repository.GetCountAsync<User>(cancellationToken));
        // Get the count of all user entities in the database.
    }

    public async Task<ServiceResponse> AddUser(UserAddRecord user, UserRecord? requestingUser,
        CancellationToken cancellationToken = default)
    {
        if (requestingUser != null && requestingUser.Role != UserRoleEnum.Admin)
            return ServiceResponse.FromError(CommonErrors.UserAddUnauthorized);

        var existing = await repository.GetAsync(new UserSpec(user.Email), cancellationToken);

        if (existing != null) return ServiceResponse.FromError(CommonErrors.UserAlreadyExists);

        await repository.AddAsync(new User
        {
            Email = user.Email,
            Name = user.Name,
            Role = user.Role,
            Password = user.Password
        }, cancellationToken);

        await mailService.SendMail(user.Email, "Welcome!", MailTemplates.UserAddTemplate(user.Name), true, "Itify",
            cancellationToken);

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> UpdateUser(UserUpdateRecord user, UserRecord? requestingUser,
        CancellationToken cancellationToken = default)
    {
        if (requestingUser != null && requestingUser.Role != UserRoleEnum.Admin && requestingUser.Id != user.Id)
            return ServiceResponse.FromError(CommonErrors.UserUpdateUnauthorized);

        var entity = await repository.GetAsync(new UserSpec(user.Id), cancellationToken);

        if (entity == null) return ServiceResponse.FromError(CommonErrors.UserNotFound);

        entity.Name = user.Name ?? entity.Name;
        entity.Password = user.Password ?? entity.Password;

        await repository.UpdateAsync(entity, cancellationToken);

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> DeleteUser(Guid id, UserRecord? requestingUser = null,
        CancellationToken cancellationToken = default)
    {
        if (requestingUser != null && requestingUser.Role != UserRoleEnum.Admin && requestingUser.Id != id)
            return ServiceResponse.FromError(CommonErrors.UserDeleteUnauthorized);

        var entity = await repository.GetAsync(new UserSpec(id), cancellationToken);

        if (entity == null) return ServiceResponse.FromError(CommonErrors.UserNotFound);

        await repository.DeleteAsync<User>(id, cancellationToken);

        return ServiceResponse.ForSuccess();
    }
}