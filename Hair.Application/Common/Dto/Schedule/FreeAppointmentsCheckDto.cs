namespace Hair.Application.Common.Dto.Schedule;

public record FreeAppointmentsCheckDto(
        Guid barberId,
        DateTime dateAndTime
    );