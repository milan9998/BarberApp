namespace Hair.Application.Common.Dto.Auth;

public record CompanyOwnerDto(string Email, Guid CompanyId, string FirstName, string LastName,string Password,string PhoneNumber);