using EnumsNET;
using Hair.Application.Common.Dto.Auth;
using Hair.Application.Common.Interfaces;
using Hair.Domain.Entities;
using Hair.Domain.Enums;
using Hair.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;

namespace Hair.Infrastructure.Services;

public class AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IHairDbContext dbContext) : IAuthService
{
    public async Task<AuthLevelDto> Login(LoginDto loginDto, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(loginDto.Email);
        if (user == null || user.Email != loginDto.Email)
        {
            throw new Exception("Invalid email address");
        }
        
        var password = await userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!password)
        {
            throw new Exception("Invalid password");
        }

       /* if (user.Role != Role.Admin)
        {
            throw new Exception("Invalid role");
        }*/
        var roleName = Enum.GetName(typeof(Role), user.Role);
        var result = await signInManager.PasswordSignInAsync(user, loginDto.Password, isPersistent: false, lockoutOnFailure: false);
        return new AuthLevelDto(user.Email,roleName);
    }

    public async Task<CompanyOwnerResponseDto> CreateCompanyOwnerAsync(CompanyOwnerDto companyOwnerDto,
        CancellationToken cancellationToken)
    {
        var appUser = new ApplicationUser()
        {
            UserName = companyOwnerDto.Email,
            Email = companyOwnerDto.Email,
            PhoneNumber = companyOwnerDto.PhoneNumber,
            FirstName = companyOwnerDto.FirstName,
            LastName = companyOwnerDto.LastName,
            CompanyId = companyOwnerDto.CompanyId,
            Role = Role.CompanyOwner

        };
        var result = await userManager.CreateAsync(appUser, companyOwnerDto.Password);
        
        if (!result.Succeeded)
        {
            var errorMsg = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception(errorMsg);
        }
        return new CompanyOwnerResponseDto
        (appUser.Email,
            appUser.CompanyId, 
            appUser.FirstName, 
            appUser.LastName,
            appUser.PhoneNumber);
    }

    public async Task<AuthLevelDto> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken)
    {
        if (dto.Password != dto.ConfirmPassword)
            throw new Exception("Passwords do not match");

        var existingUser = await userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            throw new Exception("User with this email already exists");

        var user = new ApplicationUser
        {
            
            UserName = dto.Email,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Role = Role.RegisteredUser 
        };

        var result = await userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var errorMsg = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception(errorMsg);
        }

        
        var roleName = Enum.GetName(typeof(Role), user.Role);
        await userManager.AddToRoleAsync(user, roleName);

        
        await signInManager.SignInAsync(user, isPersistent: false);

        return new AuthLevelDto(user.Email, roleName);
    }
   
    
    /*
     * public async Task<bool> RegisterOwnerAsync(RegisterDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null) return false;

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            EmailConfirmed = true,
            Role = UserRole.Owner
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded) return false;

        var company = new Company
        {
            CompanyName = dto.CompanyName,
            OwnerId = user.Id
        };

        _dbContext.Companies.Add(company);
        await _dbContext.SaveChangesAsync();

        return true;
    }
     */
}