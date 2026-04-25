using Itify.DbService.Entities;
using Itify.DbService.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Itify.DbService.Contexts;

public sealed class EquipmentDbContext(DbContextOptions<EquipmentDbContext> options) : DbContext(options)
{
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<DeviceCategory> DeviceCategories => Set<DeviceCategory>();
    public DbSet<DeviceAssignment> DeviceAssignments => Set<DeviceAssignment>();
    public DbSet<DeviceRequest> DeviceRequests => Set<DeviceRequest>();
    public DbSet<Ticket> Tickets => Set<Ticket>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasPostgresExtension("unaccent");
        modelBuilder.ApplyConfiguration(new DeviceCategoryConfiguration());
        modelBuilder.ApplyConfiguration(new DeviceConfiguration());
        modelBuilder.ApplyConfiguration(new DeviceAssignmentConfiguration());
        modelBuilder.ApplyConfiguration(new DeviceRequestConfiguration());
        modelBuilder.ApplyConfiguration(new TicketConfiguration());
    }
}
