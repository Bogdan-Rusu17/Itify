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

public class DeviceCategoryServiceTests
{
    private readonly IRepository<WebAppDatabaseContext> _repository;
    private readonly IDeviceCategoryService _sut;

    public DeviceCategoryServiceTests()
    {
        _repository = Substitute.For<IRepository<WebAppDatabaseContext>>();
        _sut = new DeviceCategoryService(_repository);
    }

    [Fact]
    public async Task AddDeviceCategory_WhenEmployeeRole_ReturnsError()
    {
        var requestingUser = new UserRecord { Id = Guid.NewGuid(), Role = UserRoleEnum.Employee };
        var request = new DeviceCategoryAddRecord { Name = "Laptops" };

        var result = await _sut.AddDeviceCategory(request, requestingUser);

        result.Error?.Message.Should().Be(CommonErrors.DeviceCategoryUnauthorized.Message);
    }

    [Fact]
    public async Task AddDeviceCategory_WhenNameAlreadyExists_ReturnsError()
    {
        var requestingUser = new UserRecord { Id = Guid.NewGuid(), Role = UserRoleEnum.Admin };

        _repository.GetAsync(Arg.Any<DeviceCategorySpec>(), Arg.Any<CancellationToken>())
            .Returns(new DeviceCategory { Name = "Laptops" });

        var request = new DeviceCategoryAddRecord { Name = "Laptops" };

        var result = await _sut.AddDeviceCategory(request, requestingUser);

        result.Error?.Message.Should().Be(CommonErrors.DeviceCategoryAlreadyExists.Message);
    }

    [Fact]
    public async Task DeleteDeviceCategory_WhenEmployeeRole_ReturnsError()
    {
        var requestingUser = new UserRecord { Id = Guid.NewGuid(), Role = UserRoleEnum.Employee };

        var result = await _sut.DeleteDeviceCategory(Guid.NewGuid(), requestingUser);

        result.Error?.Message.Should().Be(CommonErrors.DeviceCategoryUnauthorized.Message);
    }

    [Fact]
    public async Task DeleteDeviceCategory_WhenCategoryHasDevices_ReturnsError()
    {
        var requestingUser = new UserRecord { Id = Guid.NewGuid(), Role = UserRoleEnum.Admin };

        _repository.GetCountAsync(Arg.Any<DeviceSpec>(), Arg.Any<CancellationToken>())
            .Returns(3);

        var result = await _sut.DeleteDeviceCategory(Guid.NewGuid(), requestingUser);

        result.Error?.Message.Should().Be(CommonErrors.DeviceCategoryHasDevices.Message);
    }
}
