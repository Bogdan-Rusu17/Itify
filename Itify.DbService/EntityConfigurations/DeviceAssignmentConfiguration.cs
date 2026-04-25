using Itify.DbService.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itify.DbService.EntityConfigurations;

public class DeviceAssignmentConfiguration : IEntityTypeConfiguration<DeviceAssignment>
{
    public void Configure(EntityTypeBuilder<DeviceAssignment> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).IsRequired();
        builder.Property(e => e.UserId).IsRequired();
        builder.Property(e => e.AssignedAt).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.UpdatedAt).IsRequired();

        builder.HasOne(e => e.Ticket)
            .WithOne(t => t.DeviceAssignment)
            .HasForeignKey<Ticket>(t => t.DeviceAssignmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
