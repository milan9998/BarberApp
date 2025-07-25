using Hair.Application.Common.Dto.Schedule;

namespace Hair.Application.Common.Interfaces;

public interface IScheduleService
{
    Task<ScheduleAppointmentResponseDto> CreateScheduleAppointmentAsync(
        ScheduleAppointmentCreateDto scheduleAppointmentCreateDto, 
        CancellationToken cancellationToken
        );

    Task<List<GetAllSchedulesByBarberIdDto>> GetAllSchedulesByBarberIdAsync(
        Guid barberId,
        CancellationToken cancellationToken
    );

    Task<List<FreeAppointmentsCheckDto>> GetAllFreeAppointmentsQuery(
        DateTime selectedDate,
        Guid barberId,
        CancellationToken cancellationToken
    );

    
}