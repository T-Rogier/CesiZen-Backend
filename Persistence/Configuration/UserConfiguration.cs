using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CesiZen_Backend.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CesiZen_Backend.Persistence.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Username)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasIndex(u => u.Username)
                   .IsUnique();

            builder.Property(u => u.Email)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.HasIndex(u => u.Email)
                   .IsUnique();

            builder.Property(u => u.Password)
                   .IsRequired()
                   .HasMaxLength(300);

            builder.Property(u => u.Disabled)
                   .IsRequired();

            builder.Property(u => u.Role)
                   .HasConversion(new EnumToStringConverter<UserRole>())
                   .IsRequired();

            builder.Property(u => u.Created)
                   .IsRequired()
                   .ValueGeneratedOnAdd();

            builder.Property(u => u.Updated)
                   .IsRequired()
                   .ValueGeneratedOnUpdate();
        }
    }
}
