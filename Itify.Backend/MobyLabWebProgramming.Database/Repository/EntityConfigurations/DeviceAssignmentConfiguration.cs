using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MobyLabWebProgramming.Database.Repository.Entities;

namespace MobyLabWebProgramming.Database.Repository.EntityConfigurations;

public class DeviceAssignmentConfiguration : IEntityTypeConfiguration<DeviceAssignment>
{
    public void Configure(EntityTypeBuilder<DeviceAssignment> builder)
    {
        builder.HasKey(da => da.Id);
        builder.Property(da => da.Id).IsRequired();
        builder.Property(da => da.AssignedAt).IsRequired();
        builder.Property(da => da.CreatedAt).IsRequired();
        builder.Property(da => da.UpdatedAt).IsRequired();

        builder.HasOne(da => da.Ticket)
            .WithOne(da => da.DeviceAssignment)
            .HasForeignKey<Ticket>(t => t.DeviceAssignmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}