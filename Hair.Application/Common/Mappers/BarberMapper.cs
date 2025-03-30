using Hair.Application.Common.Dto.Barber;
using Hair.Domain.Entities;

namespace Hair.Application.Common.Mappers;

public static partial class BarberMapper
{
    public static Barber FromCreateDtoToEntityBarber(this BarberCreateDto dto)
    {
        var barber = new Barber(dto.barberName, dto.phoneNumber, dto.email, dto.individualStartTime, dto.individualEndTime);
        return barber;
    }
}