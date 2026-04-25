using Itify.DbService.Entities;
using Itify.DbService.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Itify.DbService.Contexts;

public sealed class UsersDbContext(DbContextOptions<UsersDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UserFile> UserFiles => Set<UserFile>();
    public DbSet<UserGroup> UserGroups => Set<UserGroup>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasPostgresExtension("unaccent");
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new UserFileConfiguration());
        modelBuilder.ApplyConfiguration(new UserGroupConfiguration());
    }
}
