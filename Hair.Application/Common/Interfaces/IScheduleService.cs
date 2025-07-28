using Hair.Application.Common.Dto.Schedule;

namespace Hair.Application.Common.Interfaces;

public interface IScheduleService
{
    Task<ScheduleAppointmentResponseDto> CreateScheduleAppointmentAsync(
        ScheduleAppointmentCreateDto scheduleAppointmentCreateDto, 
        CancellationToken cancellationToken
        );

    Task<List<GetAllUsedAppointmentsDto>> GetAllSchedulesByBarberIdAsync(
        Guid barberId, CancellationToken cancellationToken);

    Task<List<FreeAppointmentsCheckDto>> GetAllFreeAppointmentsQuery(
        DateTime selectedDate,
        Guid barberId,
        CancellationToken cancellationToken
    );
    

    
}