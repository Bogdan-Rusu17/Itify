using Itify.Database.Repository;
using Itify.Infrastructure.Configurations;
using Itify.Infrastructure.Repositories.Implementation;
using Itify.Infrastructure.Repositories.Interfaces;
using Itify.Services.Abstractions;
using Itify.Services.Implementations;
using Itify.Services.Workers;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Itify.Services.Extensions;

public static class WebApplicationBuilderExtensions
{
    private const string WebAppDatabaseConnectionKey = "WebAppDatabase";

    /// <summary>
    ///     This extension method adds the database configuration and repository to the application builder.
    /// </summary>
    public static WebApplicationBuilder AddRepository(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<DatabaseConfiguration>(
            builder.Configuration.GetSection(nameof(DatabaseConfiguration)));
        builder.Services.AddDbContext<WebAppDatabaseContext>(options =>
            options.UseNpgsql(
                builder.Configuration.GetConnectionString(
                    WebAppDatabaseConnectionKey), // This gets the connection string from ConnectionStrings.WebAppDatabase in appsettings.json.
                o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery)
                    .CommandTimeout((int)TimeSpan.FromMinutes(15).TotalSeconds)));
        builder.Services.AddScoped<IRepository<WebAppDatabaseContext>, Repository<WebAppDatabaseContext>>();

        return builder;
    }

    /// <summary>
    ///     This extension method adds any necessary services to the application builder that need to be injected by the
    ///     framework.
    /// </summary>
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection(nameof(JwtConfiguration)));
        builder.Services.Configure<FileStorageConfiguration>(
            builder.Configuration.GetSection(nameof(FileStorageConfiguration)));
        builder.Services.Configure<MailConfiguration>(builder.Configuration.GetSection(nameof(MailConfiguration)));
        builder.Services
            .AddScoped<IUserService, UserService>()
            .AddScoped<ILoginService, LoginService>()
            .AddScoped<IFileRepository, FileRepository>()
            .AddScoped<IUserFileService, UserFileService>()
            .AddScoped<IMailService, MailService>()
            .AddScoped<IDeviceService, DeviceService>()
            .AddScoped<IDeviceCategoryService, DeviceCategoryService>();

        return builder;
    }

    /// <summary>
    ///     This extension method adds asynchronous workers to the application builder.
    /// </summary>
    public static WebApplicationBuilder AddWorkers(this WebApplicationBuilder builder)
    {
        builder.Services.AddHostedService<InitializerWorker>();

        return builder;
    }
}