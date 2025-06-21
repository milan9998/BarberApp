using Hair.Application.Common.Interfaces;
using Hair.Application.Schedules.Commands;
using Hair.Application.Schedules.Queries;
using Hair.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

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
        try
        {
            var result = await Mediator.Send(command);
            
            return Ok(new { Message = "Successfully scheduled appointment.", Data = result });
        }
        catch (Exception ex)
        {
           
            return BadRequest(new { Message = ex.Message });
        }
      
        
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

    
}