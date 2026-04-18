using MobyLabWebProgramming.Database.Repository;
using MobyLabWebProgramming.Infrastructure.Extensions;
using MobyLabWebProgramming.Services.Extensions;

namespace MobyLabWebProgramming.Api;

public static class Program
{
    private const string ApplicationName = "MobyLab Web App";
    
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.AddCorsConfiguration()
            .AddRepository()
            .AddAuthorizationWithSwagger(ApplicationName)
            .AddServices()
            .UseLogger()
            .AddWorkers()
            .AddApi();

        var app = builder
            .Build()
            .ConfigureApplication(ApplicationName)
            .MigrateDatabase<WebAppDatabaseContext>();
        
        app.Run();
    }
}