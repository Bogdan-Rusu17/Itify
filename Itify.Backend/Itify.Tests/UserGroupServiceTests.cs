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

public class UserGroupServiceTests
{
    private readonly IRepository<WebAppDatabaseContext> _repository;
    private readonly IUserGroupService _sut;

    public UserGroupServiceTests()
    {
        _repository = Substitute.For<IRepository<WebAppDatabaseContext>>();
        _sut = new UserGroupService(_repository);
    }

    [Fact]
    public async Task AddUserGroup_WhenNotAdmin_ReturnsError()
    {
        var requestingUser = new UserRecord { Id = Guid.NewGuid(), Role = UserRoleEnum.Employee };
        var request = new UserGroupAddRecord { Name = "IT Team" };

        var result = await _sut.AddUserGroup(request, requestingUser);

        result.Error?.Message.Should().Be(CommonErrors.UserGroupUnauthorized.Message);
    }

    [Fact]
    public async Task AddUserGroup_WhenNameAlreadyExists_ReturnsError()
    {
        var requestingUser = new UserRecord { Id = Guid.NewGuid(), Role = UserRoleEnum.Admin };

        _repository.GetAsync(Arg.Any<UserGroupSpec>(), Arg.Any<CancellationToken>())
            .Returns(new UserGroup { Name = "IT Team" });

        var request = new UserGroupAddRecord { Name = "IT Team" };

        var result = await _sut.AddUserGroup(request, requestingUser);

        result.Error?.Message.Should().Be(CommonErrors.UserGroupAlreadyExists.Message);
    }

    [Fact]
    public async Task UpdateUserGroup_WhenNotFound_ReturnsError()
    {
        var requestingUser = new UserRecord { Id = Guid.NewGuid(), Role = UserRoleEnum.Admin };

        _repository.GetAsync(Arg.Any<UserGroupSpec>(), Arg.Any<CancellationToken>())
            .Returns((UserGroup?)null);

        var request = new UserGroupUpdateRecord { Id = Guid.NewGuid(), Name = "Updated Name" };

        var result = await _sut.UpdateUserGroup(request, requestingUser);

        result.Error?.Message.Should().Be(CommonErrors.UserGroupNotFound.Message);
    }

    [Fact]
    public async Task DeleteUserGroup_WhenNotAdmin_ReturnsError()
    {
        var requestingUser = new UserRecord { Id = Guid.NewGuid(), Role = UserRoleEnum.Employee };

        var result = await _sut.DeleteUserGroup(Guid.NewGuid(), requestingUser);

        result.Error?.Message.Should().Be(CommonErrors.UserGroupUnauthorized.Message);
    }
}
