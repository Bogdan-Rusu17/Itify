using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MobyLabWebProgramming.Database.Repository.Entities;
using MobyLabWebProgramming.Database.Repository.Enums;

namespace MobyLabWebProgramming.Database.Repository.EntityConfigurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).IsRequired();
        builder.Property(t => t.Description).HasMaxLength(2000).IsRequired();
        builder.Property(t => t.AdditionalNotes).HasMaxLength(2000);
        builder.Property(t => t.Type)
            .HasConversion(new EnumToStringConverter<TicketTypeEnum>())
            .HasMaxLength(50).IsRequired();
        builder.Property(t => t.Priority)
            .HasConversion(new EnumToStringConverter<TicketPriorityEnum>())
            .HasMaxLength(50).IsRequired();
        builder.Property(t => t.Status)
            .HasConversion(new EnumToStringConverter<TicketStatusEnum>())
            .HasMaxLength(50).IsRequired();
        builder.Property(t => t.CreatedAt).IsRequired();
        builder.Property(t => t.UpdatedAt).IsRequired();
    }
}