using System.Globalization;
using System.Text.RegularExpressions;
using Hair.Application.Common.Dto.Barber;
using Hair.Application.Common.Interfaces;
using Hair.Application.Common.Mappers;
using Hair.Domain.Entities;
using Hair.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Hair.Infrastructure.Services;

public class BarberService (IHairDbContext dbContext,UserManager<ApplicationUser> userManager) : IBarberService
{
    public async Task<BarberResponseDto> BarberCreateAsync(BarberCreateDto barberCreateDto, CancellationToken cancellationToken)
    {
        var company = await dbContext.Companies.Where(x => x.Id == barberCreateDto.companyId).FirstOrDefaultAsync(cancellationToken);
        
        var applicationUser = new ApplicationUser
        {
            UserName = barberCreateDto.email,
            Email = barberCreateDto.email,
            PhoneNumber = barberCreateDto.phoneNumber,
            FirstName = barberCreateDto.barberName,
            LastName = barberCreateDto.barberName,
            //CompanyId = barberCreateDto.companyId,
            Role = Role.Barber
        };
        var result = await userManager.CreateAsync(applicationUser, barberCreateDto.password);
        if (!result.Succeeded)
        {
            throw new Exception("Failed to create identity user: " + 
                                string.Join(", ", result.Errors.Select(e => e.Description)));
        }


        Barber barber = new Barber(

            barberCreateDto.barberName,
            barberCreateDto.phoneNumber,
            barberCreateDto.email,
            barberCreateDto.individualStartTime,
            barberCreateDto.individualEndTime);
       //     ).AddBarberCompany(company);
        
        
    //    barber.SetApplicationUserId(applicationUser.Id);

        if (!IsValidEmail(barberCreateDto.email))
        {
            throw new Exception("Invalid email address");
        }

        if (!IsValidSerbianPhoneNumber(barberCreateDto.phoneNumber))
        {
            throw new Exception("Invalid phone number");
        }
     //   dbContext.Barbers.Add(barber);
        var barberSaved = barberCreateDto.FromCreateDtoToEntityBarber().AddBarberCompany(company);
        barberSaved.SetApplicationUserId(applicationUser.Id);
        dbContext.Barbers.Add(barberSaved);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new BarberResponseDto(barber.BarberId,barber.BarberName, barber.PhoneNumber, barber.Email, barber.IndividualStartTime, barber.IndividualEndTime);
    }
    public async Task<string> DeleteBarberAsync(Guid barberId, CancellationToken cancellationToken)
    {
        try
        {
            var barberToDelete = await dbContext.Barbers.Where(x => x.BarberId == barberId)
                .FirstOrDefaultAsync(cancellationToken);
            if (barberToDelete == null)
            {
                throw new Exception($"Frizer sa id-jem {barberId} nije pronadjen!");
            }
         

            barberToDelete.UnsetCompany();
            
            await dbContext.SaveChangesAsync(cancellationToken);

            return "Uspešno obrisan frizer!";
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
    
    public async Task<string> UpdateBarberAsync(UpdateBarberDto updateBarberDto, CancellationToken cancellationToken)
    {
        try
        {
            var barber = await dbContext.Barbers.Where(x => x.BarberId == updateBarberDto.BarberId)
                .FirstOrDefaultAsync(cancellationToken);

            if (barber == null)
            {
                throw new Exception($"Barber with id {updateBarberDto.BarberId} was not found");
            }
            var appBarberUser = await userManager.FindByIdAsync(barber.ApplicationUserId);
            if (appBarberUser == null)
            { 
                throw new Exception($"Frizer sa id-jem {barber.ApplicationUserId} nije pronadjen!");
            }
        
            barber.UpdateBarberName(updateBarberDto.BarberName);
            barber.UpdateBarberPhoneNumber(updateBarberDto.PhoneNumber);
            barber.UpdateBarberEmail(updateBarberDto.Email);
            barber.UpdateIndividualStartTime(updateBarberDto.IndividualStartTime);
            barber.UpdateIndividualEndTime(updateBarberDto.IndividualEndTime);

            appBarberUser.FirstName = updateBarberDto.BarberName;
            appBarberUser.LastName = updateBarberDto.BarberName;
            appBarberUser.PhoneNumber = updateBarberDto.PhoneNumber;
            appBarberUser.Email = updateBarberDto.Email;
            appBarberUser.NormalizedEmail = updateBarberDto.Email.ToUpper();
            appBarberUser.NormalizedUserName = updateBarberDto.Email.ToUpper();
            appBarberUser.UserName = updateBarberDto.Email;
        
            await dbContext.SaveChangesAsync(cancellationToken);
            await userManager.UpdateAsync(appBarberUser);
        
            return "Frizer uspešno izmenjen!";
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
    

    public async Task<List<BarberDetailsDto>> GetAllBarbersAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var barbers = await dbContext.Barbers.Where(x => x.Company.Id == companyId)
            .Select(x=> new BarberDetailsDto(
                x.BarberId,
                x.BarberName,
                x.Company.CompanyName,
                x.PhoneNumber,
                x.Email,
                x.Company.Id,
                x.IndividualStartTime,
                x.IndividualEndTime))
            .ToListAsync();

        return barbers;
    }
    
    public bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;
        try
        {
            
            email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                RegexOptions.None, TimeSpan.FromMilliseconds(200));
            string DomainMapper(Match match)
            {
                var idn = new IdnMapping();
                string domainName = idn.GetAscii(match.Groups[2].Value);
                return match.Groups[1].Value + domainName;
            }
        }
        catch (RegexMatchTimeoutException e)
        {
            return false;
        }
        catch (ArgumentException e)
        {
            return false;
        }
        try
        {
            return Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.(com|net|org|gov|rs|ac.rs)$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }
    
    public bool IsValidSerbianPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;
        
        string pattern = @"^\+?381\s?(6\d{1})\s?\d{6,7}$";
        return Regex.IsMatch(phoneNumber, pattern);
    }
}