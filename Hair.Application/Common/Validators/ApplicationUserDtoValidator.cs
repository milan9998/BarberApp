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
        
        RuleFor(x => x.LastName).NotEmpty()
            .WithMessage("Prezime ne sme biti prazno!");
        RuleFor(x => x.FirstName).NotEmpty()
            .WithMessage("Ime ne sme biti prazno!");
        RuleFor(x=>x.PhoneNumber)
            .Must(phone => _barberService.IsValidSerbianPhoneNumber(phone))
            .WithMessage("Nevažeci format broja telefona!");
        RuleFor(x => x.Email).Must(mail => barberService.IsValidEmail(mail))
            .WithMessage("Nevažeći format email adrese!");
        
    }
}