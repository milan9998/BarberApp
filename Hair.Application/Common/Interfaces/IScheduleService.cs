using Hair.Application.Common.Dto.Schedule;

namespace Hair.Application.Common.Interfaces;

public interface IScheduleService
{
    Task<ScheduleAppointmentCreateDto> CreateScheduleAppointmentAsync(
        ScheduleAppointmentCreateDto scheduleAppointmentCreateDto, 
        CancellationToken cancellationToken
        );

    Task<List<GetAllSchedulesByBarberIdDto>> GetAllSchedulesByBarberIdAsync(
        Guid barberId,
        CancellationToken cancellationToken
    );
}