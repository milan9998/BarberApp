using EnumsNET;
using Hair.Application.Common.Configuration;
using Hair.Application.Common.Dto.Auth;
using Hair.Application.Common.Dto.Company;
using Hair.Application.Common.Exceptions;
using Hair.Application.Common.Interfaces;
using Hair.Domain.Entities;
using Hair.Domain.Enums;
using Hair.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;

namespace Hair.Infrastructure.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IHairDbContext dbContext,
    ILogger<AuthService> _logger,
    IEmailService emailService,
    IOptions<AppUrlSettings> appUrlOptions) : IAuthService
{
    private readonly AppUrlSettings _appUrls = appUrlOptions.Value;

    public async Task<AuthResponseDto> Login(LoginDto loginDto, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || user.Email != loginDto.Email)
            {
                throw new BadRequestException("Pogrešan email ili lozinka.");
            }

            if (!user.EmailConfirmed)
            {
                throw new BadRequestException(
                    "Email nije verifikovan. Otvorite inbox, kliknite na verifikacioni link, pa se tek onda prijavite.");
            }
        
            var password = await userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!password)
            {
                throw new BadRequestException("Pogrešan email ili lozinka.");
            }
            var barberId = await dbContext.Barbers.FirstOrDefaultAsync(x=>x.ApplicationUserId == user.Id, cancellationToken);
        
            var ownersCompanies = await dbContext.ApplicationUserCompany
                .Where(i => i.ApplicationUserId == user.Id)
                .Select(c => c.CompanyId).ToListAsync();
        
            var roleName = Enum.GetName(typeof(Role), user.Role);
            var result = await signInManager.PasswordSignInAsync(user, loginDto.Password, isPersistent: false, lockoutOnFailure: false);
            return new AuthResponseDto(user.Id, user.FirstName, user.LastName, user.Email,user.PhoneNumber,
                roleName, ownersCompanies, barberId?.BarberId);
        }
        catch (BadRequestException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Login failed");
            throw;
        }
        
    }

    public async Task<AssignCompanyOwnerDto> AssignCompanyOwnerAsync(AssignCompanyOwnerDto assignCompanyOwnerDto,
        CancellationToken cancellationToken)
    {
        try
        {
            var appUserCompany = new ApplicationUserCompany()
            {
                CompanyId = assignCompanyOwnerDto.CompanyId,
                ApplicationUserId = assignCompanyOwnerDto.ApplicationUserId.ToString(),
                Id = new Guid()
            };
            await dbContext.ApplicationUserCompany.AddAsync(appUserCompany, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Detaljan opis gde i šta se desilo u AuthService");
            throw new Exception("Unable to assign company to user", ex);
        }

        return assignCompanyOwnerDto;
    }

    public async Task<List<CompanyDetailsDto>> GetCompaniesByOwnerEmailAsync(string email, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new Exception($"Nije pronadjen email {email} vlasnika!");
            }
            var userId = user.Id;
            
            var companies = await dbContext.ApplicationUserCompany.Where(i=> i.ApplicationUserId == userId)
                .Select(i=>i.Company).ToListAsync();
            
            var response = companies.Select(x=> new CompanyDetailsDto
            {
                CompanyId = x.Id,
                CompanyName = x.CompanyName,
                ImageUrl = x.ImageUrl,
            }).ToList();
            
            return response;
        }
        catch (Exception e)
        {
            
            throw new Exception($"Greška pri dohvatanju kompanija vlasnika sa email-om: {email}!", e);
        }
    }

    public async Task<CompanyOwnerResponseDto> CreateCompanyOwnerAsync(CompanyOwnerDto companyOwnerDto,
        CancellationToken cancellationToken)
    {
        try
        {
           
            var companyExistcheck = await dbContext.ApplicationUserCompany
                .Where(x => x.CompanyId == companyOwnerDto.CompanyId).FirstOrDefaultAsync();
           
            var companyOwnerExistCheck = await userManager.FindByEmailAsync(companyOwnerDto.Email);

            //provera da li taj user sto bi trebao da bude vlasnik postoji sa tim id-jem u medju tabeli 
            /* var ownerExists = await dbContext.ApplicationUserCompany.
                 Where(x => x.ApplicationUserId == companyOwnerExistCheck.Id).FirstOrDefaultAsync();*/

            if (companyOwnerExistCheck != null && companyOwnerExistCheck.Role == Role.CompanyOwner)
            {
                throw new Exception($"Company Owner {companyOwnerDto.Email} already exists");
            }
            

            var appUser = new ApplicationUser()
            {
                UserName = companyOwnerDto.Email,
                Email = companyOwnerDto.Email,
                PhoneNumber = companyOwnerDto.PhoneNumber,
                FirstName = companyOwnerDto.FirstName,
                LastName = companyOwnerDto.LastName,
                Role = Role.CompanyOwner,
                EmailConfirmed = true
            };
            
            var result = await userManager.CreateAsync(appUser, companyOwnerDto.Password);

            if (!result.Succeeded)
            {
                var errorMsg = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception(errorMsg);
            }

            //company.CompanyOwnerId = appUser.Id;
            //      company.CompanyOwnerId = comapnyExistcheck.CompanyId.ToString();
            var appUserCompany = new ApplicationUserCompany()
            {
                CompanyId = companyOwnerDto.CompanyId,
                ApplicationUserId = appUser.Id,
                Id = new Guid()
            };
            await dbContext.ApplicationUserCompany.AddAsync(appUserCompany, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);


            return new CompanyOwnerResponseDto
            (appUser.Email,
                //appUser.CompanyId, 
                appUser.FirstName,
                appUser.LastName,
                appUser.PhoneNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Detaljan opis gde i šta se desilo u AuthService");
            throw new Exception($"Greška prilikom kreiranja vlasnika: {ex.Message}");
        }
    }
    

    public async Task<bool> CheckIfCompanyOwnerExistsAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var exists = await dbContext.ApplicationUserCompany
            .Where(u => u.CompanyId == companyId)
            .FirstOrDefaultAsync(cancellationToken);


        if (exists != null)
        {
            return true;
            //hrow new Exception($"Company {companyId} already exists");
        }

        return false;
    }
    public async Task<List<GetAllAppointmentsByUserIdDto>> GetAllAppointmentsByUserIdAsync(string userId, CancellationToken cancellationToken)
    {
        var appointments = await dbContext.Appointments.Where(x => x.ApplicationUserId == userId)
            .ToListAsync(cancellationToken);

        var response = appointments.Select(x => new GetAllAppointmentsByUserIdDto(
            AppointmentId: x.Id,
            Time: x.Time,
            HaircutName: x.HaircutName,
            BarberName: dbContext.Barbers.Where(b => b.BarberId == x.Barberid).FirstOrDefault().BarberName,
            BarberPhone: dbContext.Barbers.Where(b => b.BarberId == x.Barberid).FirstOrDefault().PhoneNumber,
            BarberEmail: dbContext.Barbers.Where(b => b.BarberId == x.Barberid).FirstOrDefault().Email
        )).ToList();

        return response;
    }
    public async Task<RegisterResponseDto> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken)
    {
        try
        {
            if (dto.Password != dto.ConfirmPassword)
                throw new BadRequestException("Lozinke se ne poklapaju.");

            var existingUser = await userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new BadRequestException("Korisnik sa ovim emailom već postoji.");

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Role = Role.RegisteredUser,
                EmailConfirmed = false
            };

            var result = await userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errorMsg = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BadRequestException(errorMsg);
            }

            var roleName = Enum.GetName(typeof(Role), user.Role);
            await userManager.AddToRoleAsync(user, roleName!);

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var verifyUrl =
                $"{_appUrls.ApiBaseUrl.TrimEnd('/')}/auth/verify-email?userId={Uri.EscapeDataString(user.Id)}&token={encodedToken}";

            var html = $"""
                <div style="font-family:Arial,sans-serif;line-height:1.5;color:#1d2a3f">
                  <h2>Potvrdite nalog</h2>
                  <p>Zdravo {user.FirstName},</p>
                  <p>Hvala na registraciji na Barber Control HQ. Kliknite na dugme ispod da verifikujete email.</p>
                  <p><a href="{verifyUrl}" style="display:inline-block;padding:12px 18px;background:#1f4f96;color:#fff;text-decoration:none;border-radius:8px">Verifikuj email</a></p>
                  <p>Ili otvorite ovaj link:<br/><a href="{verifyUrl}">{verifyUrl}</a></p>
                </div>
                """;

            try
            {
                await emailService.SendAsync(
                    user.Email!,
                    "Verifikacija naloga - Barber Control HQ",
                    html,
                    cancellationToken);
            }
            catch (Exception emailEx)
            {
                _logger.LogError(emailEx, "Verification email failed for {Email}", user.Email);
                throw new BadRequestException(
                    "Nalog je kreiran, ali verifikacioni email nije poslat. Proverite SMTP podešavanja ili kontaktirajte admina.");
            }

            return new RegisterResponseDto(
                user.Email!,
                "Nalog je kreiran. Poslali smo vam email za verifikaciju. Otvorite inbox, kliknite na link, pa se tek onda prijavite.",
                true);
        }
        catch (BadRequestException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Registration failed");
            throw;
        }
    }

    public async Task<string> VerifyEmailAsync(string userId, string token, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            throw new Exception("Korisnik nije pronađen.");
        }

        if (user.EmailConfirmed)
        {
            return "Email je već verifikovan.";
        }

        string decodedToken;
        try
        {
            decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        }
        catch
        {
            throw new Exception("Neispravan verifikacioni token.");
        }

        var result = await userManager.ConfirmEmailAsync(user, decodedToken);
        if (!result.Succeeded)
        {
            var errorMsg = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Verifikacija nije uspela: {errorMsg}");
        }

        return "Email je uspešno verifikovan. Možete se prijaviti.";
    }

    public async Task<string> UpdateCompanyOwnerAsync(UpdateOwnerDto updateOwnerDto, CancellationToken cancellationToken)
    {
        try
        {
            var ownerToUpdate = await userManager.FindByIdAsync(updateOwnerDto.OwnerId);
            ownerToUpdate.FirstName = updateOwnerDto.FirstName;
            ownerToUpdate.LastName = updateOwnerDto.LastName;
            ownerToUpdate.UserName = updateOwnerDto.Email;
            ownerToUpdate.NormalizedUserName = updateOwnerDto.Email.ToUpper();
            ownerToUpdate.NormalizedEmail = updateOwnerDto.Email.ToUpper();
            ownerToUpdate.Email = updateOwnerDto.Email;
            ownerToUpdate.PhoneNumber = updateOwnerDto.PhoneNumber;
            
            await userManager.UpdateAsync(ownerToUpdate);
            return "Uspešno izmenjen vlasnik!";
            
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Detaljan opis gde i šta se desilo u AuthService");
            throw new Exception("Greška pri izmeni detalja vlasnika!");
        }
    }


  
}