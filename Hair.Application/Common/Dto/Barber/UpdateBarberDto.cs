namespace Hair.Application.Common.Dto.Barber;

public record UpdateBarberDto(
    Guid BarberId,
    string BarberName,
    string PhoneNumber,
    string Email,
    TimeSpan IndividualStartTime,
    TimeSpan IndividualEndTime
);