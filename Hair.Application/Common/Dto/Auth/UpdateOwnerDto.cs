namespace Hair.Application.Common.Dto.Auth;

public record UpdateOwnerDto(
    string OwnerId,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber
);