using FluentValidation;
using Hair.Application.Common.Dto.Barber;

namespace Hair.Application.Common.Validators;

public class BarberCreateDtoValidator : AbstractValidator<BarberCreateDto>
{
    public BarberCreateDtoValidator()
    {
        RuleFor(x => x.barberName).NotEmpty();
        RuleFor(x=> x.barberName).MinimumLength(3);
        RuleFor(x=> x.barberName).MaximumLength(30);
        RuleFor(x=> x.email).NotNull();
    }
}