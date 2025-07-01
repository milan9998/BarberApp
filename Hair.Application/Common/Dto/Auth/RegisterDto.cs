namespace Hair.Application.Common.Dto.Auth;

public record RegisterDto(string Email,string Password,string ConfirmPassword, string PhoneNumber,string FirstName,string LastName);