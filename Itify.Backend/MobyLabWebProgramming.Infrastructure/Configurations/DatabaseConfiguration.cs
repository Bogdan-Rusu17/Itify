namespace MobyLabWebProgramming.Infrastructure.Configurations;

/// <summary>
/// This class is used to make configuration for the database such as enabling automatic database migrations.
/// </summary>
public class DatabaseConfiguration
{
    public bool EnableAutomaticMigrations { get; set; }
}