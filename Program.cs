using CesiZen_Backend.Common.Converter;
using CesiZen_Backend.Common.Options;
using CesiZen_Backend.Persistence;
using CesiZen_Backend.Services.ActivityService;
using CesiZen_Backend.Services.Articleservice;
using CesiZen_Backend.Services.ArticleService;
using CesiZen_Backend.Services.AuthService;
using CesiZen_Backend.Services.CategoryService;
using CesiZen_Backend.Services.MenuService;
using CesiZen_Backend.Services.PasswordHandler;
using CesiZen_Backend.Services.UserService;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using System.Reflection;
using System.Security.Claims;
using System.Text;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

try
{
    #region Logging Configuration

    Log.Information("Starting up the application...");
    builder.Host.UseSerilog((context, loggerConfiguration) =>
    {
        loggerConfiguration.WriteTo.Console();
        loggerConfiguration.ReadFrom.Configuration(context.Configuration);
    });

    #endregion

    #region Configuration and Services

    builder.Services.AddControllers();
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    builder.Services.AddDbContext<CesiZenDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddHttpContextAccessor();

    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
    builder.Services.AddScoped<IActivityService, ActivityService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<ICategoryService, CategoryService>();
    builder.Services.AddScoped<IArticleService, ArticleService>();
    builder.Services.AddScoped<IMenuService, MenuService>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();

    #endregion

    #region Authentication and Authorization

    builder.Services.Configure<JwtOptions>(
        builder.Configuration.GetSection("Jwt")
    );
    JwtOptions jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,

            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),

            RoleClaimType = ClaimTypes.Role,

            ClockSkew = TimeSpan.Zero
        };
    });

    builder.Services.AddAuthorization();

    #endregion

    #region Converters and Validators

    builder.Services.Configure<JsonOptions>(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new UserRoleDisplayNameConverter());
        options.JsonSerializerOptions.Converters.Add(new ActivityTypeDisplayNameConverter());
        options.JsonSerializerOptions.Converters.Add(new SavedActivityStatesDisplayNameConverter());
    });

    builder.Services.AddFluentValidationAutoValidation();

    builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

    #endregion

    #region Application Configuration

    WebApplication app = builder.Build();

    await using (AsyncServiceScope serviceScope = app.Services.CreateAsyncScope())
    await using (CesiZenDbContext dbContext = serviceScope.ServiceProvider.GetRequiredService<CesiZenDbContext>())
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

    //app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseSerilogRequestLogging();

    app.MapControllers();

    #endregion

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
