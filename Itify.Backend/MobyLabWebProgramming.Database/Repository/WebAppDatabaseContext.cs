using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Infrastructure.Extensions;

namespace MobyLabWebProgramming.Database.Repository;

/// <summary>
/// This is the database context used to connect with the database and links the ORM, Entity Framework, with it.
/// </summary>
public sealed class WebAppDatabaseContext(DbContextOptions<WebAppDatabaseContext> options, IServiceProvider serviceProvider) : DbContext(options)
{
    /// <summary>
    /// Here additional configuration for the ORM is performed.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasPostgresExtension("unaccent")
            .ApplyConfigurationsFromAssemblies([], serviceProvider); // Here all the classes that contain implement IEntityTypeConfiguration<T> are searched at runtime
                                                                     // such that each entity that needs to be mapped to the database tables is configured accordingly.
    }
}