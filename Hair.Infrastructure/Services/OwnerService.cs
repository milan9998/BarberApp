using Hair.Application.Common.Dto.Company;
using Hair.Application.Common.Interfaces;
using Hair.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hair.Infrastructure.Services;

public class OwnerService(IHairDbContext dbContext) : IOwnerService
{
    public async Task<HaircutDto> CreateHaircutByOwner(HaircutDto haircutDto, CancellationToken cancellationToken)
    {
       var newHaircut = new Haircut(haircutDto.Duration,haircutDto.HaircutType,haircutDto.Price);

       var company = await dbContext.Companies.Where(x => x.Id == haircutDto.CompanyId).FirstOrDefaultAsync();
       newHaircut.AddCompanyCompany(company);
       dbContext.Haircuts.Add(newHaircut);
       await dbContext.SaveChangesAsync(cancellationToken);
       return new HaircutDto(haircutDto.HaircutId, haircutDto.HaircutType, haircutDto.Price, haircutDto.Duration, haircutDto.CompanyId);
    }
}