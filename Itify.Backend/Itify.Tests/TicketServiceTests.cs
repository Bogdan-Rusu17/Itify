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

public class TicketServiceTests
{
    private readonly IMailService _mailService;
    private readonly IRepository<WebAppDatabaseContext> _repository;
    private readonly TicketService _sut;

    public TicketServiceTests()
    {
        _repository = Substitute.For<IRepository<WebAppDatabaseContext>>();
        _mailService = Substitute.For<IMailService>();
        var mailConfig = Options.Create(new MailConfiguration { FrontendUrl = "http://localhost:3000" });

        _sut = new TicketService(_repository, _mailService, mailConfig);
    }

    [Fact]
    public async Task AddTicket_WhenAssignmentNotFound_ReturnsError()
    {
        _repository.GetAsync(Arg.Any<DeviceAssignmentSpec>(), Arg.Any<CancellationToken>())
            .Returns((DeviceAssignment?)null);

        var result = await _sut.AddTicket(new TicketAddRecord { DeviceAssignmentId = Guid.NewGuid() },
            new UserRecord { Role = UserRoleEnum.Employee });

        result.Error?.Message.Should().Be(CommonErrors.DeviceAssignmentNotFound.Message);
    }

    [Fact]
    public async Task AddTicket_WhenAssignmentDoesNotBelongToUser_ReturnsError()
    {
        var userId = Guid.NewGuid();
        _repository.GetAsync(Arg.Any<DeviceAssignmentSpec>(), Arg.Any<CancellationToken>())
            .Returns(new DeviceAssignment { UserId = Guid.NewGuid() });

        var result = await _sut.AddTicket(new TicketAddRecord
                { DeviceAssignmentId = Guid.NewGuid() },
            new UserRecord { Id = userId, Role = UserRoleEnum.Employee });

        result.Error?.Message.Should().Be(CommonErrors.DeviceAssignmentUnauthorized.Message);
    }

    [Fact]
    public async Task AddTicket_WhenUnresolvedTicketAlreadyExists_ReturnsError()
    {
        var userId = Guid.NewGuid();
        var assignmentId = Guid.NewGuid();

        _repository.GetAsync(Arg.Any<DeviceAssignmentSpec>(), Arg.Any<CancellationToken>())
            .Returns(new DeviceAssignment { UserId = userId });

        _repository.GetAsync(Arg.Any<TicketSpec>(), Arg.Any<CancellationToken>())
            .Returns(new Ticket { Status = TicketStatusEnum.Open });

        var result = await _sut.AddTicket(
            new TicketAddRecord { DeviceAssignmentId = assignmentId, Type = TicketTypeEnum.Feedback },
            new UserRecord { Id = userId, Role = UserRoleEnum.Employee });

        result.Error?.Message.Should().Be(CommonErrors.TicketAlreadyExists.Message);
    }

    [Fact]
    public async Task AddTicket_WhenRepairRequestAndDeviceAlreadyInRepair_ReturnsError()
    {
        var userId = Guid.NewGuid();

        _repository.GetAsync(Arg.Any<DeviceAssignmentSpec>(), Arg.Any<CancellationToken>())
            .Returns(new DeviceAssignment { UserId = userId, DeviceId = Guid.NewGuid() });

        _repository.GetAsync(Arg.Any<TicketSpec>(), Arg.Any<CancellationToken>())
            .Returns((Ticket?)null);

        _repository.GetAsync(Arg.Any<DeviceSpec>(), Arg.Any<CancellationToken>())
            .Returns(new Device { Status = DeviceStatusEnum.InRepair });

        var result = await _sut.AddTicket(
            new TicketAddRecord { DeviceAssignmentId = Guid.NewGuid(), Type = TicketTypeEnum.RepairRequest },
            new UserRecord { Id = userId, Role = UserRoleEnum.Employee });

        result.Error?.Message.Should().Be(CommonErrors.DeviceAlreadyInRepair.Message);
    }
    
