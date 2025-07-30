using FluentValidation;
using Hair.Application.Common.Dto.Barber;

namespace Hair.Application.Common.Validators;

public class BarberCreateDtoValidator : AbstractValidator<BarberCreateDto>
{
    public BarberCreateDtoValidator()
    {
        RuleFor(x => x.barberName).NotEmpty()
            .WithMessage("Ime frizera ne sme biti prazno!");
        RuleFor(x=> x.barberName).MinimumLength(3)
            .WithMessage("Ime frizera mora imati minimum 3 karaktera!");
        RuleFor(x=> x.barberName).MaximumLength(30)
            .WithMessage("Ime frizera ne sme imati vise od 30 karaktera!");
        RuleFor(x=> x.email).NotNull();
    }
}