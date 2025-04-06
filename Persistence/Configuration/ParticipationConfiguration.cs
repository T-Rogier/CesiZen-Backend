using CesiZen_Backend.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CesiZen_Backend.Persistence.Configuration
{
    public class ParticipationConfiguration : IEntityTypeConfiguration<Participation>
    {
        public void Configure(EntityTypeBuilder<Participation> builder)
        {
            builder.ToTable("Participations");

            builder.HasKey(p => p.Id);

            builder.HasOne(p => p.User)
                   .WithMany(u => u.Participations)
                   .HasForeignKey(p => p.UserId);

            builder.HasOne(p => p.Activity)
                   .WithMany(a => a.Participations)
                   .HasForeignKey(p => p.ActivityId);

            builder.Property(p => p.ParticipationDate)
                   .IsRequired();

            builder.Property(p => p.Duration)
                   .IsRequired();
        }
    }
}
