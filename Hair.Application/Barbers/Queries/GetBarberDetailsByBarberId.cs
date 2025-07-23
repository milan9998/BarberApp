using Hair.Application.Common.Dto.Barber;
using Hair.Application.Common.Interfaces;
using Hair.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hair.Application.Barbers.Queries;

public record GetBarberDetailsByBarberId(Guid BarberId) : IRequest<BarberDetailsDto>;

public class GetBarberDetailsByBarberIdHandler(IHairDbContext dbContext) : IRequestHandler<GetBarberDetailsByBarberId, BarberDetailsDto>
{
    public async Task<BarberDetailsDto> Handle(GetBarberDetailsByBarberId request, CancellationToken cancellationToken)
    {
        var barber = await dbContext.Barbers.Include(c=>c.Company)
            .Where(x => x.BarberId == request.BarberId).FirstOrDefaultAsync();
        
        if (barber is null)
            throw new NotFoundException($"Barber with ID {request.BarberId} not found.", barber);

        var response = new BarberDetailsDto(
            BarberId: barber.BarberId,
            BarberName: barber.BarberName,
            CompanyName: barber.Company.CompanyName,
            PhoneNumber: barber.PhoneNumber,
            Email: barber.Email,
            CompanyId: barber.Company.Id,
            IndividualStartTime: barber.IndividualStartTime,
            IndividualEndTime: barber.IndividualEndTime
        );
        
        return response;

    }
}