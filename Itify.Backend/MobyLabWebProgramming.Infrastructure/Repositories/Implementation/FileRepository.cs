using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MobyLabWebProgramming.Infrastructure.Configurations;
using MobyLabWebProgramming.Infrastructure.DataTransferObjects;
using MobyLabWebProgramming.Infrastructure.Errors;
using MobyLabWebProgramming.Infrastructure.Repositories.Interfaces;
using MobyLabWebProgramming.Infrastructure.Responses;

namespace MobyLabWebProgramming.Infrastructure.Repositories.Implementation;

public class FileRepository : IFileRepository
{
    private readonly string _fileStoragePath;

    private static void CreateIfNotExists(string path) => Directory.CreateDirectory(path); // If the directory path doesn't exist it should be created.

    /// <summary>
    /// This gets a new unique filename.
    /// When managing files the filename on your filesystem should be one decided by the application to avoid security issues and avoid overriding the files by requesting a unique filename.
    /// </summary>
    private static string NewFileName(string extension) =>
        Path.GetRandomFileName() + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + extension;

    /// <summary>
    /// Inject the required service configuration from the application.json or environment variables.
    /// </summary>
    public FileRepository(IOptions<FileStorageConfiguration> fileStorage)
    {
        _fileStoragePath = fileStorage.Value.SavePath;
        CreateIfNotExists(_fileStoragePath); // Create the file storage path if it doesn't exist.
    }

