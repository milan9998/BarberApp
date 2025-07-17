namespace Hair.Application.Common.Dto.Auth;

public record AuthResponseDto(
    string Email, 
    string Role,
    List<Guid> Companies
    
    );