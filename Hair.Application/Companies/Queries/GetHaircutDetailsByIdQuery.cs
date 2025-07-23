using Hair.Application.Common.Dto.Company;
using Hair.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hair.Application.Companies.Queries;

public record GetHaircutDetailsByIdQuery(Guid HaircutId) : IRequest<HaircutResponseDto>;

public class GetHaircutDetailsByIdQueryHandler(IHairDbContext dbContext) 
    : IRequestHandler<GetHaircutDetailsByIdQuery, HaircutResponseDto>
{
    public async Task<HaircutResponseDto> Handle(GetHaircutDetailsByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var haircut = await dbContext.Haircuts.FirstOrDefaultAsync(x => x.Id == request.HaircutId, cancellationToken);
            if (haircut == null)
            {
                throw new Exception("Usluga nije pronadjena sa zahtevanim id-jem!");
            }
            var response = new HaircutResponseDto(
                HaircutId: haircut.Id,
                HaircutType: haircut.HaircutType,
                Price: haircut.Price,
                Duration: haircut.Duration,
                CompanyId: haircut.CompanyId
            );
            return response;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}