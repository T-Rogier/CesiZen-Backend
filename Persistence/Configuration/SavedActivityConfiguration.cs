using CesiZen_Backend.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CesiZen_Backend.Persistence.Configuration
{
    public class SavedActivityConfiguration : IEntityTypeConfiguration<SavedActivity>
    {
        public void Configure(EntityTypeBuilder<SavedActivity> builder)
        {
            builder.ToTable("SavedActivities");

            builder.HasKey(s => new { s.UserId, s.ActivityId });

            builder.HasOne(p => p.User)
                   .WithMany(u => u.SavedActivities)
                   .HasForeignKey(p => p.UserId);

            builder.HasOne(p => p.Activity)
                   .WithMany(a => a.SavedActivities)
                   .HasForeignKey(p => p.ActivityId);

            builder.Property(p => p.IsFavoris)
                   .IsRequired();

            builder.Property(p => p.State)
                   .HasConversion(new EnumToStringConverter<SavedActivityStates>())
                   .IsRequired();

            builder.Property(p => p.Progress)
                   .HasConversion(new PercentageConverter())
                   .HasColumnType("double precision")
                   .IsRequired();
        }
    }
}
