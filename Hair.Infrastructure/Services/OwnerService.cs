using Hair.Application.Common.Dto.Company;
using Hair.Application.Common.Interfaces;
using Hair.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Hair.Infrastructure.Services;

public class OwnerService(IHairDbContext dbContext) : IOwnerService
{
    public async Task<string> CreateHaircutByOwner(HaircutDto haircutDto, CancellationToken cancellationToken)
    {
        try
        {
            var newHaircut = new Haircut(haircutDto.Duration,haircutDto.HaircutType,haircutDto.Price);

            var company = await dbContext.Companies.Where(x => x.Id == haircutDto.CompanyId).FirstOrDefaultAsync();
            newHaircut.AddCompanyCompany(company);
            dbContext.Haircuts.Add(newHaircut);
            await dbContext.SaveChangesAsync(cancellationToken);
            return "Successfully created haircut";
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("Failed to create haircut", e);
        }
       
    }
}