using Itify.DbService.Entities;
using Itify.DbService.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Itify.DbService.EntityConfigurations;

public class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).IsRequired();
        builder.Property(e => e.Name).HasMaxLength(255).IsRequired();
        builder.Property(e => e.SerialNumber).HasMaxLength(255).IsRequired();
        builder.HasAlternateKey(e => e.SerialNumber);
        builder.Property(e => e.Status)
            .HasConversion(new EnumToStringConverter<DeviceStatusEnum>())
            .HasMaxLength(50).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.UpdatedAt).IsRequired();

        builder.HasMany(e => e.Assignments)
            .WithOne(a => a.Device)
            .HasForeignKey(a => a.DeviceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
