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

public class DeviceServiceTests
{
    private readonly IRepository<WebAppDatabaseContext> _repository;
    private readonly IDeviceService _sut;

    public DeviceServiceTests()
    {
        _repository = Substitute.For<IRepository<WebAppDatabaseContext>>();
        _sut = new DeviceService(_repository);
    }

    [Fact]
    public async Task AddDevice_WhenEmployeeRole_ReturnsError()
    {
        var requestingUser = new UserRecord { Id = Guid.NewGuid(), Role = UserRoleEnum.Employee };
        var request = new DeviceAddRecord
        {
            Name = "Laptop",
            SerialNumber = "SN123",
            Status = DeviceStatusEnum.Available,
            CategoryId = Guid.NewGuid()
        };

        var result = await _sut.AddDevice(request, requestingUser);

        result.Error?.Message.Should().Be(CommonErrors.UnauthorizedDeviceAddOrUpdate.Message);
    }

    [Fact]
    public async Task AddDevice_WhenCategoryNotFound_ReturnsError()
    {
        var requestingUser = new UserRecord { Id = Guid.NewGuid(), Role = UserRoleEnum.Admin };

        _repository.GetAsync(Arg.Any<DeviceCategorySpec>(), Arg.Any<CancellationToken>())
            .Returns((DeviceCategory?)null);

        var request = new DeviceAddRecord
        {
            Name = "Laptop",
            SerialNumber = "SN123",
            Status = DeviceStatusEnum.Available,
            CategoryId = Guid.NewGuid()
        };

        var result = await _sut.AddDevice(request, requestingUser);

        result.Error?.Message.Should().Be(CommonErrors.DeviceCategoryNotFound.Message);
    }

    [Fact]
    public async Task AddDevice_WhenSerialNumberAlreadyExists_ReturnsError()
    {
        var requestingUser = new UserRecord { Id = Guid.NewGuid(), Role = UserRoleEnum.Admin };

        _repository.GetAsync(Arg.Any<DeviceCategorySpec>(), Arg.Any<CancellationToken>())
            .Returns(new DeviceCategory { Name = "Laptop" });

        _repository.GetAsync(Arg.Any<DeviceSpec>(), Arg.Any<CancellationToken>())
            .Returns(new Device { SerialNumber = "SN123" });

        var request = new DeviceAddRecord
        {
            Name = "Laptop",
            SerialNumber = "SN123",
            Status = DeviceStatusEnum.Available,
            CategoryId = Guid.NewGuid()
        };

        var result = await _sut.AddDevice(request, requestingUser);

        result.Error?.Message.Should().Be(CommonErrors.DeviceAlreadyExists.Message);
    }

    [Fact]
    public async Task UpdateDevice_WhenDeviceNotFound_ReturnsError()
    {
        var requestingUser = new UserRecord { Id = Guid.NewGuid(), Role = UserRoleEnum.Admin };

        _repository.GetAsync(Arg.Any<DeviceSpec>(), Arg.Any<CancellationToken>())
            .Returns((Device?)null);

        var request = new DeviceUpdateRecord { Id = Guid.NewGuid(), Name = "Updated Laptop" };

        var result = await _sut.UpdateDevice(request, requestingUser);

        result.Error?.Message.Should().Be(CommonErrors.DeviceNotFound.Message);
    }
}
