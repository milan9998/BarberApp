using Hair.Application.Common.Dto.Barber;
using Hair.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hair.Application.Companies.Queries;

public record CompanyDetailsQuery(Guid CompanyId) : IRequest<List<BarberDetailsDto>>;

public class CompanyDetailsQueryHandler(ICompanyService companyService)
    : IRequestHandler<CompanyDetailsQuery, List<BarberDetailsDto>>
{
    public async Task<List<BarberDetailsDto>> Handle(CompanyDetailsQuery request, CancellationToken cancellationToken)
    {
        var x = await companyService.CompanyDetailsByIdAsync(request.CompanyId, cancellationToken);
        return x;
    }
}





/*
var barbers = await dbContext.Barbers.Include(x => x.Company)
    .Where(x => x.Company.Id == request.CompanyId)
    .ToListAsync(cancellationToken);


if (barbers == null || barbers.Count == 0)
{
    return new List<BarberDetailsDto>();
}


return barbers.Select(barber =>
    new BarberDetailsDto(barber.BarberName, barber.Company?.CompanyName ?? "No company")).ToList();
*/