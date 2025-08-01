﻿using EnumsNET;
using Hair.Application.Common.Dto.Auth;
using Hair.Application.Common.Dto.Company;
using Hair.Application.Common.Interfaces;
using Hair.Domain.Entities;
using Hair.Domain.Enums;
using Hair.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hair.Infrastructure.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IHairDbContext dbContext, ILogger<AuthService> _logger) : IAuthService
{
    public async Task<AuthResponseDto> Login(LoginDto loginDto, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || user.Email != loginDto.Email)
            {
                throw new Exception("Pogrešno uneti podaci.");
            }
        
            var password = await userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!password)
            {
                throw new Exception("Pogrešno uneti podaci.");
            }
            var barberId = await dbContext.Barbers.FirstOrDefaultAsync(x=>x.ApplicationUserId == user.Id, cancellationToken);
        
            var ownersCompanies = await dbContext.ApplicationUserCompany
                .Where(i => i.ApplicationUserId == user.Id)
                .Select(c => c.CompanyId).ToListAsync();
        
        
            //var allOwnersCompanies = await dbContext.Companies.ToListAsync(cancellationToken);

            var roleName = Enum.GetName(typeof(Role), user.Role);
            var result = await signInManager.PasswordSignInAsync(user, loginDto.Password, isPersistent: false, lockoutOnFailure: false);
            return new AuthResponseDto(user.Id, user.FirstName, user.LastName, user.Email,user.PhoneNumber,
                roleName, ownersCompanies, barberId?.BarberId);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            Console.WriteLine(e);
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
                Role = Role.CompanyOwner
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
    public async Task<AuthLevelDto> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken)
    {
        try
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
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw new Exception(e.Message);
        }
        
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