using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Hair.Application.Common.Dto.Schedule;
using Hair.Application.Common.Interfaces;
using Hair.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ValidationException = FluentValidation.ValidationException;

namespace Hair.Infrastructure.Services;


/*
   public class ScheduleService(IHairDbContext dbContext, 
                             INotificationService notificationService,
                             IValidator<ScheduleAppointmentCreateDto> validator) : IScheduleService
 */
public class ScheduleService(IHairDbContext dbContext, 
                            IBarberService barberService, 
                            INotificationService notificationService) : IScheduleService
{
  
   public async Task<ScheduleAppointmentCreateDto> CreateScheduleAppointmentAsync(ScheduleAppointmentCreateDto schedule,
        CancellationToken cancellationToken)
    {

        bool isWithinWorkHours = await IsWithinBarberWorkHours(schedule, cancellationToken);
        if (!isWithinWorkHours)
        {
            throw new Exception("Barber is not available during the requested time.");
        }
        
        DateTime normalizedTime = new DateTime(
            schedule.time.Year,
            schedule.time.Month,
            schedule.time.Day,
            schedule.time.Hour,
            schedule.time.Minute,
            0, 
            0, 
            schedule.time.Kind 
        );

       
      /*  var occupiedAppointment = await dbContext.Appointments
            .FirstOrDefaultAsync(x => x.Time == normalizedTime, cancellationToken);*/
        var x = await IsAppointmentAvailable(schedule.barberId,normalizedTime, cancellationToken);
        
        if (x)
        {
            throw new ValidationException("Schedule appointment already exists.");
        }
      /*  bool checkEmail = barberService.IsValidEmail(schedule.email);
        if (!checkEmail)
        {
            throw new ValidationException("Invalid email address.");
        }*/
        try
        {
            Customer customer = new Customer(
                schedule.firstName,
                schedule.lastName,
                schedule.email,
                schedule.phoneNumber);
                
                
       
            Appointment appointment = new Appointment(schedule.time,schedule.barberId);
            appointment.SetHaircutName(schedule.haircut);

            appointment.SetTime(new DateTime(
                appointment.Time.Year,
                appointment.Time.Month,
                appointment.Time.Day,
                appointment.Time.Hour,
                appointment.Time.Minute,
                0, // Seconds set to 0
                0, // Milliseconds set to 0
                DateTimeKind.Utc
            ));
            
          
            await notificationService.SendSmsAsync(customer.PhoneNumber, "Zdravo");
            
            dbContext.Customers.Add(customer);
            dbContext.Appointments.Add(appointment);
            await dbContext.SaveChangesAsync(cancellationToken);
            return new ScheduleAppointmentCreateDto(customer.FirstName, customer.LastName, customer.Email,
                customer.PhoneNumber, appointment.Time, schedule.barberId, appointment.HaircutName);
        }
        catch (Exception ex)
        {
            

            throw new Exception(ex.Message);
        }
    }

    


    public async Task<List<GetAllSchedulesByBarberIdDto>> GetAllSchedulesByBarberIdAsync(
        Guid barberId, CancellationToken cancellationToken)
    {
        var appointments = await dbContext.Appointments.Where(x => barberId == x.Barberid).ToListAsync();
        var result = appointments.Select(appointment => new GetAllSchedulesByBarberIdDto
        {
            barberId = appointment.Barberid,
            time = appointment.Time
        }).ToList();
        return result;
    }
    
    private async Task<bool> IsWithinBarberWorkHours(ScheduleAppointmentCreateDto schedule, CancellationToken cancellationToken)
    {
        var barber = await dbContext.Barbers.FirstOrDefaultAsync(x => x.BarberId == schedule.barberId, cancellationToken);
        if (barber == null) return false;

        var start = barber.IndividualStartTime.Value;
        var end = barber.IndividualEndTime.Value;
        return schedule.time.TimeOfDay >= start && schedule.time.TimeOfDay < end;
    }

    private async Task<bool> IsAppointmentAvailable(Guid barberId, DateTime time, CancellationToken cancellationToken)
    {
        var occupied = await dbContext.Appointments
            .Where(x => x.Barberid == barberId && x.Time == time) // Proverava samo datog frizera
            .FirstOrDefaultAsync(cancellationToken);
        
        return occupied != null; // Termin je slobodan za datog frizera
    }

}
