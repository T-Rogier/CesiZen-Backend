using CesiZen_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CesiZen_Backend.Persistence
{
    public class CesiZenDbContext : DbContext
    {
        public CesiZenDbContext(DbContextOptions<CesiZenDbContext> options): base(options) { }
        protected CesiZenDbContext() { }

        public virtual DbSet<User> Users => Set<User>();
        public virtual DbSet<Category> Categories => Set<Category>();
        public virtual DbSet<Activity> Activities => Set<Activity>();
        public virtual DbSet<Menu> Menus => Set<Menu>();
        public virtual DbSet<Article> Articles => Set<Article>();
        public virtual DbSet<SavedActivity> SavedActivities => Set<SavedActivity>();
        public virtual DbSet<Participation> Participations => Set<Participation>();

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
