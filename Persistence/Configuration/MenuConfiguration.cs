using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CesiZen_Backend.Models;

namespace CesiZen_Backend.Persistence.Configuration
{
    public class MenuConfiguration : IEntityTypeConfiguration<Menu>
    {
        public void Configure(EntityTypeBuilder<Menu> builder)
        {
            builder.ToTable("Menus");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(m => m.HierarchyLevel)
                   .IsRequired();

            builder.HasOne(m => m.Parent)
                   .WithMany()
                   .HasForeignKey(m => m.ParentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(m => m.Created)
                   .IsRequired()
                   .ValueGeneratedOnAdd();

            builder.Property(m => m.Updated)
                   .IsRequired()
                   .ValueGeneratedOnUpdate();
        }
    }
}
