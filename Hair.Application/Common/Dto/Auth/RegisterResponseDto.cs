namespace Hair.Application.Common.Dto.Auth;

public record RegisterResponseDto(
    string Email,
    string Message,
    bool RequiresEmailVerification
);
