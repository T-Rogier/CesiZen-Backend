using Bogus;
using CesiZen_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CesiZen_Backend.Persistence
{
    public static class DataSeeder
    {
        // Générer des utilisateurs
        public static List<User> GenerateUsers(int count)
        {
            var faker = new Faker<User>()
                .CustomInstantiator(f =>
                {
                    return User.Create(
                        identifiant: f.Internet.UserName(),
                        email: f.Internet.Email(),
                        password: f.Internet.Password(),
                        disabled: false,
                        role: f.PickRandom(UserRole.User, UserRole.Admin)
                    );
                });

            return faker.Generate(count);
        }

        // Générer des activités
        public static List<Activity> GenerateActivities(List<User> users, List<Category> categories, int count)
        {
            var faker = new Faker<Activity>()
                .CustomInstantiator(f =>
                {
                    User user = f.PickRandom(users);
                    List<Category> selectedCategories = f.PickRandom(categories, f.Random.Int(1, 3)).ToList();
                    return Activity.Create(
                        title: f.Lorem.Sentence(3),
                        description: f.Lorem.Paragraph(),
                        content: f.Lorem.Paragraphs(2),
                        thumbnailImageLink: f.Image.PicsumUrl(),
                        estimatedDuration: TimeSpan.FromMinutes(f.Random.Int(10, 60)),
                        createdBy: user,
                        categories: selectedCategories,
                        type: f.PickRandom(ActivityType.Classique, ActivityType.Ecriture, ActivityType.Playlist, ActivityType.Jeu)
                    );
                });

            return faker.Generate(count);
        }

        // Générer des catégories
        public static List<Category> GenerateCategories(int count)
        {
            var faker = new Faker("fr");
            var categories = new List<Category>();
            var generatedNames = new HashSet<string>();

            for (int i = 0; i < count; i++)
            {
                string name;
                do
                {
                    name = faker.Commerce.Categories(1).First();
                } while (generatedNames.Contains(name));

                var category = Category.Create(
                    name: name,
                    iconLink: faker.Image.PicsumUrl(200, 200, true)
                );

                generatedNames.Add(name);
                categories.Add(category);
            }

            return categories;
        }

        // Générer des menus
        public static async Task SeedMenusAsync(CesiZenDbContext context, CancellationToken cancellationToken)
        {
            if (context.Menus.Any())
                return;

            var faker = new Faker("fr");

            var parentMenus = Enumerable.Range(0, 3).Select(_ =>
                Menu.Create(
                    title: faker.Commerce.Categories(1).First(),
                    hierarchyLevel: 0
                )).ToList();

            await context.Menus.AddRangeAsync(parentMenus, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            var childMenus = parentMenus
                .SelectMany(parent => Enumerable.Range(0, 2).Select(_ =>
                    Menu.Create(
                        title: faker.Commerce.Categories(1).First(),
                        hierarchyLevel: 1,
                        parentId: parent.Id
                    )))
                .ToList();

            await context.Menus.AddRangeAsync(childMenus, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        // Générer des articles
        public static List<Article> GenerateArticles(List<Menu> menus, int count)
        {
            var faker = new Faker<Article>()
                .CustomInstantiator(f =>
                {
                    Menu selectedMenu = f.PickRandom(menus);
                    return Article.Create(
                        title: f.Lorem.Sentence(3),
                        content: f.Lorem.Paragraphs(4),
                        menu: selectedMenu
                    );
                });

            return faker.Generate(count);
        }

        // Générer des participations (Many-to-Many avec une date et une durée)
        public static List<Participation> GenerateParticipations(List<User> users, List<Activity> activities, int count)
        {
            var faker = new Faker<Participation>()
                .CustomInstantiator(f =>
                {
                    var user = f.PickRandom(users);
                    var activity = f.PickRandom(activities);
                    return Participation.Create(
                        user: user,
                        activity: activity,
                        date: f.Date.Past(1).ToUniversalTime(),
                        duration: TimeSpan.FromMinutes(f.Random.Int(30, 120))
                    );
                });

            return faker.Generate(count);
        }

        // Générer des activités sauvegardées (One-to-Many avec un utilisateur)
        public static List<SavedActivity> GenerateSavedActivities(List<User> users, List<Activity> activities, int count)
        {
            var faker = new Faker<SavedActivity>()
                .CustomInstantiator(f =>
                {
                    var user = f.PickRandom(users);
                    var activity = f.PickRandom(activities);
                    return SavedActivity.Create(
                        user: user,
                        activity: activity,
                        isFavoris: f.Random.Bool(),
                        state: f.PickRandom(SavedActivityStates.NoProgress, SavedActivityStates.InProgress, SavedActivityStates.Completed),
                        progress: new Percentage(f.Random.Int(0, 100) / 100)
                    );
                });

            return faker.Generate(count);
        }

        // Fonction de seeding
        public static async Task SeedAsync(CesiZenDbContext context, CancellationToken cancellationToken)
        {
            if (context.Users.Any() || context.Categories.Any() || context.Activities.Any())
                return;

            var users = GenerateUsers(10);  // 10 utilisateurs
            var categories = GenerateCategories(5);  // 5 catégories

            // Ajouter des utilisateurs et catégories
            await context.Set<User>().AddRangeAsync(users, cancellationToken);
            await context.Set<Category>().AddRangeAsync(categories, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            // Ajouter des activités
            var activities = GenerateActivities(users, categories, 20);  // 20 activités
            await context.Set<Activity>().AddRangeAsync(activities, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            // Ajouter des menus
            await SeedMenusAsync(context, cancellationToken);

            // Ajouter des articles
            var menus = await context.Set<Menu>().ToListAsync(cancellationToken);
            var articles = GenerateArticles(menus, 10);
            await context.Set<Article>().AddRangeAsync(articles, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            // Ajouter des participations
            var participations = GenerateParticipations(users, activities, 30);  // 30 participations
            await context.Set<Participation>().AddRangeAsync(participations, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            // Ajouter des activités sauvegardées
            var savedActivities = GenerateSavedActivities(users, activities, 10);  // 10 activités sauvegardées
            await context.Set<SavedActivity>().AddRangeAsync(savedActivities, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
