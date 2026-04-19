using Itify.Database.Repository.Enums;

namespace Itify.Services.DataTransferObjects;

public record UserUpdateRecord(Guid Id, string? Name = null, string? Password = null, UserRoleEnum? Role = null);
