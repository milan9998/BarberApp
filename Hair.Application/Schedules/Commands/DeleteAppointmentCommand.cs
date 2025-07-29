using Hair.Application.Common.Dto.Schedule;
using Hair.Application.Common.Interfaces;
using MediatR;

namespace Hair.Application.Schedules.Commands;

public record DeleteAppointmentCommand(Guid BarberId, DateTime SelectedDate):IRequest<FreeAppointmentsCheckDto>;
public class DeleteAppointmentCommandHandler(IScheduleService scheduleService) : IRequestHandler<DeleteAppointmentCommand, FreeAppointmentsCheckDto>
{
    public async Task<FreeAppointmentsCheckDto> Handle(DeleteAppointmentCommand request, CancellationToken cancellationToken)
    {
        var deleteAppointment = 
            await scheduleService.DeleteAppointmentByBarber(request.BarberId, request.SelectedDate, cancellationToken);
        return deleteAppointment;
    }
}