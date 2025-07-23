namespace Hair.Application.Common.Dto.Auth;

public record CompanyOwnerDetails(
    string OwnerId,
    string Email, 
    string FirstName,
    string LastName,
    string PhoneNumber
);