namespace Hair.Application.Common.Dto.Auth;

public record CompanyOwnerResponseDto(string Email, Guid ?CompanyId, string FirstName, string LastName,string PhoneNumber);