﻿using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Hair.Application.Common.Dto.Schedule;
using Hair.Application.Common.Exceptions;
using Hair.Application.Common.Interfaces;
using Hair.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Twilio.TwiML.Voice;
using ValidationException = FluentValidation.ValidationException;

namespace Hair.Infrastructure.Services;


/*
   public class ScheduleService(IHairDbContext dbContext, 
                             INotificationService notificationService,
                             IValidator<ScheduleAppointmentCreateDto> validator) : IScheduleService
 */
public class ScheduleService(
    IHairDbContext dbContext,UserManager<ApplicationUser> userManager,
    ILogger<ScheduleService> _logger) : IScheduleService
{

    public async Task<ScheduleAppointmentResponseDto> CreateScheduleAppointmentAsync(ScheduleAppointmentCreateDto schedule, CancellationToken cancellationToken)
    {

        try
        {

            bool isWithinWorkHours = await IsWithinBarberWorkHours(schedule, cancellationToken);
            if (!isWithinWorkHours)
            {
                throw new Exception("Barber is not available during the requested time.");
            }

            var userExists = await userManager.FindByEmailAsync(schedule.email);
            if (userExists is null)
            {
                throw new Exception("Morate biti ulogovani da bi ste zakazali tretman.");
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

            var x = await IsAppointmentAvailable(schedule.barberId, normalizedTime, cancellationToken);
            if (x)
            {
                throw new AppointmentConflictException("Schedule appointment already exists.");

            }

            var haircut = await dbContext.Haircuts.Where(x => x.Id == schedule.haircutId)
                .FirstOrDefaultAsync(cancellationToken);
            decimal haircutDuration = haircut.Duration;
            int requiredSlots = (int)Math.Ceiling(haircutDuration / 30m);

            //Console.WriteLine("Haircut duration: " + Math.Ceiling(haircutDuration / 30));

            bool canSchedule = await CanSchedule(userExists.PhoneNumber, schedule.time);
            if (!canSchedule)
            {
                throw new Exception("Možete zakazati termin samo jednom u 7 dana.");
            }
            var allFreeAppointments =
                await GetAllFreeAppointmentsQuery(schedule.time.Date, schedule.barberId, cancellationToken);
            var freeTimes = allFreeAppointments.Select(dto => dto.dateAndTime).ToHashSet();
            List<DateTime> bookedAppointmentsTimes = new List<DateTime>();
            bool foundConsecutiveSlots = false;
            DateTime currentCheckTime = normalizedTime;

            // var requiredTimes = new List<DateTime>(); // čuva sve vreme koje je validno
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
    
                appointment.ApplicationUserId = userExists.Id; // ❗ OBAVEZNO: dodeli UserId

                dbContext.Appointments.Add(appointment);
            }
            await dbContext.SaveChangesAsync(cancellationToken);
            return new ScheduleAppointmentResponseDto(userExists.FirstName, userExists.LastName, userExists.Email,
                userExists.PhoneNumber, bookedAppointmentsTimes[0], schedule.barberId, haircut.HaircutType);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Detaljan opis gde i šta se desilo u ScheduleService");
            throw exception;
        }

        return null;
    }
    /*  try
      {
          AnonymousUser anonymousUser = new AnonymousUser(
              schedule.firstName,
              schedule.lastName,
              schedule.email,
              schedule.phoneNumber);
       //   Appointment appointment = new Appointment(schedule.time, schedule.barberId);

          


          dbContext.AnonymousUsers.Add(anonymousUser);

          
      }*/
    public async Task<bool> CanSchedule(string phoneNumber, DateTime requestedDate)
    {
        var fromDate = requestedDate.AddDays(-6); // uključuje danas i još 6 dana unazad
        var toDate = requestedDate.AddDays(6);    // uključuje i 6 dana unapred

        return !await dbContext.Appointments
            .AnyAsync(a => a.ApplicationUser.PhoneNumber == phoneNumber &&
                           a.Time >= fromDate &&
                           a.Time <= toDate);
    }

        public async Task<List<GetAllSchedulesByBarberIdDto>> GetAllSchedulesByBarberIdAsync(
        Guid barberId, CancellationToken cancellationToken)
    {
        var appointments = await dbContext.Appointments.Where(x => barberId == x.Id).ToListAsync();
        var result = appointments.Select(appointment => new GetAllSchedulesByBarberIdDto
        {
            barberId = appointment.Barberid,
            time = appointment.Time
        }).ToList();
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
            .Where(x => x.Barberid == barberId && x.Time == time) // Proverava samo datog frizera
            .FirstOrDefaultAsync(cancellationToken);

        return occupied != null; // Termin je slobodan za datog frizera
    }


    public async Task<List<FreeAppointmentsCheckDto>> GetAllFreeAppointmentsQuery(DateTime selectedDate, Guid barberId, CancellationToken cancellationToken)
    {
        var occupiedAppointments = await dbContext.Appointments
            .Where(x => x.Time.Date == selectedDate.Date && x.Barberid == barberId)
            .ToListAsync(cancellationToken);

        var occupiedTimes = occupiedAppointments
            .Select(app => app.Time) // Čuvamo puni DateTime sa vremenom
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
}
