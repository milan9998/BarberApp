using Hair.Application.Common.Interfaces;
using Hair.Application.Schedules.Commands;
using Hair.Application.Schedules.Queries;
using Hair.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hair.Api.Controllers;
[ApiController]
[Route("schedule")]
public class ScheduleController: ApiBaseController
{
    IHairDbContext _dbContext;

    public ScheduleController(IHairDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Owner CRM: booking stats for one company (per barber + recent list + last 6 months).
    /// </summary>
    [HttpGet("company-crm")]
    public async Task<IActionResult> GetCompanyCrm([FromQuery] Guid companyId, CancellationToken cancellationToken)
    {
        var barbers = await _dbContext.Barbers
            .AsNoTracking()
            .Where(b => b.Company != null && b.Company.Id == companyId)
            .Select(b => new { b.BarberId, b.BarberName, b.Email, b.PhoneNumber })
            .ToListAsync(cancellationToken);

        var barberIds = barbers.Select(b => b.BarberId).ToList();
        var now = DateTime.UtcNow;
        var fromMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-5);

        var appointments = barberIds.Count == 0
            ? new List<Appointment>()
            : await _dbContext.Appointments
                .AsNoTracking()
                .Where(a => barberIds.Contains(a.Barberid))
                .OrderByDescending(a => a.Time)
                .ToListAsync(cancellationToken);

        var perBarber = barbers.Select(b =>
        {
            var barberAppts = appointments.Where(a => a.Barberid == b.BarberId).ToList();
            var upcoming = barberAppts.Count(a => a.Time >= now);
            var past = barberAppts.Count(a => a.Time < now);
            return new
            {
                barberId = b.BarberId,
                barberName = b.BarberName,
                email = b.Email,
                phoneNumber = b.PhoneNumber,
                total = barberAppts.Count,
                upcoming,
                completed = past
            };
        }).OrderByDescending(x => x.total).ToList();

        var byMonth = Enumerable.Range(0, 6)
            .Select(offset =>
            {
                var monthStart = fromMonth.AddMonths(offset);
                var count = appointments.Count(a => a.Time.Year == monthStart.Year && a.Time.Month == monthStart.Month);
                return new { year = monthStart.Year, month = monthStart.Month, count };
            })
            .ToList();

        var recent = appointments.Take(25).Select(a =>
        {
            var barber = barbers.FirstOrDefault(b => b.BarberId == a.Barberid);
            return new
            {
                appointmentId = a.Id,
                barberId = a.Barberid,
                barberName = barber?.BarberName ?? "—",
                time = a.Time,
                haircutName = a.HaircutName,
                isUpcoming = a.Time >= now
            };
        }).ToList();

        return Ok(new
        {
            companyId,
            totalAppointments = appointments.Count,
            upcomingAppointments = appointments.Count(a => a.Time >= now),
            completedAppointments = appointments.Count(a => a.Time < now),
            barberCount = barbers.Count,
            perBarber,
            byMonth,
            recent
        });
    }
    
    [HttpGet("appointments-per-month")]
    public IActionResult GetAppointmentsPerMonth()
    {
        var result = _dbContext.Appointments
            .GroupBy(a => new { Month = a.Time.Month, Year = a.Time.Year })  
            .Select(g => new 
            { 
                Year = g.Key.Year, 
                Month = g.Key.Month, 
                Count = g.Count() 
            })
            .OrderBy(g => g.Year)
            .ThenBy(g => g.Month)
            .ToList();

        return Ok(result);

    }
    [HttpGet("appointments-per-barber")]
    public IActionResult GetAppointmentsPerBarber()
    {
        var result = _dbContext.Appointments
            .GroupBy(a => new { barberId = a.Barberid })  
            .Select(g => new 
            { 
               
                barberId = g.Key.barberId, 
                Count = g.Count() 
            })
            
            .OrderBy(g => g.barberId)
            .ToList();

        return Ok(result);

    }

    [HttpPost ("CreateAppointment")]
    public async Task<ActionResult<Appointment>> CreateAppointmentAsync([FromForm]ScheduleAppointmentCommand command)
    {
       
            var result = await Mediator.Send(command);
            
            return Ok(new { Message = "Successfully scheduled appointment.", Data = result });
        
      
      
        
    }
    
    [HttpGet ("GetAllUsedAppointments")]
    public async Task<ActionResult<Appointment>> GetAllUsedAppointmentsAsync([FromQuery]GetAllScheduledAppointments query)
    {
        return Ok(await Mediator.Send(query));
    }

    
    [HttpGet ("GetAllFreeAppointments")]
    public async Task<ActionResult<Appointment>> GetAllFreeAppointmentsAsync([FromQuery]GetAllFreeAppointmentsQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    [HttpDelete("DeleteAppointment")]
    public async Task<ActionResult<Appointment>> DeleteAppointmentAsync([FromQuery] DeleteAppointmentCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    
}