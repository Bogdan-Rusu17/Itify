using System.Net;
using MobyLabWebProgramming.Database.Repository;
using MobyLabWebProgramming.Database.Repository.Entities;
using MobyLabWebProgramming.Infrastructure.DataTransferObjects;
using MobyLabWebProgramming.Infrastructure.Errors;
using MobyLabWebProgramming.Infrastructure.Repositories.Interfaces;
using MobyLabWebProgramming.Infrastructure.Requests;
using MobyLabWebProgramming.Infrastructure.Responses;
using MobyLabWebProgramming.Services.Abstractions;
using MobyLabWebProgramming.Services.DataTransferObjects;
using MobyLabWebProgramming.Services.Specifications;

namespace MobyLabWebProgramming.Services.Implementations;

/// <summary>
/// Inject the required services through the constructor.
/// </summary>
public class UserFileService(IRepository<WebAppDatabaseContext> repository, IFileRepository fileRepository) : IUserFileService
{
    /// <summary>
    /// This static method creates the path for a user to where it has to store the files, each user should have an own folder.
    /// </summary>
    private static string GetFileDirectory(Guid userId) => Path.Join(userId.ToString(), IUserFileService.UserFilesDirectory);


    public async Task<ServiceResponse<PagedResponse<UserFileRecord>>> GetUserFiles(PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default)
    {
        var result = await repository.PageAsync(pagination, new UserFileProjectionSpec(pagination.Search), cancellationToken);

        return ServiceResponse.ForSuccess(result);
    }

    public async Task<ServiceResponse> SaveFile(UserFileAddRecord file, UserRecord requestingUser, CancellationToken cancellationToken = default)
    {
        var fileName = fileRepository.SaveFile(file.File, GetFileDirectory(requestingUser.Id)); // First save the file on the filesystem.

        if (fileName.Result == null) // If not successful respond with the error.
        {
            return fileName.ToResponse();
        }

        await repository.AddAsync(new UserFile
        {
            Name = file.File.FileName,
            Description = file.Description,
            Path = fileName.Result,
            UserId = requestingUser.Id
        }, cancellationToken); // When the file is saved on the filesystem save the returned file path in the database to identify the file.

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse<FileRecord>> GetFileDownload(Guid id, CancellationToken cancellationToken = default) // If not successful respond with the error.
    {
        var userFile = await repository.GetAsync<UserFile>(id, cancellationToken); // First get the file entity from the database to find the location on the filesystem.

        return userFile != null ? 
            fileRepository.GetFile(Path.Join(GetFileDirectory(userFile.UserId), userFile.Path), userFile.Name) : 
            ServiceResponse.FromError<FileRecord>(new(HttpStatusCode.NotFound, "File entry not found!", ErrorCodes.EntityNotFound));
    }
}
