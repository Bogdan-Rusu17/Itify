using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MobyLabWebProgramming.Infrastructure.Configurations;
using MobyLabWebProgramming.Infrastructure.Middlewares;
using Serilog;

namespace MobyLabWebProgramming.Services.Extensions;

public static class WebApplicationExtensions
{
    /// <summary>
    /// This extension method adds all the configuration for the application that is about to run.
    /// </summary>
    public static WebApplication ConfigureApplication(this WebApplication application, string applicationName)
    {
        application.UseMiddleware<GlobalExceptionHandlerMiddleware>() // Adds the global exception handler middleware.
            .UseSwagger() // Adds the swagger.
            .UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", applicationName)) // Add the swagger UI with the application name.
            .UseCors() // Sets to use the CORS configuration.
            .UseRouting() // Adds routing.
            .UseAuthentication() // Adds authentication.
            .UseSerilogRequestLogging() // Adds advanced logging using the Serilog NuGets.
            .UseAuthorization() // Adds authorization to verify the JWT.
            .UseSerilogRequestLogging(options => 
            {
                // Adds serilog configuration for logging request.
                options.MessageTemplate = "{RemoteIpAddress} {RequestScheme} {RequestHost} {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value ?? "");
                    diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme.ToUpper());
                    diagnosticContext.Set("RemoteIpAddress", httpContext.Connection.RemoteIpAddress?.ToString() ?? "");
                };
            });
        application.MapControllers(); // Adds controller mappings.

        return application;
    }

    public static WebApplication MigrateDatabase<T>(this WebApplication application) where T : DbContext
    {
        try
        {
            var options = application.Services.GetRequiredService<IOptions<DatabaseConfiguration>>();

            if (options.Value.EnableAutomaticMigrations)
            {
                using var scope = application.Services.CreateScope();  
                var dbContext = scope.ServiceProvider.GetRequiredService<T>();
                dbContext.Database.Migrate();
            }
        }
        catch (Exception ex)
        {
            var logger = application.Services.GetService<ILogger>();

            logger?.Error(ex, "Could not perform migration");
        }
        
        return application;
    }
}
