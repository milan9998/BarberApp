using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Hair.Infrastructure.Context;

public class ConnDbContextFactory : IDesignTimeDbContextFactory<ConnDbContext>
{
    public ConnDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../Hair.API");

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<ConnDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new ConnDbContext(optionsBuilder.Options);
    }
}