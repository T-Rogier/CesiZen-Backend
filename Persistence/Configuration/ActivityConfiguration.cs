using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CesiZen_Backend.Models;

namespace CesiZen_Backend.Persistence.Configuration
{
    public class ActivityConfiguration : IEntityTypeConfiguration<Activity>
    {
        public void Configure(EntityTypeBuilder<Activity> builder)
        {
            // Define table name
            builder.ToTable("Activities");

            // Set primary key
            builder.HasKey(m => m.Id);

            // Configure properties
            builder.Property(m => m.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(m => m.Description)
                   .IsRequired()
                   .HasMaxLength(2000);

            builder.Property(m => m.Content)
                   .IsRequired()
                   .HasMaxLength(200);


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
