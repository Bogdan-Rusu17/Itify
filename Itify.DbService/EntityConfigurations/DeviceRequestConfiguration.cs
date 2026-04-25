using Itify.DbService.Entities;
using Itify.DbService.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Itify.DbService.EntityConfigurations;

public class DeviceRequestConfiguration : IEntityTypeConfiguration<DeviceRequest>
{
    public void Configure(EntityTypeBuilder<DeviceRequest> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).IsRequired();
        builder.Property(e => e.UserId).IsRequired();
        builder.Property(e => e.Reason).HasMaxLength(1000).IsRequired();
        builder.Property(e => e.Status)
            .HasConversion(new EnumToStringConverter<RequestStatusEnum>())
            .HasMaxLength(50).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.UpdatedAt).IsRequired();
    }
}
