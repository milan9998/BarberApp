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
    IOptions<AppUrlSettings> appUrlOptions,
    IAppLocalizer localizer) : IAuthService
{
    private readonly AppUrlSettings _appUrls = appUrlOptions.Value;

    public async Task<AuthResponseDto> Login(LoginDto loginDto, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || user.Email != loginDto.Email)
            {
                throw new BadRequestException(localizer.T(
                    "Incorrect email or password.",
                    "Pogrešan email ili lozinka."));
            }

            if (!user.EmailConfirmed)
            {
                throw new BadRequestException(localizer.T(
                    "Email is not verified. Open your inbox, click the verification link, then log in.",
                    "Email nije verifikovan. Otvorite inbox, kliknite na verifikacioni link, pa se tek onda prijavite."));
            }
        
            var password = await userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!password)
            {
                throw new BadRequestException(localizer.T(
                    "Incorrect email or password.",
                    "Pogrešan email ili lozinka."));
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

            await userManager.AddToRoleAsync(appUser, nameof(Role.CompanyOwner));

            var appUserCompany = new ApplicationUserCompany()
            {
                CompanyId = companyOwnerDto.CompanyId,
                ApplicationUserId = appUser.Id,
                Id = Guid.NewGuid()
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
                throw new BadRequestException(localizer.T(
                    "Passwords do not match.",
                    "Lozinke se ne poklapaju."));

            var existingUser = await userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new BadRequestException(localizer.T(
                    "A user with this email already exists.",
                    "Korisnik sa ovim emailom već postoji."));

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
                var errorMsg = string.Join("\n", result.Errors.Select(LocalizeIdentityError));
                throw new BadRequestException(errorMsg);
            }

            var roleName = Enum.GetName(typeof(Role), user.Role);
            await userManager.AddToRoleAsync(user, roleName!);

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var verifyUrl =
                $"{_appUrls.ApiBaseUrl.TrimEnd('/')}/auth/verify-email?userId={Uri.EscapeDataString(user.Id)}&token={encodedToken}";

            var html = $"""
                <div style="margin:0;padding:0;background:#0a0a0a;font-family:Arial,Helvetica,sans-serif">
                  <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="background:#0a0a0a;padding:28px 12px">
                    <tr>
                      <td align="center">
                        <table role="presentation" width="560" cellspacing="0" cellpadding="0" style="max-width:560px;background:#111111;border:1px solid #2a2a2a">
                          <tr>
                            <td style="height:6px;background:#c8102e;font-size:0;line-height:0">&nbsp;</td>
                          </tr>
                          <tr>
                            <td style="padding:28px 28px 8px;color:#ffffff;font-size:12px;letter-spacing:0.16em;text-transform:uppercase;font-weight:700">
                              Barber Control Headquarters
                            </td>
                          </tr>
                          <tr>
                            <td style="padding:8px 28px 0;color:#ffffff;font-size:28px;line-height:1.2;font-weight:700">
                              Potvrdite nalog
                            </td>
                          </tr>
                          <tr>
                            <td style="padding:14px 28px 0;color:#cfcfcf;font-size:15px;line-height:1.65">
                              Zdravo {user.FirstName},
                              <br/><br/>
                              Hvala na registraciji. Otvorite dugme ispod da verifikujete email i otključate prijavu.
                            </td>
                          </tr>
                          <tr>
                            <td style="padding:24px 28px 8px" align="left">
                              <a href="{verifyUrl}"
                                 style="display:inline-block;padding:14px 22px;background:#c8102e;color:#ffffff;text-decoration:none;font-size:14px;font-weight:700;letter-spacing:0.08em;text-transform:uppercase">
                                Verifikuj email
                              </a>
                            </td>
                          </tr>
                          <tr>
                            <td style="padding:12px 28px 28px;color:#9a9a9a;font-size:12px;line-height:1.6">
                              Ako dugme ne radi, otvorite ovaj link:
                              <br/>
                              <a href="{verifyUrl}" style="color:#ffffff;word-break:break-all">{verifyUrl}</a>
                            </td>
                          </tr>
                        </table>
                      </td>
                    </tr>
                  </table>
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
                throw new BadRequestException(localizer.T(
                    "Account was created, but the verification email could not be sent. Check SMTP settings or contact an admin.",
                    "Nalog je kreiran, ali verifikacioni email nije poslat. Proverite SMTP podešavanja ili kontaktirajte admina."));
            }

            return new RegisterResponseDto(
                user.Email!,
                localizer.T(
                    "Account created. We sent you a verification email. Open your inbox, click the link, then log in.",
                    "Nalog je kreiran. Poslali smo vam email za verifikaciju. Otvorite inbox, kliknite na link, pa se tek onda prijavite."),
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
            throw new Exception(localizer.T(
                "User was not found.",
                "Korisnik nije pronađen."));
        }

        if (user.EmailConfirmed)
        {
            return localizer.T(
                "Email is already verified.",
                "Email je već verifikovan.");
        }

        string decodedToken;
        try
        {
            decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        }
        catch
        {
            throw new Exception(localizer.T(
                "Invalid verification token.",
                "Neispravan verifikacioni token."));
        }

        var result = await userManager.ConfirmEmailAsync(user, decodedToken);
        if (!result.Succeeded)
        {
            var errorMsg = string.Join("\n", result.Errors.Select(LocalizeIdentityError));
            throw new Exception(localizer.T(
                $"Verification failed: {errorMsg}",
                $"Verifikacija nije uspela: {errorMsg}"));
        }

        return localizer.T(
            "Email verified successfully. You can log in.",
            "Email je uspešno verifikovan. Možete se prijaviti.");
    }

    private string LocalizeIdentityError(IdentityError error) => error.Code switch
    {
        "PasswordTooShort" => localizer.T(
            "Password must be at least 6 characters.",
            "Lozinka mora imati najmanje 6 karaktera."),
        "PasswordRequiresDigit" => localizer.T(
            "Password must have at least one digit (0-9).",
            "Lozinka mora imati najmanje jednu cifru (0-9)."),
        "PasswordRequiresUpper" => localizer.T(
            "Password must have at least one uppercase letter (A-Z).",
            "Lozinka mora imati najmanje jedno veliko slovo (A-Z)."),
        "PasswordRequiresLower" => localizer.T(
            "Password must have at least one lowercase letter (a-z).",
            "Lozinka mora imati najmanje jedno malo slovo (a-z)."),
        "PasswordRequiresNonAlphanumeric" => localizer.T(
            "Password must have at least one special character (!@#$...).",
            "Lozinka mora imati najmanje jedan specijalan karakter (!@#$...)."),
        "DuplicateUserName" or "DuplicateEmail" => localizer.T(
            "A user with this email already exists.",
            "Korisnik sa ovim emailom već postoji."),
        _ => error.Description
    };

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