using CesiZen_Backend.Behaviors;
using CesiZen_Backend.Persistence;
using CesiZen_Backend.Services.ActivityService;
using CesiZen_Backend.Services.Articleservice;
using CesiZen_Backend.Services.ArticleService;
using CesiZen_Backend.Services.AuthService;
using CesiZen_Backend.Services.CategoryService;
using CesiZen_Backend.Services.MenuService;
using CesiZen_Backend.Services.ParticipationService;
using CesiZen_Backend.Services.SavedActivityService;
using CesiZen_Backend.Services.UserService;
using CesiZen_Backend.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using System.Reflection;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

try
{
    Log.Information("Starting up the application...");
    builder.Host.UseSerilog((context, loggerConfiguration) =>
    {
        loggerConfiguration.WriteTo.Console();
        loggerConfiguration.ReadFrom.Configuration(context.Configuration);
    });

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    builder.Services.AddDbContext<CesiZenDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddScoped<IActivityService, ActivityService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<ICategoryService, CategoryService>();
    builder.Services.AddScoped<IArticleService, ArticleService>();
    builder.Services.AddScoped<IMenuService, MenuService>();
    builder.Services.AddScoped<IParticipationService, ParticipationService>();
    builder.Services.AddScoped<ISavedActivityService, SavedActivityService>();
    builder.Services.AddScoped<IAuthService, AuthService>();

    builder.Services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    });

    builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();

    var app = builder.Build();

    await using (var serviceScope = app.Services.CreateAsyncScope())
    await using (var dbContext = serviceScope.ServiceProvider.GetRequiredService<CesiZenDbContext>())
    {
        await dbContext.Database.EnsureCreatedAsync();
    }

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(opt =>
        {
            opt.Title = "CesiZen";
            opt.Theme = ScalarTheme.BluePlanet;
        });
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.UseSerilogRequestLogging();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
