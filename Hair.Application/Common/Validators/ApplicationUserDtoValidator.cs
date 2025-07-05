using FluentValidation;
using Hair.Application.Common.Dto.Auth;
using Hair.Application.Common.Interfaces;
using Hair.Domain.Entities;

namespace Hair.Application.Common.Validators;

public class ApplicationUserDtoValidator : AbstractValidator<RegisterDto>
{
    private readonly IBarberService _barberService;
    
    public ApplicationUserDtoValidator(IBarberService barberService)
    {
        _barberService = barberService;
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x=>x.PhoneNumber)
            .Must(phone => _barberService.IsValidSerbianPhoneNumber(phone))
            .WithMessage("Invalid phone number format!");
        RuleFor(x => x.Email).Must(mail => barberService.IsValidEmail(mail)).WithMessage("Invalid email format!");
            
        
    }
}