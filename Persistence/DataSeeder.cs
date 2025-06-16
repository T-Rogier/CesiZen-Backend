using Bogus;
using CesiZen_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CesiZen_Backend.Persistence
{
    public static class DataSeeder
    {
        public static List<User> GenerateUsers(int count)
        {
            Faker<User> faker = new Faker<User>()
                .CustomInstantiator(f =>
                {
                    return User.Create(
                        username: f.Internet.UserName(),
                        email: f.Internet.Email(),
                        password: BCrypt.Net.BCrypt.HashPassword("test"),
                        disabled: false,
                        role: f.PickRandom(UserRole.User, UserRole.Admin)
                    );
                });

            return faker.Generate(count);
        }

        public static List<Activity> GenerateActivities(List<User> users, List<Category> categories, int count)
        {
            Faker<Activity> faker = new Faker<Activity>()
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
                        type: f.PickRandom(ActivityType.Classic, ActivityType.Writting, ActivityType.Playlist, ActivityType.Game)
                    );
                });

            return faker.Generate(count);
        }

        public static List<Category> GenerateCategories(int count)
        {
            Faker faker = new("fr");
            List<Category> categories = [];
            HashSet<string> generatedNames = [];

            for (int i = 0; i < count; i++)
            {
                string name;
                do
                {
                    name = faker.Commerce.Categories(1).First();
                } while (generatedNames.Contains(name));

                Category category = Category.Create(
                    name: name,
                    iconLink: faker.Image.PicsumUrl(200, 200, true)
                );

                generatedNames.Add(name);
                categories.Add(category);
            }

            return categories;
        }

        public static async Task SeedMenusAsync(CesiZenDbContext context, CancellationToken cancellationToken)
        {
            if (context.Menus.Any())
                return;

            Faker faker = new("fr");

            List<Menu> parentMenus = Enumerable.Range(0, 3).Select(_ =>
                Menu.Create(
                    title: faker.Commerce.Categories(1).First(),
                    hierarchyLevel: 0
                )).ToList();

            await context.Menus.AddRangeAsync(parentMenus, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            List<Menu> childMenus = parentMenus
                .SelectMany(parent => Enumerable.Range(0, 2).Select(_ =>
                    Menu.Create(
                        title: faker.Commerce.Categories(1).First(),
                        hierarchyLevel: 1,
                        parentId: parent.Id
                    ))).ToList();

            await context.Menus.AddRangeAsync(childMenus, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public static List<Article> GenerateArticles(List<Menu> menus, int count)
        {
            Faker<Article> faker = new Faker<Article>()
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

        public static List<Participation> GenerateParticipations(List<User> users, List<Activity> activities, int count)
        {
            Faker<Participation> faker = new Faker<Participation>()
                .CustomInstantiator(f =>
                {
                    User user = f.PickRandom(users);
                    Activity activity = f.PickRandom(activities);
                    return Participation.Create(
                        user: user,
                        activity: activity,
                        date: f.Date.Past(1).ToUniversalTime(),
                        duration: TimeSpan.FromMinutes(f.Random.Int(30, 120))
                    );
                });

            return faker.Generate(count);
        }

        public static List<SavedActivity> GenerateSavedActivities(List<User> users, List<Activity> activities, int count)
        {
            Faker<SavedActivity> faker = new Faker<SavedActivity>()
                .CustomInstantiator(f =>
                {
                    User user = f.PickRandom(users);
                    Activity activity = f.PickRandom(activities);
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

        public static async Task SeedAsync(CesiZenDbContext context, CancellationToken cancellationToken)
        {
            if (context.Users.Any() || context.Categories.Any() || context.Activities.Any())
                return;

            List<User> users = GenerateUsers(10);
            List<Category> categories = GenerateCategories(5);

            await context.Set<User>().AddRangeAsync(users, cancellationToken);
            await context.Set<Category>().AddRangeAsync(categories, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            List<Activity> activities = GenerateActivities(users, categories, 20);
            await context.Set<Activity>().AddRangeAsync(activities, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            await SeedMenusAsync(context, cancellationToken);

            List<Menu> menus = await context.Set<Menu>().ToListAsync(cancellationToken);
            List<Article> articles = GenerateArticles(menus, 10);
            await context.Set<Article>().AddRangeAsync(articles, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            List<Participation> participations = GenerateParticipations(users, activities, 30);
            await context.Set<Participation>().AddRangeAsync(participations, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            List<SavedActivity> savedActivities = GenerateSavedActivities(users, activities, 10);
            List<SavedActivity> deduplicated = savedActivities
              .GroupBy(sa => new { sa.UserId, sa.ActivityId })
              .Select(g => g.First())
              .ToList();
            await context.Set<SavedActivity>().AddRangeAsync(deduplicated, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
