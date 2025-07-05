using FluentValidation;
using FluentValidation.AspNetCore;
using Hair.Application.Common.Dto.Auth;
using Hair.Application.Common.Interfaces;
using Hair.Application.Common.Validators;

namespace Hair.Application.Auth.Commands;

public class ApplicationUserCommandValidator : AbstractValidator<RegisterCommand>
{
    public ApplicationUserCommandValidator(IValidator<RegisterDto> registerDtoValidator)
    {
        RuleFor(x => x.Register).SetValidator(registerDtoValidator);
    }
}