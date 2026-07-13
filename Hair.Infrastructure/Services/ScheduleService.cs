using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Hair.Application.Common.Dto.Schedule;
using Hair.Application.Common.Exceptions;
using Hair.Application.Common.Interfaces;
using Hair.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ValidationException = FluentValidation.ValidationException;

namespace Hair.Infrastructure.Services;

public class ScheduleService(
    IHairDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    ILogger<ScheduleService> _logger,
    IEmailService emailService) : IScheduleService
{
    public async Task<ScheduleAppointmentResponseDto> CreateScheduleAppointmentAsync(ScheduleAppointmentCreateDto schedule, CancellationToken cancellationToken)
    {

        try
        {

            bool isWithinWorkHours = await IsWithinBarberWorkHours(schedule, cancellationToken);
            if (!isWithinWorkHours)
            {
                throw new BadRequestException("Frizer nije dostupan u izabranom terminu.");
            }

            var userExists = await userManager.FindByEmailAsync(schedule.email);
            if (userExists is null)
            {
                throw new BadRequestException(
                    "Morate imati registrovan nalog sa ovim emailom da biste zakazali. Registrujte se i verifikujte email.");
            }

            if (!userExists.EmailConfirmed)
            {
                throw new BadRequestException(
                    "Email nije verifikovan. Potvrdite nalog pre zakazivanja (proverite inbox).");
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

            var isAvailableCheck = await IsAppointmentAvailable(schedule.barberId, normalizedTime, cancellationToken);
            if (isAvailableCheck)
            {
                throw new AppointmentConflictException("Ovaj termin je već zauzet. Izaberite drugi.");

            }

            var haircut = await dbContext.Haircuts.Where(x => x.Id == schedule.haircutId)
                .FirstOrDefaultAsync(cancellationToken);
            if (haircut is null)
            {
                throw new BadRequestException("Izabrana usluga nije pronađena.");
            }
            decimal haircutDuration = haircut.Duration;
            int requiredSlots = (int)Math.Ceiling(haircutDuration / 30m);

           
            bool canSchedule = await CanSchedule(userExists.PhoneNumber, schedule.time);
            if (!canSchedule)
            {
                throw new BadRequestException("Možete zakazati termin samo jednom u 7 dana.");
            }
            var allFreeAppointments =
                await GetAllFreeAppointmentsQuery(schedule.time.Date, schedule.barberId, cancellationToken);
            var freeTimes = allFreeAppointments.Select(dto => dto.dateAndTime).ToHashSet();
            List<DateTime> bookedAppointmentsTimes = new List<DateTime>();
            bool foundConsecutiveSlots = false;
            DateTime currentCheckTime = normalizedTime;

           
            if (!freeTimes.Contains(currentCheckTime))
            {
                throw new ValidationException("The requested start time is not available.");
            }

            if (requiredSlots <= 1)
            {

            }

            bookedAppointmentsTimes.Add(currentCheckTime);
            for (int i = 1; i < requiredSlots; i++)
            {
                currentCheckTime = normalizedTime.AddMinutes(i * 30);
                /*  var barber = await dbContext.Barbers.FirstOrDefaultAsync(x=> x.BarberId == schedule.barberId, cancellationToken);
                  if (barber == null)
                  {
                      throw new ValidationException("Barber not found for work hour check.");
                  }*/

                if (!freeTimes.Contains(currentCheckTime))
                {
                    foundConsecutiveSlots = false;
                    break;
                }

                bookedAppointmentsTimes.Add(currentCheckTime);
                foundConsecutiveSlots = true;

            }

            if (!foundConsecutiveSlots && bookedAppointmentsTimes.Count != requiredSlots)
            {
                throw new AppointmentConsecutiveException(
                    $"Nema dovoljno uzastopnih slobodnih termina za ovaj tretman koji ste izabrali  {haircutDuration}  minuta, izaberite drugi termin.");
            }
            foreach (var timeSlot in bookedAppointmentsTimes)
            {
                Appointment appointment = new Appointment(timeSlot, schedule.barberId)
                    .SetHaircutName(haircut.HaircutType);
    
                appointment.ApplicationUserId = userExists.Id; 

                dbContext.Appointments.Add(appointment);
            }
            await dbContext.SaveChangesAsync(cancellationToken);

            var barber = await dbContext.Barbers
                .FirstOrDefaultAsync(x => x.BarberId == schedule.barberId, cancellationToken);

            await SendBookingEmailsAsync(
                userExists,
                barber,
                haircut.HaircutType,
                bookedAppointmentsTimes[0],
                cancellationToken);

            return new ScheduleAppointmentResponseDto(userExists.FirstName, userExists.LastName, userExists.Email,
                userExists.PhoneNumber, bookedAppointmentsTimes[0], schedule.barberId, haircut.HaircutType);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Detaljan opis gde i šta se desilo u ScheduleService");
            throw exception;
        }

  
    }
    public async Task<bool> CanSchedule(string phoneNumber, DateTime requestedDate)
    {
        var fromDate = requestedDate.AddDays(-6); 
        var toDate = requestedDate.AddDays(6);    

        return !await dbContext.Appointments
            .AnyAsync(a => a.ApplicationUser.PhoneNumber == phoneNumber &&
                           a.Time >= fromDate &&
                           a.Time <= toDate);
    }

    public async Task<List<GetAllUsedAppointmentsDto>> GetAllSchedulesByBarberIdAsync(
        Guid barberId, CancellationToken cancellationToken)
    {
        
        var appointments = await dbContext.Appointments.Where(x => barberId == x.Barberid)
            .ToListAsync(cancellationToken);
        
        
        var result = appointments.Select(appointment => new GetAllUsedAppointmentsDto(
            AppointmentId: appointment.Id,
            BarberId: appointment.Barberid,
            Time: appointment.Time,
            HaircutName: appointment.HaircutName,
            ApplicationUserId: appointment.ApplicationUserId,
            FirstName: userManager.FindByIdAsync(appointment.ApplicationUserId).Result.FirstName,
            LastName: userManager.FindByIdAsync(appointment.ApplicationUserId).Result.LastName,
            Email: userManager.FindByIdAsync(appointment.ApplicationUserId).Result.Email,
            PhoneNumber: userManager.FindByIdAsync(appointment.ApplicationUserId).Result.PhoneNumber
        )).ToList();
        
        return result;
        
    }

    private async Task<bool> IsWithinBarberWorkHours(ScheduleAppointmentCreateDto schedule,
        CancellationToken cancellationToken)
    {
        var barber =
            await dbContext.Barbers.FirstOrDefaultAsync(x => x.BarberId == schedule.barberId, cancellationToken);
        if (barber == null) return false;

        var start = barber.IndividualStartTime;
        var end = barber.IndividualEndTime;
        return schedule.time.TimeOfDay >= start && schedule.time.TimeOfDay < end;
    }

    private async Task<bool> IsAppointmentAvailable(Guid barberId, DateTime time, CancellationToken cancellationToken)
    {
        var occupied = await dbContext.Appointments
            .Where(x => x.Barberid == barberId && x.Time == time) 
            .FirstOrDefaultAsync(cancellationToken);

        return occupied != null; 
    }

     public async Task<string> DeleteAppointmentByAppointmentIdAsync(Guid appointmentId, CancellationToken cancellationToken)
    {
        try
        {
            var appointment = await dbContext.Appointments.FirstOrDefaultAsync(x=> x.Id == appointmentId, cancellationToken);
            if (appointment == null)
            {
                throw new Exception("Nije pronadjen termin za brisanje!");
            }
            dbContext.Appointments.Remove(appointment);
            await dbContext.SaveChangesAsync(cancellationToken);

            return "Uspešno obrisan termin!";

        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            Console.WriteLine(e);
            throw;
        }
        
        
        
    }
    
    
    public async Task<List<FreeAppointmentsCheckDto>> GetAllFreeAppointmentsQuery(DateTime selectedDate, Guid barberId, CancellationToken cancellationToken)
    {
        var occupiedAppointments = await dbContext.Appointments
            .Where(x => x.Time.Date == selectedDate.Date && x.Barberid == barberId)
            .ToListAsync(cancellationToken);

        var occupiedTimes = occupiedAppointments
            .Select(app => app.Time) 
            .ToList();
        
        var barberWorkTime = await dbContext.Barbers
            .Where(x=> x.BarberId == barberId)
            .FirstOrDefaultAsync(cancellationToken);

        var startTime = selectedDate.Date.AddHours(barberWorkTime.IndividualStartTime.Hours)
            .AddMinutes(barberWorkTime.IndividualStartTime.Minutes);

        var endTime = selectedDate.Date.AddHours(barberWorkTime.IndividualEndTime.Hours)
            .AddMinutes(barberWorkTime.IndividualEndTime.Minutes);
        
        var list = new List<DateTime>();
        
        for (var i = startTime; i < endTime; i = i.AddMinutes(30))
        {
            list.Add(i);
        }

        list.RemoveAll(x => occupiedTimes.Contains(x));
        var list2 = list.Select(time => new FreeAppointmentsCheckDto(barberId, time)).ToList();

        return list2;
    }

    private async Task SendBookingEmailsAsync(
        ApplicationUser client,
        Barber? barber,
        string haircutName,
        DateTime appointmentTime,
        CancellationToken cancellationToken)
    {
        var timeLabel = appointmentTime.ToString("dd.MM.yyyy. HH:mm");

        try
        {
            var clientHtml = $"""
                <div style="font-family:Arial,sans-serif;line-height:1.55;color:#1d2a3f">
                  <h2>Termin je uspešno zakazan</h2>
                  <p>Zdravo {client.FirstName},</p>
                  <p>Vaš termin je potvrđen.</p>
                  <ul>
                    <li><strong>Usluga:</strong> {haircutName}</li>
                    <li><strong>Frizer:</strong> {barber?.BarberName ?? "-"}</li>
                    <li><strong>Termin:</strong> {timeLabel}</li>
                  </ul>
                  <p>Vidimo se u salonu.</p>
                  <p style="color:#6a7f9f;font-size:12px">Barber Control HQ</p>
                </div>
                """;

            await emailService.SendAsync(
                client.Email!,
                $"Potvrda termina - {timeLabel}",
                clientHtml,
                cancellationToken);

            if (!string.IsNullOrWhiteSpace(barber?.Email))
            {
                var barberHtml = $"""
                    <div style="font-family:Arial,sans-serif;line-height:1.55;color:#1d2a3f">
                      <h2>Nova rezervacija</h2>
                      <p>Zakazan je novi termin kod vas.</p>
                      <ul>
                        <li><strong>Klijent:</strong> {client.FirstName} {client.LastName}</li>
                        <li><strong>Email:</strong> {client.Email}</li>
                        <li><strong>Telefon:</strong> {client.PhoneNumber}</li>
                        <li><strong>Usluga:</strong> {haircutName}</li>
                        <li><strong>Termin:</strong> {timeLabel}</li>
                      </ul>
                      <p style="color:#6a7f9f;font-size:12px">Barber Control HQ</p>
                    </div>
                    """;

                await emailService.SendAsync(
                    barber.Email,
                    $"Nova rezervacija - {client.FirstName} {client.LastName} ({timeLabel})",
                    barberHtml,
                    cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Booking emails failed for appointment at {Time}", appointmentTime);
        }
    }
}
