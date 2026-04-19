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

public class DeviceRequestServiceTests
{
    private readonly IRepository<WebAppDatabaseContext> _repository;
    private readonly IMailService _mailService;
    private readonly IDeviceRequestService _sut;

    public DeviceRequestServiceTests()
    {
        _repository = Substitute.For<IRepository<WebAppDatabaseContext>>();
        _mailService = Substitute.For<IMailService>();
        var mailConfig = Options.Create(new MailConfiguration());

        _sut = new DeviceRequestService(_repository, _mailService, mailConfig);
    }

    [Fact]
    public async Task AddDeviceRequest_WhenCategoryNotFound_ReturnsError()
    {
        _repository.GetAsync(Arg.Any<DeviceCategorySpec>(), Arg.Any<CancellationToken>())
            .Returns((DeviceCategory?)null);

        var requestingUser = new UserRecord { Id = Guid.NewGuid(), Role = UserRoleEnum.Employee };
        var request = new DeviceRequestAddRecord { Reason = "Need a laptop", CategoryId = Guid.NewGuid() };

        var result = await _sut.AddDeviceRequest(request, requestingUser);

        result.Error?.Message.Should().Be(CommonErrors.DeviceCategoryNotFound.Message);
    }

    [Fact]
    public async Task AddDeviceRequest_WhenPendingRequestAlreadyExists_ReturnsError()
    {
        _repository.GetAsync(Arg.Any<DeviceCategorySpec>(), Arg.Any<CancellationToken>())
            .Returns(new DeviceCategory { Name = "Laptop" });

        _repository.GetAsync(Arg.Any<DeviceRequestSpec>(), Arg.Any<CancellationToken>())
            .Returns(new DeviceRequest { Status = RequestStatusEnum.Pending });

        var requestingUser = new UserRecord { Id = Guid.NewGuid(), Role = UserRoleEnum.Employee };
        var request = new DeviceRequestAddRecord { Reason = "Need a laptop", CategoryId = Guid.NewGuid() };

        var result = await _sut.AddDeviceRequest(request, requestingUser);

        result.Error?.Message.Should().Be(CommonErrors.UnauthorizedDeviceRequestAlreadyOnCategory.Message);
    }

    [Fact]
    public async Task UpdateDeviceRequest_WhenNotFound_ReturnsError()
    {
        _repository.GetAsync(Arg.Any<DeviceRequestSpec>(), Arg.Any<CancellationToken>())
            .Returns((DeviceRequest?)null);

        var requestingUser = new UserRecord { Id = Guid.NewGuid(), Role = UserRoleEnum.Admin };
        var request = new DeviceRequestUpdateRecord { Id = Guid.NewGuid(), Status = RequestStatusEnum.Approved };

        var result = await _sut.UpdateDeviceRequest(request, requestingUser);

        result.Error?.Message.Should().Be(CommonErrors.DeviceRequestNotFound.Message);
    }

    [Fact]
    public async Task DeleteDeviceRequest_WhenEmployeeDeletesOthersRequest_ReturnsError()
    {
        var ownerId = Guid.NewGuid();
        var requestingUser = new UserRecord { Id = Guid.NewGuid(), Role = UserRoleEnum.Employee };

        _repository.GetAsync(Arg.Any<DeviceRequestSpec>(), Arg.Any<CancellationToken>())
            .Returns(new DeviceRequest { UserId = ownerId, Status = RequestStatusEnum.Pending });

        var result = await _sut.DeleteDeviceRequest(Guid.NewGuid(), requestingUser);

        result.Error?.Message.Should().Be(CommonErrors.UnauthorizedDeviceRequestDelete.Message);
    }
}
