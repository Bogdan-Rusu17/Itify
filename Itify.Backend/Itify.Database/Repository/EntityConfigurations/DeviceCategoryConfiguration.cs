using Itify.Database.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itify.Database.Repository.EntityConfigurations;

public class DeviceCategoryConfiguration : IEntityTypeConfiguration<DeviceCategory>
{
    public void Configure(EntityTypeBuilder<DeviceCategory> builder)
    {
        builder.HasKey(dc => dc.Id);
        builder.Property(dc => dc.Name).HasMaxLength(255).IsRequired();
        builder.Property(dc => dc.Description).HasMaxLength(1000);
        builder.Property(dc => dc.CreatedAt).IsRequired();
        builder.Property(dc => dc.UpdatedAt).IsRequired();
        
        builder.HasMany(dc => dc.Devices)
            .WithOne(d => d.Category)
            .HasForeignKey(d => d.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(dc => dc.DeviceRequests)
            .WithOne(d => d.Category)
            .HasForeignKey(d => d.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}