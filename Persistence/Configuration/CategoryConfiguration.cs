using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CesiZen_Backend.Models;

namespace CesiZen_Backend.Persistence.Configuration
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(c => c.IconLink)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(c => c.Deleted)
                   .IsRequired();

            builder.Property(m => m.Created)
                   .IsRequired()
                   .ValueGeneratedOnAdd();

            builder.Property(m => m.Updated)
                   .IsRequired()
                   .ValueGeneratedOnUpdate();

            builder.HasIndex(c => c.Name).IsUnique();
        }
    }
}
