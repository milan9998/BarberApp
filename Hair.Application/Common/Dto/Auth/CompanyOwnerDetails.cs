namespace Hair.Application.Common.Dto.Auth;

public record CompanyOwnerDetails(
    string OwnerId,
    string Email, 
    //Guid? CompanyId, 
    string Name,
    string PhoneNumber
    );