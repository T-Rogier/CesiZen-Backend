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
                   .HasMaxLength(300);

            builder.Property(u => u.Disabled)
                   .IsRequired();

            builder.Property(u => u.Role)
                   .HasConversion(new EnumToStringConverter<UserRole>())
                   .IsRequired();

            builder.Property(u => u.Provider)
                   .HasMaxLength(100);

            builder.Property(u => u.ProviderId)
                   .HasMaxLength(300);

            builder.Property(u => u.RefreshToken)
                   .HasMaxLength(512);

            builder.Property(u => u.RefreshTokenExpiryTime);
        }
    }
}
