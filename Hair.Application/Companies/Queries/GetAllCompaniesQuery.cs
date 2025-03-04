using Hair.Application.Common.Dto.Company;
using Hair.Application.Common.Interfaces;
using Hair.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hair.Application.Companies.Queries;

public record GetAllCompaniesQuery(CompanyDetailsDto CompanyDetailsDto) : IRequest<IList<CompanyDetailsDto>>;

public class GetAllCompaniesQueryHandler(ICompanyService companyService) : IRequestHandler<GetAllCompaniesQuery, IList<CompanyDetailsDto>>
{
    public async Task<IList<CompanyDetailsDto>> Handle(GetAllCompaniesQuery request, CancellationToken cancellationToken)
    {
        var res = await companyService.GetAllCompaniesAsync(request.CompanyDetailsDto, cancellationToken);

        return res;
        
        /*
        var companies = await dbContext.Companies.ToListAsync(cancellationToken);

        var result = companies.Select(x => new CompanyDetailsDto()).ToList();
        return result;*/
    }
}