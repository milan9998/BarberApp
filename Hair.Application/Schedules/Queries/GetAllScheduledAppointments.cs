using Hair.Application.Common.Dto.Schedule;
using Hair.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hair.Application.Schedules.Queries;

public record GetAllScheduledAppointments(Guid barberId) : IRequest<IList<GetAllSchedulesByBarberIdDto>>;

public class ProductDetailsQueryHandler(IScheduleService scheduleService) 
    : IRequestHandler<GetAllScheduledAppointments, IList<GetAllSchedulesByBarberIdDto>>
{
    public async Task<IList<GetAllSchedulesByBarberIdDto>> Handle(GetAllScheduledAppointments request, 
        CancellationToken cancellationToken)
    {
        var x = await scheduleService.GetAllSchedulesByBarberIdAsync(request.barberId, cancellationToken);
        return x;
    }
}