    [Fact]
    public async Task AddTicket_WhenResolvedTicketExists_DeletesOldAndCreatesNew()
    {
        var userId = Guid.NewGuid();
        var existingTicketId = Guid.NewGuid();

        _repository.GetAsync(Arg.Any<DeviceAssignmentSpec>(), Arg.Any<CancellationToken>())
            .Returns(new DeviceAssignment { UserId = userId });

        _repository.GetAsync(Arg.Any<TicketSpec>(), Arg.Any<CancellationToken>())
            .Returns(new Ticket { Id = existingTicketId, Status = TicketStatusEnum.Resolved });

        _repository.ListAsync(Arg.Any<UserSpec>(), Arg.Any<CancellationToken>())
            .Returns(new List<User>());

        var result = await _sut.AddTicket(
            new TicketAddRecord { DeviceAssignmentId = Guid.NewGuid(), Type = TicketTypeEnum.Feedback },
            new UserRecord { Id = userId, Role = UserRoleEnum.Employee });

        result.IsOk.Should().BeTrue();
        await _repository.Received(1).DeleteAsync<Ticket>(existingTicketId, Arg.Any<CancellationToken>());
        await _repository.Received(1).AddAsync(Arg.Any<Ticket>(), Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task UpdateTicket_WhenUserIsNotAdminOrItEngineer_ReturnsError()
    {
        var result = await _sut.UpdateTicket(
            new TicketUpdateRecord { Id = Guid.NewGuid(), Status = TicketStatusEnum.InProgress },
            new UserRecord { Role = UserRoleEnum.Employee });

        result.Error?.Message.Should().Be(CommonErrors.TicketUnauthorizedUpdate.Message);
    }
    
    [Fact]
    public async Task UpdateTicket_WhenTicketNotFound_ReturnsError()
    {
        _repository.GetAsync(Arg.Any<TicketSpec>(), Arg.Any<CancellationToken>())
            .Returns((Ticket?)null);

        var result = await _sut.UpdateTicket(
            new TicketUpdateRecord { Id = Guid.NewGuid(), Status = TicketStatusEnum.InProgress },
            new UserRecord { Role = UserRoleEnum.Admin });

        result.Error?.Message.Should().Be(CommonErrors.TicketNotFound.Message);
    }
    
    [Fact]
    public async Task UpdateTicket_WhenAlreadyResolved_ReturnsError()
    {
        _repository.GetAsync(Arg.Any<TicketSpec>(), Arg.Any<CancellationToken>())
            .Returns(new Ticket { Status = TicketStatusEnum.Resolved });

        var result = await _sut.UpdateTicket(
            new TicketUpdateRecord { Id = Guid.NewGuid(), Status = TicketStatusEnum.InProgress },
            new UserRecord { Role = UserRoleEnum.Admin });

        result.Error?.Message.Should().Be(CommonErrors.TicketAlreadyResolved.Message);
    }
    
    [Fact]
    public async Task UpdateTicket_WhenResolvedRepairRequest_SetsDeviceStatusToAssigned()
    {
        var deviceId = Guid.NewGuid();
        var device = new Device { Id = deviceId, Status = DeviceStatusEnum.InRepair };

        _repository.GetAsync(Arg.Any<TicketSpec>(), Arg.Any<CancellationToken>())
            .Returns(new Ticket { Status = TicketStatusEnum.Open, Type = TicketTypeEnum.RepairRequest });

        _repository.GetAsync(Arg.Any<DeviceAssignmentSpec>(), Arg.Any<CancellationToken>())
            .Returns(new DeviceAssignment { DeviceId = deviceId });

        _repository.GetAsync(Arg.Any<DeviceSpec>(), Arg.Any<CancellationToken>())
            .Returns(device);

        _repository.GetAsync(Arg.Any<UserSpec>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var result = await _sut.UpdateTicket(
            new TicketUpdateRecord { Id = Guid.NewGuid(), Status = TicketStatusEnum.Resolved },
            new UserRecord { Role = UserRoleEnum.Admin });

        result.IsOk.Should().BeTrue();
        device.Status.Should().Be(DeviceStatusEnum.Assigned);
    }
    
    [Fact]
    public async Task DeleteTicket_WhenTicketNotFound_ReturnsError()
    {
        _repository.GetAsync(Arg.Any<TicketSpec>(), Arg.Any<CancellationToken>())
            .Returns((Ticket?)null);
        
        var result = await _sut.DeleteTicket(Guid.NewGuid(),
            new UserRecord { Role = UserRoleEnum.Employee });

        result.Error?.Message.Should().Be(CommonErrors.TicketNotFound.Message);
    }
    
    [Fact]
    public async Task DeleteTicket_WhenUserIsNotAdminOrItEngineer_ReturnsError ()
    {
        _repository.GetAsync(Arg.Any<TicketSpec>(), Arg.Any<CancellationToken>())
            .Returns(new Ticket { Status = TicketStatusEnum.InProgress });
        
        var result = await _sut.DeleteTicket(Guid.NewGuid(),
            new UserRecord { Role = UserRoleEnum.Employee });

        result.Error?.Message.Should().Be(CommonErrors.TicketUnauthorizedDelete.Message);
    }
    
    [Fact]
    public async Task DeleteTicket_ShouldSetDeviceStatusToAssignedIfRepairTicket ()
    {
        var deviceId = Guid.NewGuid();
        var deviceAssignmentId = Guid.NewGuid();
        var device = new Device { Id = deviceId, Status = DeviceStatusEnum.InRepair };
        
        _repository.GetAsync(Arg.Any<TicketSpec>(), Arg.Any<CancellationToken>())
            .Returns(new Ticket { Status = TicketStatusEnum.InProgress, Type = TicketTypeEnum.RepairRequest });
        _repository.GetAsync(Arg.Any<DeviceAssignmentSpec>(), Arg.Any<CancellationToken>())
            .Returns(new DeviceAssignment { DeviceId = deviceId, Id = deviceAssignmentId });
        _repository.GetAsync(Arg.Any<DeviceSpec>(), Arg.Any<CancellationToken>())
            .Returns(device);
        
        var result = await _sut.DeleteTicket(Guid.NewGuid(),
            new UserRecord { Role = UserRoleEnum.ItEngineer });
        
        result.IsOk.Should().BeTrue();
        device.Status.Should().Be(DeviceStatusEnum.Assigned);
    }
}