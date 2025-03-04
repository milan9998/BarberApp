using Hair.Application.Common.Dto.Barber;
using Hair.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hair.Application.Barbers.Queries;

public record GetAllBarbersQuery(Guid companyId) : IRequest<List<BarberDetailsDto>>;

public class GetAllBarbersQueryHandler (IBarberService barberService) : IRequestHandler<GetAllBarbersQuery, List<BarberDetailsDto>>
{
    public async Task<List<BarberDetailsDto>> Handle(GetAllBarbersQuery request, CancellationToken cancellationToken)
    {
        var x = await barberService.GetAllBarbersAsync(request.companyId, cancellationToken);
        return x;
    }
    
}

