namespace Hair.Application.Common.Dto.Barber;

public record BarberCreateDto(Guid companyId,string barberName,string password,string phoneNumber,string email,TimeSpan individualStartTime,TimeSpan individualEndTime);