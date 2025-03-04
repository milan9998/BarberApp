using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;
using Hair.Application.Common.Dto.Schedule;
using Hair.Application.Common.Interfaces;
using Hair.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hair.Application.Schedules.Queries;
public record GetAllFreeAppointmentsQuery(DateTime selectedDate, Guid barberId): IRequest<List<FreeAppointmentsCheckDto>>;

public class GetAllFreAppointmentsHandler(IHairDbContext dbContext) : IRequestHandler<GetAllFreeAppointmentsQuery, List<FreeAppointmentsCheckDto>>
{
    public async Task<List<FreeAppointmentsCheckDto>> Handle(GetAllFreeAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var occupiedAppointments = await dbContext.Appointments
            .Where(x => x.Time.Date == request.selectedDate.Date && x.Barberid == request.barberId)
            .ToListAsync(cancellationToken);

        var occupiedTimes = occupiedAppointments
            .Select(app => app.Time) // Čuvamo puni DateTime sa vremenom
            .ToList();


        var barberWorkTime = await dbContext.Barbers
            .Where(x=> x.BarberId == request.barberId)
            .FirstOrDefaultAsync(cancellationToken);

        var startTime = request.selectedDate.Date.AddHours(barberWorkTime.IndividualStartTime.Value.Hours)
            .AddMinutes(barberWorkTime.IndividualStartTime.Value.Minutes);

        var endTime = request.selectedDate.Date.AddHours(barberWorkTime.IndividualEndTime.Value.Hours)
            .AddMinutes(barberWorkTime.IndividualEndTime.Value.Minutes);


        var list = new List<DateTime>();


        for (var i = startTime; i <= endTime; i = i.AddMinutes(30))
        {
            list.Add(i);
        }

        list.RemoveAll(x => occupiedTimes.Contains(x));
        var list2 = list.Select(time => new FreeAppointmentsCheckDto(request.barberId, time)).ToList();

        return list2;

    }
}