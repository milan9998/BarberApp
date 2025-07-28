namespace Hair.Application.Common.Dto.Schedule;

public record GetAllUsedAppointmentsDto(
    Guid AppointmentId,
    Guid BarberId,
    DateTime Time,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string HaircutName,
    string ApplicationUserId
);