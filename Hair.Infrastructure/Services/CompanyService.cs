using Hair.Application.Common.Dto.Barber;
using Hair.Application.Common.Dto.Company;
using Hair.Application.Common.Interfaces;
using Hair.Application.Common.Mappers;
using Hair.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hair.Infrastructure.Services;

public class CompanyService (IHairDbContext dbContext) : ICompanyService
{
    public async Task<CompanyCreateDto> CreateCompanyAsync(CompanyCreateDto companyCreate,
        CancellationToken cancellationToken)
    {
        var x = await dbContext.Companies.Where(c => c.CompanyName == companyCreate.CompanyName)
            .FirstOrDefaultAsync();
        if (x is  not null)
        {
            throw new Exception($"Company {companyCreate.CompanyName} already exists");
        }
        
     
        Company company = new Company(companyCreate.CompanyName);
       
        
        
        var companySaved = companyCreate.FromCreateDtoToEntity();
        dbContext.Companies.Add(companySaved);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CompanyCreateDto(company.CompanyName);
    }

    public async Task<List<BarberFullDetailsDto>> CompanyDetailsByIdAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var barbers = await dbContext.Barbers.Include(x => x.Company)
            .Where(x => x.Company.Id == companyId)
            .ToListAsync(cancellationToken);


        if (barbers == null || barbers.Count == 0)
        {
            return new List<BarberFullDetailsDto>();
        }


        return barbers.Select(barber =>
            new BarberFullDetailsDto(barber.BarberId,barber.BarberName, barber.Company.CompanyName)).ToList();
    }

    public async Task<List<CompanyDetailsDto>> GetAllCompaniesAsync(CompanyDetailsDto companyDetailsDto, CancellationToken cancellationToken)
    {
        var companies = await dbContext.Companies.ToListAsync(cancellationToken);

        var result = companies.Select(x => new CompanyDetailsDto()
        {
            CompanyId = x.Id,
            CompanyName = x.CompanyName
        }).ToList();
        return result;
    }
}