using Hair.Application.Common.Dto.Company;
using Hair.Application.Common.Interfaces;
using MediatR;

namespace Hair.Application.Companies.Queries;

public record CompanyDetailsByIdQuery(Guid CompanyId) : IRequest<CompanyDetailsDto>;

public class CompanyDetailsByIdQueryHandler(ICompanyService companyService) : IRequestHandler<CompanyDetailsByIdQuery, CompanyDetailsDto>
{
    public async Task<CompanyDetailsDto> Handle(CompanyDetailsByIdQuery request, CancellationToken cancellationToken)
    {
        var companyDetailsById = await companyService.GetCompanyDetailsById(request.CompanyId, cancellationToken);
        return companyDetailsById;
    }
}