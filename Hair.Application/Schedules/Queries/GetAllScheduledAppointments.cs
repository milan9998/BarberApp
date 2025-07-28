using Hair.Application.Common.Dto.Schedule;
using Hair.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hair.Application.Schedules.Queries;

public record GetAllScheduledAppointments(Guid barberId) : IRequest<IList<GetAllUsedAppointmentsDto>>;

public class ProductDetailsQueryHandler(IScheduleService scheduleService) 
    : IRequestHandler<GetAllScheduledAppointments, IList<GetAllUsedAppointmentsDto>>
{
    public async Task<IList<GetAllUsedAppointmentsDto>> Handle(GetAllScheduledAppointments request, 
        CancellationToken cancellationToken)
    {
        var x = await scheduleService.GetAllSchedulesByBarberIdAsync(request.barberId, cancellationToken);
        return x;
    }
}