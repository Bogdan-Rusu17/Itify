using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MobyLabWebProgramming.Database.Repository.Entities;
using MobyLabWebProgramming.Database.Repository.Enums;

namespace MobyLabWebProgramming.Database.Repository.EntityConfigurations;

public class DeviceRequestConfiguration : IEntityTypeConfiguration<DeviceRequest>
{
    public void Configure(EntityTypeBuilder<DeviceRequest> builder)
    {
        builder.HasKey(dr => dr.Id);
        builder.Property(dr => dr.Id).IsRequired();
        builder.Property(dr => dr.Reason).HasMaxLength(1000).IsRequired();
        builder.Property(dr => dr.Status)
            .HasConversion(new EnumToStringConverter<RequestStatusEnum>())
            .HasMaxLength(50).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.UpdatedAt).IsRequired();
    }
}