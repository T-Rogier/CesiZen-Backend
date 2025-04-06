using CesiZen_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CesiZen_Backend.Persistence
{
    public class CesiZenDbContext(DbContextOptions<CesiZenDbContext> options) : DbContext(options)
    {
        public DbSet<Activity> Activities => Set<Activity>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Menu> Menus => Set<Menu>();
        public DbSet<Article> Articles => Set<Article>();
        public DbSet<SavedActivity> SavedActivites => Set<SavedActivity>();
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
                    var user = await context.Set<User>().FirstOrDefaultAsync(u => u.Email == "admin@test.com", cancellationToken);
                    if (user == null)
                    {
                        user = User.Create("admin@test.com", "password123", "GAdmin01", UserRole.Admin);
                        await context.Set<User>().AddAsync(user, cancellationToken);
                        await context.SaveChangesAsync(cancellationToken);
                    }

                    var category1 = await context.Set<Category>().FirstOrDefaultAsync(c => c.Name == "Relaxation", cancellationToken);
                    var category2 = await context.Set<Category>().FirstOrDefaultAsync(c => c.Name == "Énergie", cancellationToken);
                    if (category1 == null)
                    {
                        category1 = Category.Create("Relaxation", "https://example.com/icon1.png");
                        category2 = Category.Create("Énergie", "https://example.com/icon2.png");
                        await context.Set<Category>().AddRangeAsync(new[] { category1, category2 }, cancellationToken);
                        await context.SaveChangesAsync(cancellationToken);
                    }

                    var activity = await context.Set<Activity>().FirstOrDefaultAsync(a => a.Title == "Yoga Zen", cancellationToken);
                    if (activity == null)
                    {
                        activity = Activity.Create(
                            title: "Yoga Zen",
                            description: "Séance de yoga pour se relaxer profondément",
                            content: "Contenu vidéo ou audio ici...",
                            thumbnailImageLink: "https://example.com/yoga-thumbnail.jpg",
                            estimatedDuration: TimeSpan.FromMinutes(30),
                            createdBy: user,
                            type: ActivityType.Classique,
                            categories: new List<Category> { category1!, category2! }
                        );

                        await context.Set<Activity>().AddAsync(activity, cancellationToken);
                        await context.SaveChangesAsync(cancellationToken);
                    }
                })
                .UseSeeding((context, _) =>
                {
                    var user = context.Set<User>().FirstOrDefault(u => u.Email == "admin@test.com");
                    if (user == null)
                    {
                        user = User.Create("admin@test.com", "password123", "GAdmin01", UserRole.Admin);
                        context.Set<User>().Add(user);
                        context.SaveChanges();
                    }

                    var category1 = context.Set<Category>().FirstOrDefault(c => c.Name == "Relaxation");
                    var category2 = context.Set<Category>().FirstOrDefault(c => c.Name == "Énergie");
                    if (category1 == null)
                    {
                        category1 = Category.Create("Relaxation", "https://example.com/icon1.png");
                        category2 = Category.Create("Énergie", "https://example.com/icon2.png");
                        context.Set<Category>().AddRange(category1, category2);
                        context.SaveChanges();
                    }

                    var activity = context.Set<Activity>().FirstOrDefault(a => a.Title == "Yoga Zen");
                    if (activity == null)
                    {
                        activity = Activity.Create(
                            title: "Yoga Zen",
                            description: "Séance de yoga pour se relaxer profondément",
                            content: "Contenu vidéo ou audio ici...",
                            thumbnailImageLink: "https://example.com/yoga-thumbnail.jpg",
                            estimatedDuration: TimeSpan.FromMinutes(30),
                            createdBy: user,
                            type: ActivityType.Classique,
                            categories: new List<Category> { category1!, category2! }
                        );

                        context.Set<Activity>().Add(activity);
                        context.SaveChanges();
                    }
                });
        }

    }
}
