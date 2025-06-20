using Hair.Application.Common.Interfaces;
using Hair.Infrastructure.Configuration;
using Hair.Infrastructure.Context;
using Hair.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hair.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
       // var dbConfiguration = new PostgresDbConfiguration();
    //    configuration.GetSection("PostgresDbConfiguration").Bind(dbConfiguration);
       // services.AddDbContext<ConnDbContext>(options=>options.UseNpgsql(dbConfiguration.ConnectionString,
       //     x=>x.MigrationsAssembly(typeof(ConnDbContext).Assembly.GetName().Name)));
        services.AddScoped<IHairDbContext>(provider => provider.GetRequiredService<ConnDbContext>());

        services.AddScoped<IScheduleService, ScheduleService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<IBarberService, BarberService>();
        services.AddScoped<INotificationService, VonageNotificationService>();
        
        
        return services;
    }
}