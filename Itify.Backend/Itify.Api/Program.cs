using Itify.Database.Repository;
using Itify.Infrastructure.Extensions;
using Itify.Services.Extensions;

namespace Itify.Api;

public static class Program
{
    private const string ApplicationName = "Itify.Services";
    
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