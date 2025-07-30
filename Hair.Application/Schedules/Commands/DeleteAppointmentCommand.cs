using Hair.Application.Common.Dto.Schedule;
using Hair.Application.Common.Interfaces;
using MediatR;

namespace Hair.Application.Schedules.Commands;

public record DeleteAppointmentCommand(Guid AppointmentId) : IRequest<string>;

public class DeleteAppointmentCommandHandler(IScheduleService scheduleService) : IRequestHandler<DeleteAppointmentCommand, string>
{
    public async Task<string> Handle(DeleteAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointmentToDelete = await scheduleService.DeleteAppointmentByAppointmentIdAsync(request.AppointmentId, cancellationToken);
        return appointmentToDelete;
    }
}