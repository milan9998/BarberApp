using Hair.Application.Common.Interfaces;
using Hair.Domain.Entities;
using Hair.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Hair.Infrastructure.Services;

public class AdminSeederService(
    UserManager<ApplicationUser> userManager,
    IHairDbContext dbContext) : IAdminSeederService
{
    public async Task SeedAdminAsync()
    {
        const string adminEmail = "admin@gmail.com";
        const string adminPassword = "Admin@123";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
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

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (!result.Succeeded)
            {
                throw new Exception("Admin kreiranje nije uspelo: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(adminUser, nameof(Role.Admin)))
        {
            await userManager.AddToRoleAsync(adminUser, nameof(Role.Admin));
        }
    }

    public async Task SeedDemoOwnerAsync()
    {
        const string ownerEmail = "owner.truefitt@demo.com";
        const string ownerPassword = "Owner@123";

        var company = await dbContext.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CompanyName == "Truefitt & Hill");

        if (company == null)
        {
            return;
        }

        var existingOwner = await userManager.FindByEmailAsync(ownerEmail);
        if (existingOwner == null)
        {
            existingOwner = new ApplicationUser
            {
                UserName = ownerEmail,
                Email = ownerEmail,
                EmailConfirmed = true,
                Role = Role.CompanyOwner,
                FirstName = "James",
                LastName = "Truefitt",
                PhoneNumber = "381601110001"
            };

            var result = await userManager.CreateAsync(existingOwner, ownerPassword);
            if (!result.Succeeded)
            {
                throw new Exception("Demo owner kreiranje nije uspelo: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            existingOwner.Role = Role.CompanyOwner;
            existingOwner.EmailConfirmed = true;
            await userManager.UpdateAsync(existingOwner);
        }

        if (!await userManager.IsInRoleAsync(existingOwner, nameof(Role.CompanyOwner)))
        {
            await userManager.AddToRoleAsync(existingOwner, nameof(Role.CompanyOwner));
        }

        var linkExists = await dbContext.ApplicationUserCompany
            .AnyAsync(x => x.ApplicationUserId == existingOwner.Id && x.CompanyId == company.Id);

        if (!linkExists)
        {
            await dbContext.ApplicationUserCompany.AddAsync(new ApplicationUserCompany
            {
                Id = Guid.NewGuid(),
                ApplicationUserId = existingOwner.Id,
                CompanyId = company.Id
            });
            await dbContext.SaveChangesAsync(CancellationToken.None);
        }
    }

    public async Task SeedDemoCrmAsync()
    {
        var company = await dbContext.Companies
            .FirstOrDefaultAsync(c => c.CompanyName == "Truefitt & Hill");

        if (company == null)
        {
            return;
        }

        var demoCustomers = new[]
        {
            ("crm.client1@demo.com", "Alex", "Carter", "381601110101"),
            ("crm.client2@demo.com", "Maya", "Brooks", "381601110102"),
            ("crm.client3@demo.com", "Noah", "Reed", "381601110103"),
            ("crm.client4@demo.com", "Lila", "Quinn", "381601110104"),
            ("crm.client5@demo.com", "Owen", "Hayes", "381601110105")
        };

        var customerIds = new List<string>();
        foreach (var (email, first, last, phone) in demoCustomers)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    Role = Role.RegisteredUser,
                    FirstName = first,
                    LastName = last,
                    PhoneNumber = phone
                };
                var created = await userManager.CreateAsync(user, "Client@123");
                if (!created.Succeeded)
                {
                    continue;
                }
            }

            customerIds.Add(user.Id);
        }

        if (customerIds.Count == 0)
        {
            return;
        }

        var barberSpecs = new[]
        {
            ("Victor Wells", "victor.wells@truefitt.demo", "381601220001", new TimeSpan(9, 0, 0), new TimeSpan(18, 0, 0)),
            ("Henry Clarke", "henry.clarke@truefitt.demo", "381601220002", new TimeSpan(9, 0, 0), new TimeSpan(18, 0, 0)),
            ("Arthur Lane", "arthur.lane@truefitt.demo", "381601220003", new TimeSpan(10, 0, 0), new TimeSpan(19, 0, 0)),
            ("Edward Moss", "edward.moss@truefitt.demo", "381601220004", new TimeSpan(8, 30, 0), new TimeSpan(17, 0, 0))
        };

        var barbers = await dbContext.Barbers
            .Where(b => b.Company != null && b.Company.Id == company.Id)
            .ToListAsync();

        foreach (var (name, email, phone, start, end) in barberSpecs)
        {
            if (barbers.Any(b => b.BarberName == name || b.Email == email))
            {
                continue;
            }

            var appUser = await userManager.FindByEmailAsync(email);
            if (appUser == null)
            {
                appUser = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    Role = Role.Barber,
                    FirstName = name.Split(' ')[0],
                    LastName = name.Split(' ').Last(),
                    PhoneNumber = phone
                };
                var created = await userManager.CreateAsync(appUser, "Barber@123");
                if (!created.Succeeded)
                {
                    continue;
                }

                if (!await userManager.IsInRoleAsync(appUser, nameof(Role.Barber)))
                {
                    await userManager.AddToRoleAsync(appUser, nameof(Role.Barber));
                }
            }

            var barber = new Barber(name, phone, email, start, end).AddBarberCompany(company);
            barber.SetApplicationUserId(appUser.Id);
            dbContext.Barbers.Add(barber);
            barbers.Add(barber);
        }

        await dbContext.SaveChangesAsync(CancellationToken.None);

        barbers = await dbContext.Barbers
            .Where(b => b.Company != null && b.Company.Id == company.Id)
            .ToListAsync();

        if (barbers.Count == 0)
        {
            return;
        }

        var barberIds = barbers.Select(b => b.BarberId).ToList();
        var existingCount = await dbContext.Appointments
            .CountAsync(a => barberIds.Contains(a.Barberid));

        if (existingCount >= 80)
        {
            return;
        }

        var services = new[]
        {
            "Heritage Cut",
            "Hot Towel Shave",
            "Beard Sculpture",
            "Executive Grooming",
            "Signature Finish"
        };

        var now = DateTime.UtcNow;
        var rnd = new Random(1805);
        var monthWeights = new[] { 8, 12, 18, 24, 28, 32 };
        var toAdd = new List<Appointment>();

        for (var monthOffset = 5; monthOffset >= 0; monthOffset--)
        {
            var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-monthOffset);
            var count = monthWeights[5 - monthOffset];
            for (var i = 0; i < count; i++)
            {
                var barber = barbers[rnd.Next(barbers.Count)];
                var day = 1 + rnd.Next(Math.Min(27, DateTime.DaysInMonth(monthStart.Year, monthStart.Month)));
                var hour = 9 + rnd.Next(8);
                var minute = rnd.Next(2) * 30;
                var time = new DateTime(monthStart.Year, monthStart.Month, day, hour, minute, 0, DateTimeKind.Utc);
                if (time > now.AddDays(-1) && monthOffset == 0)
                {
                    time = now.AddDays(-(1 + rnd.Next(10))).Date.AddHours(hour).AddMinutes(minute);
                }

                var customerId = customerIds[rnd.Next(customerIds.Count)];
                var appt = new Appointment(time, barber.BarberId)
                    .SetHaircutName(services[rnd.Next(services.Length)]);
                appt.ApplicationUserId = customerId;
                toAdd.Add(appt);
            }
        }

        // Upcoming appointments for strongest CRM screenshot
        for (var i = 1; i <= 28; i++)
        {
            var barber = barbers[i % barbers.Count];
            var time = now.Date.AddDays(i % 14).AddHours(10 + (i % 6)).AddMinutes((i % 2) * 30);
            var appt = new Appointment(DateTime.SpecifyKind(time, DateTimeKind.Utc), barber.BarberId)
                .SetHaircutName(services[i % services.Length]);
            appt.ApplicationUserId = customerIds[i % customerIds.Count];
            toAdd.Add(appt);
        }

        await dbContext.Appointments.AddRangeAsync(toAdd);
        await dbContext.SaveChangesAsync(CancellationToken.None);
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
