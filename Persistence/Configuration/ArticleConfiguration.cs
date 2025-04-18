using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CesiZen_Backend.Models;

namespace CesiZen_Backend.Persistence.Configuration
{
    public class ArticleConfiguration : IEntityTypeConfiguration<Article>
    {
        public void Configure(EntityTypeBuilder<Article> builder)
        {
            builder.ToTable("Articles");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(a => a.Content)
                   .IsRequired();

            builder.HasOne(a => a.Menu)
                   .WithMany()
                   .HasForeignKey(a => a.MenuId);
        }
    }
}
