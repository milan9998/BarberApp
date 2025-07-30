namespace Hair.Application.Common.Dto.Auth;

public record GetAllAppointmentsByUserIdDto(
    Guid AppointmentId,
    DateTime Time,
    string HaircutName,
    string BarberName,
    string BarberPhone,
    string BarberEmail
);