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

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(m => m.Description)
                   .IsRequired()
                   .HasMaxLength(2000);

            builder.Property(m => m.Content)
                   .IsRequired()
                   .HasMaxLength(20000);

            builder.Property(m => m.ThumbnailImageLink)
                   .HasMaxLength(1000);

            builder.Property(m => m.EstimatedDuration)
                   .IsRequired();

            builder.Property(m => m.ViewCount)
                   .IsRequired();

            builder.Property(m => m.Activated)
                   .IsRequired();

            builder.Property(m => m.Deleted)
                   .IsRequired();

            builder.Property(m => m.Type)
                   .HasConversion(new EnumToStringConverter<ActivityType>())
                   .IsRequired();

            builder.Property(m => m.CreatedById)
                   .IsRequired();

            builder.HasOne(m => m.CreatedBy)
                   .WithMany()
                   .HasForeignKey(m => m.CreatedById)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(a => a.Categories)
                   .WithMany()
                   .UsingEntity<Dictionary<string, object>>(
                       "ActivityCategories",
                       j => j.HasOne<Category>().WithMany().HasForeignKey("CategoryId"),
                       j => j.HasOne<Activity>().WithMany().HasForeignKey("ActivityId"));

            builder.Property(m => m.Created)
                   .IsRequired()
                   .ValueGeneratedOnAdd();

            builder.Property(m => m.Updated)
                   .IsRequired()
                   .ValueGeneratedOnUpdate();

            builder.HasIndex(m => m.Title);
        }
    }
}
}
