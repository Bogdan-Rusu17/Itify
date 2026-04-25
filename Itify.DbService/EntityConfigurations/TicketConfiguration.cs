using Itify.DbService.Entities;
using Itify.DbService.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Itify.DbService.EntityConfigurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).IsRequired();
        builder.Property(e => e.UserId).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(2000).IsRequired();
        builder.Property(e => e.AdditionalNotes).HasMaxLength(2000);
        builder.Property(e => e.Type)
            .HasConversion(new EnumToStringConverter<TicketTypeEnum>())
            .HasMaxLength(50).IsRequired();
        builder.Property(e => e.Priority)
            .HasConversion(new EnumToStringConverter<TicketPriorityEnum>())
            .HasMaxLength(50).IsRequired();
        builder.Property(e => e.Status)
            .HasConversion(new EnumToStringConverter<TicketStatusEnum>())
            .HasMaxLength(50).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.UpdatedAt).IsRequired();
    }
}
