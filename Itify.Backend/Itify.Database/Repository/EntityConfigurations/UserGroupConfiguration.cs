using Itify.Database.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itify.Database.Repository.EntityConfigurations;

public class UserGroupConfiguration : IEntityTypeConfiguration<UserGroup>
{
    public void Configure(EntityTypeBuilder<UserGroup> builder)
    {
        builder.HasKey(ug => ug.Id);
        builder.Property(ug => ug.Id).IsRequired();
        builder.Property(ug => ug.Name).HasMaxLength(255).IsRequired();
        builder.Property(ug => ug.Description).HasMaxLength(1000);
        builder.Property(ug => ug.CreatedAt).IsRequired();
        builder.Property(ug => ug.UpdatedAt).IsRequired();

        builder.HasMany(ug => ug.Users)
            .WithMany(u => u.UserGroups);
    }
}