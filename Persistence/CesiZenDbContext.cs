using CesiZen_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CesiZen_Backend.Persistence
{
    public class CesiZenDbContext(DbContextOptions<CesiZenDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Activity> Activities => Set<Activity>();
        public DbSet<Menu> Menus => Set<Menu>();
        public DbSet<Article> Articles => Set<Article>();
        public DbSet<SavedActivity> SavedActivities => Set<SavedActivity>();
        public DbSet<Participation> Participations => Set<Participation>();

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
                    await DataSeeder.SeedAsync((CesiZenDbContext)context, cancellationToken);
                })
                .UseSeeding((context, _) =>
                {
                    DataSeeder.SeedAsync((CesiZenDbContext)context, CancellationToken.None).GetAwaiter().GetResult();
                });
        }

    }
}
