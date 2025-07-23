using Hair.Application.Common.Dto.Barber;

namespace Hair.Application.Common.Interfaces;

public interface IBarberService
{
    Task<BarberResponseDto> BarberCreateAsync(
        BarberCreateDto barberCreateDto, 
        CancellationToken cancellationToken
    );

    Task<string> DeleteBarberAsync(Guid barberId, CancellationToken cancellationToken);

    Task<string> UpdateBarberAsync(UpdateBarberDto updateBarberDto, CancellationToken cancellationToken);
    Task<List<BarberDetailsDto>> GetAllBarbersAsync(
        Guid companyId, 
        CancellationToken cancellationToken
    );
    
    Boolean IsValidSerbianPhoneNumber(string phoneNumber);
    
    Boolean IsValidEmail(string email);
}