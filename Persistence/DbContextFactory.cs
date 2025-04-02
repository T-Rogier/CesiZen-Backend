using CesiZen_Backend.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

public class CesiZenDbContextFactory : IDesignTimeDbContextFactory<CesiZenDbContext>
{
    public CesiZenDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<CesiZenDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new CesiZenDbContext(optionsBuilder.Options);
    }
}
