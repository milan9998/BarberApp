using FluentValidation;
using Hair.Application.Common.Dto.Schedule;
using Hair.Application.Common.Validators;

namespace Hair.Application.Schedules.Commands;

public class ScheduleAppointmentCommandValidator : AbstractValidator<ScheduleAppointmentCommand>
{
    public ScheduleAppointmentCommandValidator(IValidator<ScheduleAppointmentCreateDto> scheduleValidator)
    {
        RuleFor(x => x.Schedule).NotNull();
        RuleFor(x=>x.Schedule).SetValidator(scheduleValidator);
    }
}