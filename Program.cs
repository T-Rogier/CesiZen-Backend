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
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

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

app.MapControllers();

app.Run();
