using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CesiZen_Backend.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CesiZen_Backend.Persistence.Configuration
{
    public class ActivityConfiguration : IEntityTypeConfiguration<Activity>
    {
        public void Configure(EntityTypeBuilder<Activity> builder)
        {
            builder.ToTable("Activities");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(a => a.Description)
                   .IsRequired()
                   .HasMaxLength(2000);

            builder.Property(a => a.Content)
                   .IsRequired()
                   .HasMaxLength(20000);

            builder.Property(a => a.ThumbnailImageLink)
                   .HasMaxLength(1000);

            builder.Property(a => a.EstimatedDuration)
                   .IsRequired();

            builder.Property(a => a.ViewCount)
                   .IsRequired();

            builder.Property(a => a.Activated)
                   .IsRequired();

            builder.Property(a => a.Deleted)
                   .IsRequired();

            builder.Property(a => a.Type)
                   .HasConversion(new EnumToStringConverter<ActivityType>())
                   .IsRequired();

            builder.Property(a => a.CreatedById)
                   .IsRequired();

            builder.HasOne(a => a.CreatedBy)
                   .WithMany()
                   .HasForeignKey(a => a.CreatedById)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(a => a.Categories)
                   .WithMany()
                   .UsingEntity<Dictionary<string, object>>(
                       "ActivityCategories",
                       j => j.HasOne<Category>().WithMany().HasForeignKey("CategoryId"),
                       j => j.HasOne<Activity>().WithMany().HasForeignKey("ActivityId"));

            builder.Property(a => a.Created)
                   .IsRequired()
                   .ValueGeneratedOnAdd();

            builder.Property(a => a.Updated)
                   .IsRequired()
                   .ValueGeneratedOnUpdate();

            builder.HasIndex(a => a.Title);
        }
    }
}
