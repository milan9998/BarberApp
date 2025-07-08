using System.Collections;
using Hair.Application.Common.Dto.Company;
using Hair.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hair.Application.Companies.Queries;

public record GetAllHaircutsByCompanyIdQuery(Guid CompanyId) : IRequest<List<HaircutDto>>;

public class GetAllHaircutsByCompanyIdQueryHandler(IHairDbContext dbContext): IRequestHandler<GetAllHaircutsByCompanyIdQuery,List<HaircutDto>>
{
    public async Task<List<HaircutDto>> Handle(GetAllHaircutsByCompanyIdQuery request, CancellationToken cancellationToken)
    {
        var haircuts = await dbContext.Haircuts.Where(x => x.CompanyId == request.CompanyId).ToListAsync(cancellationToken);
        var response = haircuts.Select(x => new HaircutDto(
            HaircutId: x.Id,
            HaircutType: x.HaircutType,
            Price: x.Price,
            Duration: x.Duration,
            CompanyId: x.CompanyId
        )).ToList();
        return response;
    }
}