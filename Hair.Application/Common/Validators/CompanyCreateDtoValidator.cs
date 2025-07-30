using FluentValidation;
using Hair.Application.Common.Dto.Company;
using Hair.Application.Companies.Commands;

namespace Hair.Application.Common.Validators;

public class CompanyCreateDtoValidator : AbstractValidator<CompanyCreateDto>
{
    public CompanyCreateDtoValidator()
    {
        
        RuleFor(x => x.CompanyName).NotEmpty()
            .WithMessage("Naziv kompanije ne sme biti prazno!");
        RuleFor(x=> x.CompanyName).MinimumLength(3)
            .WithMessage("Naziv kompanije mora imati minimum 3 karaktera!");
        RuleFor(x=> x.CompanyName).MaximumLength(30)
            .WithMessage("Naziv kompanije ne sme imati više od 30 karaktera!");
        
    }
}