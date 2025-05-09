using FluentValidation;
using Hair.Application.Common.Dto.Company;
using Hair.Application.Companies.Commands;

namespace Hair.Application.Common.Validators;

public class CompanyCreateDtoValidator : AbstractValidator<CompanyCreateDto>
{
    public CompanyCreateDtoValidator()
    {
        
        RuleFor(x => x.CompanyName).NotEmpty();
        RuleFor(x=> x.CompanyName).MinimumLength(3);
        RuleFor(x=> x.CompanyName).MaximumLength(30);
        
    }
}