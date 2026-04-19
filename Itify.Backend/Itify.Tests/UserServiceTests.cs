using FluentAssertions;
using Itify.Database.Repository;
using Itify.Database.Repository.Entities;
using Itify.Database.Repository.Enums;
using Itify.Infrastructure.Errors;
using Itify.Infrastructure.Repositories.Interfaces;
using Itify.Services.Abstractions;
using Itify.Services.DataTransferObjects;
using Itify.Services.Implementations;
using Itify.Services.Specifications;
using NSubstitute;

namespace Itify.Tests;

public class UserServiceTests
{
    private readonly ILoginService _loginService;
    private readonly IMailService _mailService;
    private readonly IRepository<WebAppDatabaseContext> _repository;
    private readonly IUserService _sut;

    public UserServiceTests()
    {
        _repository = Substitute.For<IRepository<WebAppDatabaseContext>>();
        _loginService = Substitute.For<ILoginService>();
        _mailService = Substitute.For<IMailService>();

        _sut = new UserService(_repository, _loginService, _mailService);
    }

    [Fact]
    public async Task Register_WhenEmailAlreadyExists_ReturnsError()
    {
        var existingUser = new User
        {
            Email = "test@example.com"
        };

        _repository.GetAsync(Arg.Any<UserSpec>(),
                Arg.Any<CancellationToken>())
            .Returns(existingUser);

        var request = new UserAddRecord
        {
            Email = "test@example.com",
            Name = "Test",
            Password = "pass",
            Role = UserRoleEnum.Employee
        };

        var result = await _sut.Register(request);

        result.IsOk.Should().BeFalse();
        result.Error?.Message.Should().Be(CommonErrors.UserAlreadyExists.Message);
    }

    [Fact]
    public async Task Register_WhenRoleIsNotEmployee_ReturnsError()
    {
        var request = new UserAddRecord
        {
            Email = "admin@example.com",
            Name = "Admin",
            Password = "pass",
            Role = UserRoleEnum.Admin
        };

        var result = await _sut.Register(request);

        result.Error?.Message.Should().Be(CommonErrors.RegisterRoleNotAllowed.Message);
    }

    [Fact]
    public async Task Login_WhenUserNotFound_ReturnsError()
    {
        _repository.GetAsync(Arg.Any<UserSpec>(),
                Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var result = await _sut.Login(new LoginRecord("notfound@example.com", "pass"));

        result.Error?.Message.Should().Be(CommonErrors.UserNotFound.Message);
    }

    [Fact]
    public async Task Login_WhenPasswordIsWrong_ReturnsError()
    {
        _repository.GetAsync(Arg.Any<UserSpec>(),
                Arg.Any<CancellationToken>())
            .Returns(new User
            {
                Email = "test@example.com",
                Password = "correctpass"
            });

        var result = await _sut.Login(new LoginRecord("test@example.com", "wrongpass"));

        result.Error?.Message.Should().Be(CommonErrors.WrongPassword.Message);
    }

    [Fact]
    public async Task AddUser_WhenRequestingUserIsNotAdmin_ReturnsError()
    {
        var requestingUser = new UserRecord
        {
            Id = Guid.NewGuid(),
            Role = UserRoleEnum.Employee
        };
        var request = new UserAddRecord
        {
            Email = "new@example.com",
            Name = "New",
            Password = "pass",
            Role = UserRoleEnum.Employee
        };

        var result = await _sut.AddUser(request, requestingUser);

        result.Error?.Message.Should().Be(CommonErrors.UserAddUnauthorized.Message);
    }

    [Fact]
    public async Task AddUser_WhenEmailAlreadyExists_ReturnsError()
    {
        var requestingUser = new UserRecord
        {
            Id = Guid.NewGuid(),
            Role = UserRoleEnum.Admin
        };

        _repository.GetAsync(Arg.Any<UserSpec>(),
                Arg.Any<CancellationToken>())
            .Returns(new User { Email = "existing@example.com" });

        var request = new UserAddRecord
        {
            Email = "existing@example.com",
            Name = "Test",
            Password = "pass", 
            Role = UserRoleEnum.Employee
        };

        var result = await _sut.AddUser(request, requestingUser);

        result.Error?.Message.Should().Be(CommonErrors.UserAlreadyExists.Message);
    }
    
    [Fact]
    public async Task AddUser_WhenValid_SendsWelcomeMail()
    {
        var requestingUser = new UserRecord { Id = Guid.NewGuid(), Role = UserRoleEnum.Admin };

        _repository.GetAsync(Arg.Any<UserSpec>(),
                Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var request = new UserAddRecord { 
            Email = "new@example.com",
            Name = "New User", 
            Password = "pass", 
            Role = UserRoleEnum.Employee
        };

        var result = await _sut.AddUser(request, requestingUser);

        result.IsOk.Should().BeTrue();
        await _mailService.Received(1).SendMail(
            request.Email,
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<bool>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateUser_WhenEmployeeUpdatesOtherUser_ReturnsError()
    {
        var requestingUser = new UserRecord { Id = Guid.NewGuid(), Role = UserRoleEnum.Employee };
        var request = new UserUpdateRecord(Guid.NewGuid());

        var result = await _sut.UpdateUser(request, requestingUser);

        result.Error?.Message.Should().Be(CommonErrors.UserUpdateUnauthorized.Message);
    }
    
    [Fact]
    public async Task UpdateUser_WhenEmployeeTriesToChangeRole_ReturnsError()
    {
        var userId = Guid.NewGuid();
        var requestingUser = new UserRecord { Id = userId, Role = UserRoleEnum.Employee };

        _repository.GetAsync(Arg.Any<UserSpec>(),
                Arg.Any<CancellationToken>())
            .Returns(new User { Id = userId, Name = "Test", Password = "pass" });

        var request = new UserUpdateRecord(userId, Role: UserRoleEnum.Admin);

        var result = await _sut.UpdateUser(request, requestingUser);

        result.Error?.Message.Should().Be(CommonErrors.UserUpdateUnauthorized.Message);
    }
    
    [Fact]
    public async Task UpdateUser_WhenUserNotFound_ReturnsError()
    {
        var requestingUser = new UserRecord { Id = Guid.NewGuid(), Role = UserRoleEnum.Admin };

        _repository.GetAsync(Arg.Any<UserSpec>(),
                Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var request = new UserUpdateRecord(Guid.NewGuid());

        var result = await _sut.UpdateUser(request, requestingUser);

        result.Error?.Message.Should().Be(CommonErrors.UserNotFound.Message);
    }
    
    [Fact]
    public async Task DeleteUser_WhenNotAdminAndNotOwner_ReturnsError()
    {
        var requestingUser = new UserRecord { Id = Guid.NewGuid(), Role = UserRoleEnum.Employee };

        var result = await _sut.DeleteUser(Guid.NewGuid(), requestingUser);

        result.Error?.Message.Should().Be(CommonErrors.UserDeleteUnauthorized.Message);
    }
    
    [Fact]
    public async Task DeleteUser_WhenUserNotFound_ReturnsError()
    {
        var requestingUser = new UserRecord { Id = Guid.NewGuid(), Role = UserRoleEnum.Admin };

        _repository.GetAsync(Arg.Any<UserSpec>(),
                Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var result = await _sut.DeleteUser(Guid.NewGuid(), requestingUser);

        result.Error?.Message.Should().Be(CommonErrors.UserNotFound.Message);
    }

}