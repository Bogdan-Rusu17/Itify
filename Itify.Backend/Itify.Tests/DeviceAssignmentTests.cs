using FluentAssertions;
using Itify.Database.Repository;
using Itify.Database.Repository.Entities;
using Itify.Database.Repository.Enums;
using Itify.Infrastructure.Configurations;
using Itify.Infrastructure.Errors;
using Itify.Infrastructure.Repositories.Interfaces;
using Itify.Services.Abstractions;
using Itify.Services.DataTransferObjects;
using Itify.Services.Implementations;
using Itify.Services.Specifications;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Itify.Tests;

public class DeviceAssignmentTests
{
    private readonly IMailService _mailService;
    private readonly IRepository<WebAppDatabaseContext> _repository;
    private readonly IDeviceAssignmentService _sut;

    public DeviceAssignmentTests()
    {
        _repository = Substitute.For<IRepository<WebAppDatabaseContext>>();
        _mailService = Substitute.For<IMailService>();
        var mailConfig = Options.Create(new MailConfiguration { FrontendUrl = "http://localhost:3000" });

        _sut = new DeviceAssignmentService(_repository, _mailService, mailConfig);
    }

    [Fact]
    public async Task AddDeviceAssignment_WhenUserIsNotAdminOrItEngineer_ReturnsError()
    {
        var result = await _sut.AddDeviceAssignment(
            new DeviceAssignmentAddRecord { DeviceId = Guid.NewGuid(), UserId = Guid.NewGuid() },
            new UserRecord { Role = UserRoleEnum.Employee });

        result.Error?.Message.Should().Be(CommonErrors.DeviceAssignmentUnauthorized.Message);
    }

    [Fact]
    public async Task AddDeviceAssignment_WhenDeviceNotAvailable_ReturnsError()
    {
        _repository.GetAsync(Arg.Any<DeviceSpec>(), Arg.Any<CancellationToken>())
            .Returns((Device?)null);

        var result = await _sut.AddDeviceAssignment(
            new DeviceAssignmentAddRecord { DeviceId = Guid.NewGuid(), UserId = Guid.NewGuid() },
            new UserRecord { Role = UserRoleEnum.Admin });

        result.Error?.Message.Should().Be(CommonErrors.DeviceNotFound.Message);
    }

    [Fact]
    public async Task AddDeviceAssignment_WhenDeviceAlreadyAssigned_ReturnsError()
    {
        _repository.GetAsync(Arg.Any<DeviceSpec>(), Arg.Any<CancellationToken>())
            .Returns(new Device { Status = DeviceStatusEnum.Assigned });

        var result = await _sut.AddDeviceAssignment(
            new DeviceAssignmentAddRecord { DeviceId = Guid.NewGuid(), UserId = Guid.NewGuid() },
            new UserRecord { Role = UserRoleEnum.Admin });

        result.Error?.Message.Should().Be(CommonErrors.DeviceNotAvailable.Message);
    }

    [Fact]
    public async Task AddDeviceAssignment_WhenValid_SendsMailToEmployee()
    {
        var userId = Guid.NewGuid();
        var device = new Device
        {
            Id = Guid.NewGuid(),
            Name = "Dell XPS",
            SerialNumber = "SN-001",
            Status = DeviceStatusEnum.Available
        };
        var employee = new User { Id = userId, Email = "emp@example.com", Name = "Employee" };

        _repository.GetAsync(Arg.Any<DeviceSpec>(), Arg.Any<CancellationToken>())
            .Returns(device);

        _repository.GetAsync(Arg.Any<DeviceAssignmentSpec>(), Arg.Any<CancellationToken>())
            .Returns((DeviceAssignment?)null);

        _repository.GetAsync(Arg.Any<UserSpec>(), Arg.Any<CancellationToken>())
            .Returns(employee);

        var result = await _sut.AddDeviceAssignment(
            new DeviceAssignmentAddRecord { DeviceId = device.Id, UserId = userId },
            new UserRecord { Role = UserRoleEnum.Admin });

        result.IsOk.Should().BeTrue();
        await _mailService.Received(1).SendMail(
            employee.Email,
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<bool>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ReturnDeviceAssignment_WhenValid_SetsDeviceStatusToAvailable()
    {
        var device = new Device { Id = Guid.NewGuid(), Status = DeviceStatusEnum.Assigned };

        _repository.GetAsync(Arg.Any<DeviceAssignmentSpec>(), Arg.Any<CancellationToken>())
            .Returns(new DeviceAssignment {Id = Guid.NewGuid(), DeviceId = device.Id, ReturnedAt = null});

        _repository.GetAsync(Arg.Any<DeviceSpec>(), Arg.Any<CancellationToken>())
            .Returns(device);

        var result = await _sut.ReturnDeviceAssignment(Guid.NewGuid(), new UserRecord { Role = UserRoleEnum.Admin });

        result.IsOk.Should().BeTrue();
        device.Status.Should().Be(DeviceStatusEnum.Available);
    }

    [Fact]
    public async Task DeleteDeviceAssignment_WhenNotReturned_SetsDeviceStatusToAvailable()
    {
        var device = new Device { Id = Guid.NewGuid(), Status = DeviceStatusEnum.Assigned };

        _repository.GetAsync(Arg.Any<DeviceAssignmentSpec>(), Arg.Any<CancellationToken>())
            .Returns(new DeviceAssignment { Id = Guid.NewGuid(), DeviceId = device.Id, ReturnedAt = null });

        _repository.GetAsync(Arg.Any<DeviceSpec>(), Arg.Any<CancellationToken>())
            .Returns(device);

        var result = await _sut.DeleteDeviceAssignment(Guid.NewGuid(), new UserRecord { Role = UserRoleEnum.Admin });

        result.IsOk.Should().BeTrue();
        device.Status.Should().Be(DeviceStatusEnum.Available);
    }
}