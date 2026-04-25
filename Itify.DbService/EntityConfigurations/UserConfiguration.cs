using Itify.DbService.Entities;
using Itify.DbService.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Itify.DbService.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).IsRequired();
        builder.Property(e => e.Name).HasMaxLength(255).IsRequired();
        builder.Property(e => e.Email).HasMaxLength(255).IsRequired();
        builder.HasAlternateKey(e => e.Email);
        builder.Property(e => e.Password).HasMaxLength(255).IsRequired();
        builder.Property(e => e.Role)
            .HasConversion(new EnumToStringConverter<UserRoleEnum>())
            .HasMaxLength(255).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.UpdatedAt).IsRequired();
    }
}
