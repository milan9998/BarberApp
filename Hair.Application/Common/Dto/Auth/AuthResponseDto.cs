namespace Hair.Application.Common.Dto.Auth;

public record AuthResponseDto(
    string UserId,
    string FirstName,
    string LastName,
    string Email, 
    string PhoneNumber,
    string Role,
    List<Guid> CompanyIds,
    Guid? BarberId
);