using Hair.Application.Common.Interfaces;
using Hair.Domain.Entities;
using Hair.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Hair.Infrastructure.Services;

public class AdminSeederService(UserManager<ApplicationUser> _userManager) : IAdminSeederService
{
    
    public async Task SeedAdminAsync()
    {
        var adminEmail = "admin@gmail.com";
        var adminPassword = "Admin@123";

        var adminUser = await _userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                Role = Role.Admin,
                FirstName = "Admin",
                LastName = "Admin",
            };

            var result = await _userManager.CreateAsync(adminUser, adminPassword);
            if (!result.Succeeded)
            {
                throw new Exception("Admin kreiranje nije uspelo: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
    
    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        foreach (var role in Enum.GetNames(typeof(Role)))
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

}