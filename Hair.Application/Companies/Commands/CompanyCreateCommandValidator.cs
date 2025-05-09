using FluentValidation;
using Hair.Application.Common.Dto.Company;
using Hair.Application.Common.Validators;

namespace Hair.Application.Companies.Commands;

public class CompanyCreateCommandValidator : AbstractValidator<CompanyCreateCommand>
{
    public CompanyCreateCommandValidator()
    {
        /*
        RuleFor(x=> x.Company).NotEmpty();
        RuleFor(x => x.Company).SetValidator(new CompanyCreateDtoValidator());
        */
    }
}