using System.Data;
using FluentValidation;
using Hair.Application.Common.Dto.Schedule;
using Hair.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hair.Application.Common.Validators;

public class ScheduleAppointmentCreateDtoValidator : AbstractValidator<ScheduleAppointmentCreateDto>
{
   
    private readonly IBarberService _barberService;

    public ScheduleAppointmentCreateDtoValidator( IBarberService barberService)
    {
        
        _barberService = barberService;

        RuleFor(x => x.time)
            .GreaterThan(DateTime.Now).WithMessage("Ne možete zakazati termin u prošlosti");

        RuleFor(x => x.time.Minute)
            .Must(m => m % 30 == 0).WithMessage("Termini moraju biti zakazani u intervalima od 30 minuta");
        
        RuleFor(x => x.phoneNumber)
            .Must(phone => _barberService.IsValidSerbianPhoneNumber(phone))
            .WithMessage("Nevažeći format broja telefona!");
        
        RuleFor(x => x.email).Must(mail => barberService.IsValidEmail(mail))
            .WithMessage("Nevažeći format email adrese!");
    }

    
}