using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MobyLabWebProgramming.Database.Repository.Entities;
using MobyLabWebProgramming.Database.Repository.Enums;

namespace MobyLabWebProgramming.Database.Repository.EntityConfigurations;

public class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).IsRequired();
        builder.Property(d => d.Name).HasMaxLength(255).IsRequired();
        builder.Property(d => d.SerialNumber).HasMaxLength(255).IsRequired();
        builder.HasAlternateKey(d => d.SerialNumber);
        builder.Property(d => d.Status)
            .HasConversion(new EnumToStringConverter<DeviceStatusEnum>())
            .HasMaxLength(50).IsRequired();
        builder.Property(d => d.CreatedAt).IsRequired();
        builder.Property(d => d.UpdatedAt).IsRequired();
        
        builder.HasMany(e => e.Assignments)
            .WithOne(da => da.Device)
            .HasForeignKey(da => da.DeviceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}