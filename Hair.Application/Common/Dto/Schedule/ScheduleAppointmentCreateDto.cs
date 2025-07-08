using Hair.Domain.Entities;

namespace Hair.Application.Common.Dto.Schedule;

public record ScheduleAppointmentCreateDto(
    string? firstName,
    string? lastName,
    string? email,
    string phoneNumber,
    DateTime time,
    Guid barberId,
    Guid haircutId
    );