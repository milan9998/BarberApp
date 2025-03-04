using FluentValidation;
using Hair.Application.Common.Dto.Barber;
using Hair.Application.Common.Validators;

namespace Hair.Application.Barbers.Commands;

public class BarberCreateCommandValidator : AbstractValidator<BarberCreateCommand>
{
    public BarberCreateCommandValidator()
    {
        RuleFor(x=> x.Barber).NotNull();
        RuleFor(x => x.Barber).SetValidator(new BarberCreateDtoValidator());
        

    }
}