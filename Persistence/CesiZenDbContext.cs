using CesiZen_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace CesiZen_Backend.Persistence
{
    public class CesiZenDbContext(DbContextOptions<CesiZenDbContext> options) : DbContext(options)
    {
        public DbSet<Activity> Activities => Set<Activity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("app");
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CesiZenDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseAsyncSeeding(async (context, _, cancellationToken) =>
                {
                    var sampleActivity = await context.Set<Activity>().FirstOrDefaultAsync(b => b.Title == "Yoga Matinal", cancellationToken);
                    if (sampleActivity == null)
                    {
                        sampleActivity = Activity.Create(
                            title: "Yoga Matinal",
                            description: "Une séance de yoga pour bien commencer la journée.",
                            content: "Cette activité inclut des exercices de respiration et des postures relaxantes.",
                            thumbnailImageLink: "https://example.com/yoga.jpg"
                        );
                        await context.Set<Activity>().AddAsync(sampleActivity, cancellationToken);
                        await context.SaveChangesAsync(cancellationToken);
                    }
                })
                .UseSeeding((context, _) =>
                {
                    var sampleActivity = context.Set<Activity>().FirstOrDefault(b => b.Title == "Méditation Guidée");
                    if (sampleActivity == null)
                    {
                        sampleActivity = Activity.Create(
                            title: "Méditation Guidée",
                            description: "Un moment de méditation pour apaiser l'esprit.",
                            content: "Guidé par un expert, vous apprendrez à vous détendre en pleine conscience.",
                            thumbnailImageLink: "https://example.com/meditation.jpg"
                        );
                        context.Set<Activity>().Add(sampleActivity);
                        context.SaveChanges();
                    }
                });
        }
    }
}
