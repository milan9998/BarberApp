using Hair.Application.Common.Configuration;
using Hair.Application.Common.Interfaces;
using Hair.Infrastructure.Context;
using Hair.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hair.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));
        services.Configure<AppUrlSettings>(configuration.GetSection(AppUrlSettings.SectionName));

        services.AddScoped<IHairDbContext>(provider => provider.GetRequiredService<ConnDbContext>());

        services.AddScoped<IScheduleService, ScheduleService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<IBarberService, BarberService>();
        services.AddScoped<INotificationService, VonageNotificationService>();
        services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddScoped<IAdminSeederService, AdminSeederService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IOwnerService, OwnerService>();
        
        return services;
    }
}
