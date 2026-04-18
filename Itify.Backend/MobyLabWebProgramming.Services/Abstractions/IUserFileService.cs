using MobyLabWebProgramming.Infrastructure.DataTransferObjects;
using MobyLabWebProgramming.Infrastructure.Requests;
using MobyLabWebProgramming.Infrastructure.Responses;
using MobyLabWebProgramming.Services.DataTransferObjects;

namespace MobyLabWebProgramming.Services.Abstractions;

/// <summary>
/// This service is a simple service to demonstrate how to work with files.
/// </summary>
public interface IUserFileService
{
    public const string UserFilesDirectory = "UserFiles";

    /// <summary>
    /// GetUserFiles gets the user files as pages from the database.
    /// </summary>
    public Task<ServiceResponse<PagedResponse<UserFileRecord>>> GetUserFiles(PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default);
    /// <summary>
    /// SaveFile saves a file on the file storage and also saves the path to the database for a requesting user.
    /// </summary>
    public Task<ServiceResponse> SaveFile(UserFileAddRecord file, UserRecord requestingUser, CancellationToken cancellationToken = default);
    /// <summary>
    /// GetFileDownload gets a file stream for a given file found by the id in the database.
    /// </summary>
    public Task<ServiceResponse<FileRecord>> GetFileDownload(Guid id, CancellationToken cancellationToken = default);
}
