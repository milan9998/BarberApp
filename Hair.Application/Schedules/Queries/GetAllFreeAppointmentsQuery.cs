using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;
using Hair.Application.Common.Dto.Schedule;
using Hair.Application.Common.Interfaces;
using Hair.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hair.Application.Schedules.Queries;
public record GetAllFreeAppointmentsQuery(DateTime selectedDate, Guid barberId): IRequest<List<FreeAppointmentsCheckDto>>;

public class GetAllFreAppointmentsHandler(IHairDbContext dbContext, IScheduleService scheduleService) : IRequestHandler<GetAllFreeAppointmentsQuery, List<FreeAppointmentsCheckDto>>
{
    public async Task<List<FreeAppointmentsCheckDto>> Handle(GetAllFreeAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var freeAppointments = await scheduleService.GetAllFreeAppointmentsQuery(request.selectedDate, request.barberId, cancellationToken);
        return freeAppointments;
    }
}