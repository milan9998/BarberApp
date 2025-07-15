namespace Hair.Application.Common.Dto.Barber;

public record BarberResponseDto(Guid companyId,string barberName,string phoneNumber,string email,TimeSpan individualStartTime,TimeSpan individualEndTime);