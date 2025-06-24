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

            builder.HasOne(s => s.User)
                   .WithMany(s => s.SavedActivities)
                   .HasForeignKey(s => s.UserId);

            builder.HasOne(s => s.Activity)
                   .WithMany(s => s.SavedActivities)
                   .HasForeignKey(s => s.ActivityId);

            builder.Property(s => s.IsFavoris)
                   .IsRequired();

            builder.Property(s => s.State)
                   .HasConversion(new EnumToStringConverter<SavedActivityStates>())
                   .IsRequired();

            builder.Property(s => s.Progress)
                   .HasConversion(new PercentageConverter())
                   .HasColumnType("double precision")
                   .IsRequired();
        }
    }
}
