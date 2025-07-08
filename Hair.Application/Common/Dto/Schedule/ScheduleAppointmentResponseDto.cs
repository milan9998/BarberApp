namespace Hair.Application.Common.Dto.Schedule;

public record ScheduleAppointmentResponseDto(
    string? firstName,
    string? lastName,
    string? email,
    string phoneNumber,
    DateTime time,
    Guid barberId,
    string haircutName
    );