    public ServiceResponse<FileRecord> GetFile(string filePath, string? replacedFileName = null)
    {
        try
        {
            var path = Path.Join(_fileStoragePath, filePath);

            return File.Exists(path)
                ? ServiceResponse.ForSuccess<FileRecord>(new(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read), replacedFileName ?? Path.GetFileName(filePath)))
                : ServiceResponse.FromError<FileRecord>(CommonErrors.FileNotFound);
        }
        catch
        {
            return ServiceResponse.FromError<FileRecord>(CommonErrors.TechnicalSupport);
        }
    }

    public ServiceResponse<FileRecord> SaveFileAndGet(IFormFile file, string directoryPath)
    {
        try
        {
            directoryPath = Path.Join(_fileStoragePath, directoryPath);
            var extension = Path.GetExtension(file.FileName);
            var newName = NewFileName(extension);
            CreateIfNotExists(directoryPath);
            var savePath = Path.Combine(directoryPath, newName);
            var fileStream = File.Open(savePath, FileMode.CreateNew);

            file.CopyTo(fileStream);
            fileStream.Seek(0, SeekOrigin.Begin);

            return ServiceResponse.ForSuccess<FileRecord>(new(fileStream, newName));
        }
        catch
        {
            return ServiceResponse.FromError<FileRecord>(CommonErrors.TechnicalSupport);
        }
    }

    public ServiceResponse<string> SaveFile(IFormFile file, string directoryPath)
    {
        try
        {
            directoryPath = Path.Join(_fileStoragePath, directoryPath);
            var extension = Path.GetExtension(file.FileName);
            var newName = NewFileName(extension);
            CreateIfNotExists(directoryPath);
            var savePath = Path.Combine(directoryPath, newName);
            using var fileStream = File.Open(savePath, FileMode.CreateNew);

            file.CopyTo(fileStream);

            return ServiceResponse.ForSuccess(newName);
        }
        catch
        {
            return ServiceResponse.FromError<string>(CommonErrors.TechnicalSupport);
        }
    }

    public ServiceResponse<FileRecord> SaveFileAndGet(byte[] file, string directoryPath, string fileExtension)
    {
        try
        {
            directoryPath = Path.Join(_fileStoragePath, directoryPath);
            var newName = Path.GetRandomFileName() + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + fileExtension;
            CreateIfNotExists(directoryPath);
            var savePath = Path.Combine(directoryPath, newName);
            var fileStream = File.Open(savePath, FileMode.CreateNew);

            fileStream.Write(file);
            fileStream.Seek(0, SeekOrigin.Begin);

            return ServiceResponse.ForSuccess(new FileRecord(fileStream, newName));
        }
        catch
        {
            return ServiceResponse.FromError<FileRecord>(CommonErrors.TechnicalSupport);
        }
    }

    public ServiceResponse<string> SaveFile(byte[] file, string directoryPath, string fileExtension)
    {
        try
        {
            directoryPath = Path.Join(_fileStoragePath, directoryPath);
            var newName = Path.GetRandomFileName() + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + fileExtension;
            CreateIfNotExists(directoryPath);
            var savePath = Path.Combine(directoryPath, newName);
            using var fileStream = File.Open(savePath, FileMode.CreateNew);

            fileStream.Write(file);

            return ServiceResponse.ForSuccess(newName);
        }
        catch
        {
            return ServiceResponse.FromError<string>(CommonErrors.TechnicalSupport);
        }
    }

    public ServiceResponse<FileRecord> UpdateFileAndGet(IFormFile file, string filePath)
    {
        try
        {
            filePath = Path.Join(_fileStoragePath, filePath);
            var extension = Path.GetExtension(file.FileName);
            var currentExtension = Path.GetExtension(filePath);

            if (!string.Equals(extension.ToLower(), currentExtension.ToLower(), StringComparison.CurrentCultureIgnoreCase))
            {
                var newPath = filePath.Replace(currentExtension, extension);
                File.Move(filePath, newPath);
                filePath = newPath;
            }

            var fileStream = File.Open(filePath, FileMode.Truncate);
            file.CopyTo(fileStream);
            fileStream.Seek(0, SeekOrigin.Begin);

            return ServiceResponse.ForSuccess(new FileRecord(fileStream, Path.GetFileName(filePath)));
        }
        catch
        {
            return ServiceResponse.FromError<FileRecord>(CommonErrors.TechnicalSupport);
        }
    }

    public ServiceResponse<string> UpdateFile(IFormFile file, string filePath)
    {
        try
        {
            filePath = Path.Join(_fileStoragePath, filePath);
            var extension = Path.GetExtension(file.FileName);
            var currentExtension = Path.GetExtension(filePath);

            if (!string.Equals(extension.ToLower(), currentExtension.ToLower(), StringComparison.CurrentCultureIgnoreCase))
            {
                var newPath = filePath.Replace(currentExtension, extension);
                File.Move(filePath, newPath);
                filePath = newPath;
            }

            using var fileStream = File.Open(filePath, FileMode.Truncate);
            file.CopyTo(fileStream);

            return ServiceResponse.ForSuccess(Path.GetFileName(filePath));
        }
        catch
        {
            return ServiceResponse.FromError<string>(CommonErrors.TechnicalSupport);
        }
    }

    public ServiceResponse<FileRecord> UpdateFileAndGet(byte[] file, string filePath)
    {
        try
        {
            filePath = Path.Join(_fileStoragePath, filePath);
            var fileStream = File.Open(filePath, FileMode.Truncate);

            fileStream.Write(file);
            fileStream.Seek(0, SeekOrigin.Begin);

            return ServiceResponse.ForSuccess(new FileRecord(fileStream, Path.GetFileName(filePath)));
        }
        catch
        {
            return ServiceResponse.FromError<FileRecord>(CommonErrors.TechnicalSupport);
        }
    }

    public ServiceResponse<string> UpdateFile(byte[] file, string filePath)
    {
        try
        {
            filePath = Path.Join(_fileStoragePath, filePath);
            using var fileStream = File.Open(filePath, FileMode.Truncate);
            fileStream.Write(file);

            return ServiceResponse.ForSuccess(Path.GetFileName(filePath));
        }
        catch
        {
            return ServiceResponse.FromError<string>(CommonErrors.TechnicalSupport);
        }
    }

    public ServiceResponse DeleteFile(string filePath)
    {
        try
        {
            filePath = Path.Join(_fileStoragePath, filePath);
            File.Delete(filePath);

            return ServiceResponse.ForSuccess();
        }
        catch
        {
            return ServiceResponse.FromError(CommonErrors.TechnicalSupport);
        }
    }
}
