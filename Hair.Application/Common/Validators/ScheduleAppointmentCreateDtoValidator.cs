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
            .GreaterThan(DateTime.UtcNow).WithMessage("You cannot schedule an appointment in the past");

        RuleFor(x => x.time.Minute)
            .Must(m => m % 30 == 0).WithMessage("Appointments must be scheduled in 30-minute intervals");

       
        RuleFor(x => x.phoneNumber)
            .Must(phone => _barberService.IsValidSerbianPhoneNumber(phone))
            .WithMessage("Invalid phone number format!");
        RuleFor(x => x.email).Must(mail => barberService.IsValidEmail(mail)).WithMessage("Invalid email format!");

    }

    
}